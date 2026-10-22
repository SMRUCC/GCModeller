Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.Analysis.Circuits.Simulation

Namespace SelfTestFixtures

    ''' <summary>
    ''' The two node control network of the paper: the input node A and the
    ''' output node C with the four possible links A→C, C→A, A→A and C→C, which
    ''' gives 3^4 = 81 topologies. The paper shows that no two node network can
    ''' achieve adaptation, which is the first validation anchor of the search
    ''' system: the very same simulator, the very same step protocol and the
    ''' very same functional criteria have to produce Q = 0 for every one of the
    ''' 81 topologies.
    ''' (论文的双节点对照网络：输入节点 A 与输出节点 C，4 条可能链接共 81 个拓扑；
    ''' 论文证明没有任何双节点网络能实现适应性，这是搜索系统的第一个验证锚点)
    ''' </summary>
    ''' <remarks>
    ''' The model follows the same Michaelis Menten kinetics, the same background
    ''' enzyme rule and the same parameter layout as the three node
    ''' <see cref="SMRUCC.genomics.Analysis.Circuits.Model.CircuitModel"/>, so the
    ''' control experiment only differs in the network size.
    ''' </remarks>
    Public Class TwoNodeModel
        Implements IOdeModel

        ''' <summary>The number of link slots of the two node network, 2^2.</summary>
        Public Const SlotCount As Integer = 4
        ''' <summary>The fixed concentration of a background enzyme.</summary>
        Public Const BackgroundConcentration As Double = 0.5
        ''' <summary>The enzyme slot of the input signal: 0 = node A, 1 = node C, 2 = input.</summary>
        Public Const InputSlot As Integer = 2
        ''' <summary>The enzyme slot marker of a background enzyme.</summary>
        Public Const BackgroundSlot As Integer = -1

        Private ReadOnly m_actSrc As Integer()()
        Private ReadOnly m_actK As Double()()
        Private ReadOnly m_actKm As Double()()
        Private ReadOnly m_inhSrc As Integer()()
        Private ReadOnly m_inhK As Double()()
        Private ReadOnly m_inhKm As Double()()
        Private m_input As Double

        ''' <summary>
        ''' Build the reaction network of a two node circuit from its sign vector
        ''' and its sampled parameters.
        ''' </summary>
        ''' <param name="signs">the four slot sign vector in the order [A→C, C→A, A→A, C→C]</param>
        ''' <param name="params">the sampled parameter vector</param>
        Public Sub New(signs As SByte(), params As Double())
            If signs Is Nothing OrElse signs.Length <> SlotCount Then
                Throw New ArgumentException($"the sign vector must own {SlotCount} elements!", NameOf(signs))
            End If

            Dim expected As Integer = ParameterCount(signs)

            If params Is Nothing OrElse params.Length <> expected Then
                Throw New ArgumentException($"the network requires {expected} parameters, but got {If(params Is Nothing, -1, params.Length)}!", NameOf(params))
            End If

            Dim actSrc As New List(Of List(Of Integer))()
            Dim actK As New List(Of List(Of Double))()
            Dim actKm As New List(Of List(Of Double))()
            Dim inhSrc As New List(Of List(Of Integer))()
            Dim inhK As New List(Of List(Of Double))()
            Dim inhKm As New List(Of List(Of Double))()

            For j As Integer = 0 To 1
                Call actSrc.Add(New List(Of Integer)())
                Call actK.Add(New List(Of Double)())
                Call actKm.Add(New List(Of Double)())
                Call inhSrc.Add(New List(Of Integer)())
                Call inhK.Add(New List(Of Double)())
                Call inhKm.Add(New List(Of Double)())
            Next

            Dim cursor As Integer = 0

            ' 1. the active network links, in the fixed slot order
            For slot As Integer = 0 To SlotCount - 1
                Dim s As SByte = signs(slot)

                If s = 0 Then
                    Continue For
                End If

                Dim link = LinkAt(slot)
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
            Call actSrc(0).Add(InputSlot)
            Call actK(0).Add(params(cursor))
            Call actKm(0).Add(params(cursor + 1))
            cursor += 2

            ' 3. the background enzymes of the nodes that lack a regulator
            For j As Integer = 0 To 1
                If actSrc(j).Count = 0 Then
                    Call actSrc(j).Add(BackgroundSlot)
                    Call actK(j).Add(BackgroundConcentration * params(cursor))
                    Call actKm(j).Add(params(cursor + 1))
                    cursor += 2
                End If

                If inhSrc(j).Count = 0 Then
                    Call inhSrc(j).Add(BackgroundSlot)
                    Call inhK(j).Add(BackgroundConcentration * params(cursor))
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
        ''' The current value of the input signal.
        ''' </summary>
        Public Property Input As Double Implements IOdeModel.Input
            Get
                Return m_input
            End Get
            Set(value As Double)
                m_input = value
            End Set
        End Property

        ''' <summary>
        ''' Evaluate the right hand side of the two node ODE system.
        ''' </summary>
        Public Sub Evaluate(y As Double(), ydot As Double()) Implements IOdeModel.Evaluate
            For j As Integer = 0 To 1
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
        ''' signal, or the unit concentration of a background enzyme.
        ''' </summary>
        Private Function EnzymeValue(slot As Integer, y As Double()) As Double
            If slot = 0 OrElse slot = 1 Then
                Return y(slot)
            ElseIf slot = InputSlot Then
                Return m_input
            Else
                Return 1.0
            End If
        End Function

        ''' <summary>
        ''' The (source, target) node pair of a link slot, in the order
        ''' [A→C, C→A, A→A, C→C].
        ''' </summary>
        Public Shared Function LinkAt(slot As Integer) As (source As Integer, target As Integer)
            Select Case slot
                Case 0
                    Return (0, 1)
                Case 1
                    Return (1, 0)
                Case 2
                    Return (0, 0)
                Case Else
                    Return (1, 1)
            End Select
        End Function

        ''' <summary>
        ''' The number of active links of a two node sign vector.
        ''' </summary>
        Public Shared Function ActiveLinks(signs As SByte()) As Integer
            Return signs.Where(Function(s) s <> 0).Count()
        End Function

        ''' <summary>
        ''' The number of background enzymes that a two node sign vector
        ''' requires.
        ''' </summary>
        Public Shared Function BackgroundLinks(signs As SByte()) As Integer
            Dim n As Integer = 0

            For j As Integer = 0 To 1
                Dim hasActivator As Boolean = j = 0 ' the input node always owns the input link
                Dim hasInactivator As Boolean = False

                For slot As Integer = 0 To SlotCount - 1
                    Dim s As SByte = signs(slot)
                    Dim link = LinkAt(slot)

                    If link.target <> j OrElse s = 0 Then
                        Continue For
                    End If

                    If s > 0 Then
                        hasActivator = True
                    Else
                        hasInactivator = True
                    End If
                Next

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
        ''' The parameter count of a two node sign vector: one pair per active
        ''' link, one pair for the input link and one pair per background enzyme.
        ''' </summary>
        Public Shared Function ParameterCount(signs As SByte()) As Integer
            Return 2 * (ActiveLinks(signs) + 1 + BackgroundLinks(signs))
        End Function

        ''' <summary>
        ''' Whether the input node A can reach the output node C through the
        ''' active links of the sign vector.
        ''' </summary>
        Public Shared Function IsConnected(signs As SByte()) As Boolean
            Dim queue As New List(Of Integer)()
            Dim visited As Boolean() = New Boolean(1) {}

            queue.Add(0)
            visited(0) = True

            For cursor As Integer = 0 To queue.Count - 1
                Dim u As Integer = queue(cursor)

                For slot As Integer = 0 To SlotCount - 1
                    Dim link = LinkAt(slot)

                    If link.source <> u OrElse signs(slot) = 0 Then
                        Continue For
                    End If

                    If link.target = 1 Then
                        Return True
                    End If

                    If Not visited(link.target) Then
                        visited(link.target) = True
                        queue.Add(link.target)
                    End If
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Enumerate every two node topology: the 81 sign vectors in the
        ''' identifier order, optionally filtered by the input to output
        ''' connectivity.
        ''' (枚举全部双节点拓扑：按编号顺序的 81 个符号向量，可按输入输出连通性过滤)
        ''' </summary>
        Public Shared Iterator Function Enumerate(Optional requireConnected As Boolean = False) As IEnumerable(Of SByte())
            For id As Integer = 0 To 80
                Dim signs As SByte() = New SByte(SlotCount - 1) {}
                Dim rest As Integer = id

                For i As Integer = 0 To SlotCount - 1
                    signs(i) = CSByte(rest Mod 3 - 1)
                    rest \= 3
                Next

                If Not requireConnected OrElse IsConnected(signs) Then
                    Yield signs
                End If
            Next
        End Function

    End Class

End Namespace
