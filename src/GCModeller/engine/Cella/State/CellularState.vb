' ============================================================
' CellularState.vb - 虚拟细胞状态中枢
' ============================================================
' 所有子网络之间唯一的通信媒介。
'
' 设计动机：如果让各个子网络互相直接引用（GRN 读代谢网络、代谢网络读
' 翻译系统……），会形成循环依赖且无法独立测试。改为所有子网络只与
' CellularState 交互：从指定的状态池读输入、把输出写回指定的状态池。
'
' 状态池全部以 Double() 向量 + 名称索引映射表示，热路径上不做字符串
' 查找，也不反复分配数组。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Enum StatePool
    ''' <summary>转录本丰度 [gene]</summary>
    mRNA
    ''' <summary>蛋白质 / 酶水平 [gene]</summary>
    Protein
    ''' <summary>胞内代谢物浓度（Metaboliq 归一化空间）[internal metabolite]</summary>
    Metabolite
    ''' <summary>边界（胞外）代谢物浓度 [boundary metabolite]</summary>
    Boundary
    ''' <summary>信号转导通路上的磷酸化水平 / 转录因子活性 [signal channel]</summary>
    Signal
End Enum

''' <summary>
''' 虚拟细胞的状态中枢
''' </summary>
Public Class CellularState

    ' ---------------- 名称与索引 ----------------

    Public ReadOnly Property GeneNames As String()
    Public ReadOnly Property MetaboliteNames As String()
    Public ReadOnly Property BoundaryNames As String()
    Public ReadOnly Property SignalNames As String()

    Public ReadOnly Property GeneIndex As Dictionary(Of String, Integer)
    Public ReadOnly Property MetaboliteIndex As Dictionary(Of String, Integer)
    Public ReadOnly Property BoundaryIndex As Dictionary(Of String, Integer)
    Public ReadOnly Property SignalIndex As Dictionary(Of String, Integer)

    ' ---------------- 状态池 ----------------

    ''' <summary>转录本丰度 [gene]</summary>
    Public Property mRNA As Double()
    ''' <summary>蛋白质 / 酶水平 [gene]</summary>
    Public Property Protein As Double()
    ''' <summary>胞内代谢物浓度 [metabolite]，与 Metaboliq 的隐藏状态同处归一化空间</summary>
    Public Property Metabolite As Double()
    ''' <summary>边界代谢物浓度 [boundary]，由跨膜转运系统写入，作为代谢网络的外部驱动</summary>
    Public Property Boundary As Double()
    ''' <summary>信号转导通路状态 [signal]，即各转录因子的活化水平</summary>
    Public Property Signal As Double()

    ''' <summary>细胞周期相位（弧度，0 ~ 2π）</summary>
    Public Property CyclePhase As Double
    ''' <summary>物质回收池的存量（周转系统产生，补充到代谢物池）</summary>
    Public Property RecyclePool As Double

    ''' <summary>基因数量</summary>
    Public ReadOnly Property NGene As Integer
        Get
            Return GeneNames.Length
        End Get
    End Property

    ''' <summary>胞内代谢物数量</summary>
    Public ReadOnly Property NMetabolite As Integer
        Get
            Return MetaboliteNames.Length
        End Get
    End Property

    ''' <summary>边界代谢物数量</summary>
    Public ReadOnly Property NBoundary As Integer
        Get
            Return BoundaryNames.Length
        End Get
    End Property

    ''' <summary>信号通道数量</summary>
    Public ReadOnly Property NSignal As Integer
        Get
            Return SignalNames.Length
        End Get
    End Property

    Sub New(genes As IEnumerable(Of String),
            metabolites As IEnumerable(Of String),
            boundary As IEnumerable(Of String),
            signals As IEnumerable(Of String),
            Optional initialLevel As Double = 0.0)

        GeneNames = genes.SafeQuery.ToArray
        MetaboliteNames = metabolites.SafeQuery.ToArray
        BoundaryNames = boundary.SafeQuery.ToArray
        SignalNames = signals.SafeQuery.ToArray

        GeneIndex = BuildIndex(GeneNames)
        MetaboliteIndex = BuildIndex(MetaboliteNames)
        BoundaryIndex = BuildIndex(BoundaryNames)
        SignalIndex = BuildIndex(SignalNames)

        mRNA = Filled(GeneNames.Length, initialLevel)
        Protein = Filled(GeneNames.Length, initialLevel)
        Metabolite = Filled(MetaboliteNames.Length, initialLevel)
        Boundary = Filled(BoundaryNames.Length, initialLevel)
        Signal = Filled(SignalNames.Length, initialLevel)
    End Sub

    Private Shared Function BuildIndex(names As String()) As Dictionary(Of String, Integer)
        Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To names.Length - 1
            map(names(i)) = i
        Next

        Return map
    End Function

    Private Shared Function Filled(n As Integer, level As Double) As Double()
        Dim v As Double() = New Double(n - 1) {}

        If level <> 0.0 Then
            For i As Integer = 0 To n - 1
                v(i) = level
            Next
        End If

        Return v
    End Function

    ' ---------------- 向量访问 ----------------

    ''' <summary>
    ''' 取指定状态池的底层向量（热路径直接操作数组，避免字典查找）
    ''' </summary>
    Public Function Vector(pool As StatePool) As Double()
        Select Case pool
            Case StatePool.mRNA : Return mRNA
            Case StatePool.Protein : Return Protein
            Case StatePool.Metabolite : Return Metabolite
            Case StatePool.Boundary : Return Boundary
            Case StatePool.Signal : Return Signal
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(pool))
        End Select
    End Function

    Public Function IndexOf(pool As StatePool) As Dictionary(Of String, Integer)
        Select Case pool
            Case StatePool.mRNA, StatePool.Protein : Return GeneIndex
            Case StatePool.Metabolite : Return MetaboliteIndex
            Case StatePool.Boundary : Return BoundaryIndex
            Case StatePool.Signal : Return SignalIndex
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(pool))
        End Select
    End Function

    Public Function NamesOf(pool As StatePool) As String()
        Select Case pool
            Case StatePool.mRNA, StatePool.Protein : Return GeneNames
            Case StatePool.Metabolite : Return MetaboliteNames
            Case StatePool.Boundary : Return BoundaryNames
            Case StatePool.Signal : Return SignalNames
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(pool))
        End Select
    End Function

    ' ---------------- 按名称访问 ----------------

    Public Function Level(pool As StatePool, name As String) As Double
        Dim idx As Integer = -1

        If IndexOf(pool).TryGetValue(name, idx) Then
            Return Vector(pool)(idx)
        Else
            Return 0.0
        End If
    End Function

    Public Sub SetLevel(pool As StatePool, name As String, value As Double)
        Dim idx As Integer = -1

        If IndexOf(pool).TryGetValue(name, idx) Then
            Vector(pool)(idx) = value
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function TryLevel(pool As StatePool, name As String, ByRef value As Double) As Boolean
        Dim idx As Integer = -1

        If IndexOf(pool).TryGetValue(name, idx) Then
            value = Vector(pool)(idx)
            Return True
        Else
            value = 0.0
            Return False
        End If
    End Function

    ''' <summary>
    ''' 把状态池导出为 名称 => 数值 字典（快照输出用）
    ''' </summary>
    Public Function AsDictionary(pool As StatePool) As Dictionary(Of String, Double)
        Dim v As Double() = Vector(pool)
        Dim names As String() = NamesOf(pool)
        Dim out As New Dictionary(Of String, Double)(names.Length, StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To names.Length - 1
            out(names(i)) = v(i)
        Next

        Return out
    End Function

    ' ---------------- 数值安全 ----------------

    ''' <summary>
    ''' 对所有状态池做 NaN / Inf 检测与非负钳制
    ''' </summary>
    ''' <returns>是否发生了钳制（可用于日志告警）</returns>
    Public Function Sanitize() As Boolean
        Dim clipped As Boolean = False

        For Each pool As StatePool In New StatePool() {
            StatePool.mRNA, StatePool.Protein,
            StatePool.Metabolite, StatePool.Boundary, StatePool.Signal
        }
            Dim v As Double() = Vector(pool)

            For i As Integer = 0 To v.Length - 1
                Dim x As Double = v(i)

                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    v(i) = 0.0
                    clipped = True
                ElseIf x < 0.0 Then
                    v(i) = 0.0
                    clipped = True
                End If
            Next
        Next

        If Double.IsNaN(CyclePhase) OrElse Double.IsInfinity(CyclePhase) Then
            CyclePhase = 0.0
            clipped = True
        End If
        If Double.IsNaN(RecyclePool) OrElse Double.IsInfinity(RecyclePool) OrElse RecyclePool < 0.0 Then
            RecyclePool = 0.0
            clipped = True
        End If

        Return clipped
    End Function

    Public Overrides Function ToString() As String
        Return $"genes={NGene}, metabolites={NMetabolite}, boundary={NBoundary}, signals={NSignal}, phase={CyclePhase:F3}"
    End Function

End Class
