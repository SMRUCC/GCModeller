Imports System.Globalization
Imports System.Linq
Imports System.Text

Namespace Model

    ''' <summary>
    ''' The biochemical instantiation of a network topology: the enzyme reaction
    ''' network of the Michaelis Menten model of Ma et al. 2009, built from a
    ''' topology plus its sampled kinetic parameters.
    ''' (拓扑 + 参数之后的生化反应网络，即 ODE 右端的编译结果)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Every node owns a fixed total concentration of 1 which is split between
    ''' an active form ``y_j`` and an inactive form ``1 - y_j``. Following the
    ''' Equation (7) of the paper, the dynamics of a node is the sum of the
    ''' Michaelis Menten terms of its activating enzymes minus the terms of its
    ''' inactivating enzymes:
    ''' </para>
    ''' <code>
    ''' dy_j/dt = Σ_i X_i * k_i * (1-y_j)/((1-y_j) + K_i)
    '''         - Σ_i Y_i * k'_i * y_j/(y_j + K'_i)
    ''' </code>
    ''' <para>
    ''' where ``X_i`` are the activating enzymes of the node (the positive
    ''' regulators, the input signal and the background enzyme E) and ``Y_i``
    ''' are the inactivating enzymes (the negative regulators and the background
    ''' enzyme F). No cooperativity is assumed, so the Hill coefficient is 1.
    ''' </para>
    ''' <para>
    ''' The parameter vector layout is deterministic and shared with
    ''' <see cref="ParameterSampler"/>:
    ''' </para>
    ''' <list type="number">
    ''' <item> every active network link in the fixed slot order owns a (k, K) pair, </item>
    ''' <item> the input link (the signal I acts on node A) owns one (k, K) pair, </item>
    ''' <item> every background enzyme owns one (k, K) pair, in the node order A, B, C, the activator of a node before its inactivator. </item>
    ''' </list>
    ''' <para>
    ''' Background enzymes are introduced whenever a node lacks an activator or
    ''' an inactivator, so that every node can be switched on and off. Their
    ''' concentration is fixed at 0.5 (a non sampled constant of the paper) and
    ''' is folded into their rate constant here.
    ''' </para>
    ''' </remarks>
    Public Class CircuitModel

        ''' <summary>The fixed concentration of a background enzyme E_i or F_i.</summary>
        Public Const BackgroundEnzymeConcentration As Double = 0.5
        ''' <summary>The enzyme slot of the input signal inside of the reaction lists.</summary>
        Public Const InputEnzymeSlot As Integer = 3
        ''' <summary>The enzyme slot marker of a background enzyme, its concentration is folded into k.</summary>
        Public Const BackgroundEnzymeSlot As Integer = -1

        Private ReadOnly m_topology As NetworkTopology
        Private ReadOnly m_actSrc As Integer()()
        Private ReadOnly m_actK As Double()()
        Private ReadOnly m_actKm As Double()()
        Private ReadOnly m_inhSrc As Integer()()
        Private ReadOnly m_inhK As Double()()
        Private ReadOnly m_inhKm As Double()()
        Private m_input As Double

        ''' <summary>
        ''' Build the reaction network of a circuit from its topology and the
        ''' sampled kinetic parameters.
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        ''' <param name="params">the sampled parameter vector, its length must be <see cref="ParameterCount"/></param>
        ''' <exception cref="ArgumentException">the parameter vector length does not match the topology</exception>
        Public Sub New(topology As NetworkTopology, params As Double())
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim expected As Integer = ParameterCount(topology)

            If params Is Nothing OrElse params.Length <> expected Then
                Throw New ArgumentException($"the topology {topology.Id} requires {expected} parameters, but got {If(params Is Nothing, -1, params.Length)}!", NameOf(params))
            End If

            m_topology = topology

            Dim actSrc As New List(Of List(Of Integer))()
            Dim actK As New List(Of List(Of Double))()
            Dim actKm As New List(Of List(Of Double))()
            Dim inhSrc As New List(Of List(Of Integer))()
            Dim inhK As New List(Of List(Of Double))()
            Dim inhKm As New List(Of List(Of Double))()

            For j As Integer = 0 To NetworkTopology.NodeCount - 1
                Call actSrc.Add(New List(Of Integer)())
                Call actK.Add(New List(Of Double)())
                Call actKm.Add(New List(Of Double)())
                Call inhSrc.Add(New List(Of Integer)())
                Call inhK.Add(New List(Of Double)())
                Call inhKm.Add(New List(Of Double)())
            Next

            Dim cursor As Integer = 0

            ' 1. the active network links, in the fixed slot order
            For slot As Integer = 0 To NetworkTopology.LinkSlots - 1
                Dim s As SByte = topology.Signs(slot)

                If s = 0 Then
                    Continue For
                End If

                Dim link = NetworkTopology.LinkAt(slot)
                Dim k As Double = params(cursor)
                Dim km As Double = params(cursor + 1)

                cursor += 2

                If s > 0 Then
                    Call actSrc(link.target).Add(link.source)
                    Call actK(link.target).Add(k)
                    Call actKm(link.target).Add(km)
                Else
                    Call inhSrc(link.target).Add(link.source)
                    Call inhK(link.target).Add(k)
                    Call inhKm(link.target).Add(km)
                End If
            Next

            ' 2. the input link: the signal I activates the input node A
            Call actSrc(NetworkTopology.InputNode).Add(InputEnzymeSlot)
            Call actK(NetworkTopology.InputNode).Add(params(cursor))
            Call actKm(NetworkTopology.InputNode).Add(params(cursor + 1))
            cursor += 2

            ' 3. the background enzymes of the nodes that lack an activator or an inactivator
            For j As Integer = 0 To NetworkTopology.NodeCount - 1
                If actSrc(j).Count = 0 Then
                    Call actSrc(j).Add(BackgroundEnzymeSlot)
                    Call actK(j).Add(BackgroundEnzymeConcentration * params(cursor))
                    Call actKm(j).Add(params(cursor + 1))
                    cursor += 2
                End If

                If inhSrc(j).Count = 0 Then
                    Call inhSrc(j).Add(BackgroundEnzymeSlot)
                    Call inhK(j).Add(BackgroundEnzymeConcentration * params(cursor))
                    Call inhKm(j).Add(params(cursor + 1))
                    cursor += 2
                End If
            Next

            m_actSrc = actSrc.Select(Function(l) l.ToArray()).ToArray()
            m_actK = actK.Select(Function(l) l.ToArray()).ToArray()
            m_actKm = actKm.Select(Function(l) l.ToArray()).ToArray()
            m_inhSrc = inhSrc.Select(Function(l) l.ToArray()).ToArray()
            m_inhK = inhK.Select(Function(l) l.ToArray()).ToArray()
            m_inhKm = inhKm.Select(Function(l) l.ToArray()).ToArray()
        End Sub

        ''' <summary>
        ''' The topology of this circuit.
        ''' </summary>
        Public ReadOnly Property Topology As NetworkTopology
            Get
                Return m_topology
            End Get
        End Property

        ''' <summary>
        ''' The current value of the input signal, it is applied to the input
        ''' node A as an activating enzyme.
        ''' (当前输入信号强度，作为作用于节点 A 的激活酶)
        ''' </summary>
        Public Property Input As Double
            Get
                Return m_input
            End Get
            Set(value As Double)
                m_input = value
            End Set
        End Property

        ''' <summary>
        ''' The number of sampled parameters of a circuit built on the given
        ''' topology: two parameters for every active network link, two for the
        ''' input link and two for every background enzyme.
        ''' (一个拓扑对应的参数量，即 LHS 采样的维度)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Shared Function ParameterCount(topology As NetworkTopology) As Integer
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Return 2 * (topology.LinkCount + 1 + BackgroundLinkCount(topology))
        End Function

        ''' <summary>
        ''' The number of background enzymes that the given topology requires:
        ''' one inactivator for every node without a negative in link, one
        ''' activator for every node without a positive in link. The input node A
        ''' always owns the input link as its activator, so it never lacks one.
        ''' (拓扑需要补全的背景酶数量)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Shared Function BackgroundLinkCount(topology As NetworkTopology) As Integer
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim n As Integer = 0

            For j As Integer = 0 To NetworkTopology.NodeCount - 1
                Dim inLinks = topology.InLinks(j)
                Dim hasActivator As Boolean = j = NetworkTopology.InputNode OrElse inLinks.Any(Function(l) l.sign > 0)
                Dim hasInactivator As Boolean = inLinks.Any(Function(l) l.sign < 0)

                If Not hasActivator Then
                    n += 1
                End If
                If Not hasInactivator Then
                    n += 1
                End If
            Next

            Return n
        End Function

        ''' <summary>
        ''' Evaluate the right hand side of the ODE system, i.e. the time
        ''' derivative of the active form of every node.
        ''' (计算 ODE 右端，即各节点活性形式的时间导数)
        ''' </summary>
        ''' <param name="y">the active form concentration of every node, values inside of [0, 1]</param>
        ''' <param name="ydot">the output derivative vector, it is overwritten</param>
        Public Sub Evaluate(y As Double(), ydot As Double())
            For j As Integer = 0 To NetworkTopology.NodeCount - 1
                Dim yj As Double = y(j)
                Dim free As Double = 1.0 - yj
                Dim s As Double = 0.0
                Dim aSrc As Integer() = m_actSrc(j)
                Dim aK As Double() = m_actK(j)
                Dim aKm As Double() = m_actKm(j)

                For m As Integer = 0 To aSrc.Length - 1
                    s += EnzymeValue(aSrc(m), y) * aK(m) * free / (free + aKm(m))
                Next

                Dim iSrc As Integer() = m_inhSrc(j)
                Dim iK As Double() = m_inhK(j)
                Dim iKm As Double() = m_inhKm(j)

                For m As Integer = 0 To iSrc.Length - 1
                    s -= EnzymeValue(iSrc(m), y) * iK(m) * yj / (yj + iKm(m))
                Next

                ydot(j) = s
            Next
        End Sub

        ''' <summary>
        ''' The concentration of an enzyme: the active form of a node, the input
        ''' signal, or the unit concentration of a background enzyme whose
        ''' constant 0.5 concentration is already folded into its rate constant.
        ''' (酶浓度查询：节点活性形式 / 输入信号 / 背景酶)
        ''' </summary>
        ''' <param name="slot">the enzyme slot: 0..2 for the nodes, <see cref="InputEnzymeSlot"/> for the input, <see cref="BackgroundEnzymeSlot"/> for a background enzyme</param>
        ''' <param name="y">the current node state</param>
        Private Function EnzymeValue(slot As Integer, y As Double()) As Double
            If slot >= 0 AndAlso slot < NetworkTopology.NodeCount Then
                Return y(slot)
            ElseIf slot = InputEnzymeSlot Then
                Return m_input
            Else
                Return 1.0
            End If
        End Function

        ''' <summary>
        ''' A human readable description of the reaction network, which is used
        ''' by the self tests to verify the parameter binding of a circuit.
        ''' (反应网络的可读描述，供调试与自测使用)
        ''' </summary>
        Public Function Describe() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"topology {m_topology.Id} ({m_topology.ToSymbolString()}), input I = {m_input.ToString("G6", Globalization.CultureInfo.InvariantCulture)}")

            For j As Integer = 0 To NetworkTopology.NodeCount - 1
                Call sb.Append($"d{NetworkTopology.NodeNames(j)}/dt = ")

                Dim first As Boolean = True

                For m As Integer = 0 To m_actSrc(j).Length - 1
                    If Not first Then
                        Call sb.Append(" + ")
                    End If

                    Call sb.Append(EnzymeName(m_actSrc(j)(m)))
                    Call sb.Append($"*k={m_actK(j)(m).ToString("G6", Globalization.CultureInfo.InvariantCulture)},K={m_actKm(j)(m).ToString("G6", Globalization.CultureInfo.InvariantCulture)}")
                    first = False
                Next

                For m As Integer = 0 To m_inhSrc(j).Length - 1
                    If Not first Then
                        Call sb.Append(" - ")
                    Else
                        Call sb.Append("-")
                    End If

                    Call sb.Append(EnzymeName(m_inhSrc(j)(m)))
                    Call sb.Append($"*k'={m_inhK(j)(m).ToString("G6", Globalization.CultureInfo.InvariantCulture)},K'={m_inhKm(j)(m).ToString("G6", Globalization.CultureInfo.InvariantCulture)}")
                    first = False
                Next

                Call sb.AppendLine()
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' The display name of an enzyme slot.
        ''' </summary>
        Private Shared Function EnzymeName(slot As Integer) As String
            If slot >= 0 AndAlso slot < NetworkTopology.NodeCount Then
                Return NetworkTopology.NodeNames(slot)
            ElseIf slot = InputEnzymeSlot Then
                Return "I"
            Else
                Return "E/F(bg)"
            End If
        End Function

    End Class

End Namespace
