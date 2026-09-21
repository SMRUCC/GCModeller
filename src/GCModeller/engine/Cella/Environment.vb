' ============================================================
' Environment.vb - 虚拟细胞生长的空间环境
' ============================================================
' Space 是一个三维交错数组，不同形状（培养皿 / 圆柱发酵罐 / 锥形摇瓶 /
' 长方体发酵池）由 SpaceInitializer 生成，形状之外的格点为 Nothing。
'
' 环境持有一个时钟：Tick() 推进一个 TimeStep，并把时间步传递给每个格点。
' 本版本只做静态空间支撑（不做细胞生长与分裂），细胞的位置固定。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Class Environment

    ''' <summary>三维空间格点（形状之外的位置为 Nothing）</summary>
    Public Property Space As Spot()()()

    ''' <summary>默认时间步长</summary>
    Public Property TimeStep As Double = 1.0

    ''' <summary>当前仿真时间</summary>
    Public ReadOnly Property CurrentTime As Double
        Get
            Return clock
        End Get
    End Property

    ''' <summary>已经推进的步数</summary>
    Public ReadOnly Property Iteration As Integer
        Get
            Return steps
        End Get
    End Property

    Private clock As Double = 0.0
    Private steps As Integer = 0

    ''' <summary>推进一个默认时间步</summary>
    Public Sub Tick()
        Call Tick(TimeStep)
    End Sub

    ''' <summary>推进 dt 个时间单位</summary>
    Public Sub Tick(dt As Double)
        For Each row As Spot()() In Space.SafeQuery
            For Each col As Spot() In row.SafeQuery
                For Each spot As Spot In col.SafeQuery
                    If spot Is Nothing Then
                        Continue For
                    End If

                    Call spot.Tick(dt)
                Next
            Next
        Next

        clock += dt
        steps += 1
    End Sub

    ''' <summary>连续推进若干步（每步一个 TimeStep）</summary>
    Public Sub Run(steps As Integer)
        For i As Integer = 1 To steps
            Call Tick(TimeStep)
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAllCells() As IEnumerable(Of VirtualCella)
        Return GetAllSpots().SelectMany(Function(s) s.cells)
    End Function

    ''' <summary>枚举所有有效（非 Nothing）格点</summary>
    Public Function GetAllSpots() As IEnumerable(Of Spot)
        Dim list As New List(Of Spot)

        For Each row As Spot()() In Space.SafeQuery
            For Each col As Spot() In row.SafeQuery
                For Each spot As Spot In col.SafeQuery
                    If spot IsNot Nothing Then
                        list.Add(spot)
                    End If
                Next
            Next
        Next

        Return list
    End Function

    ''' <summary>有效格点数量（即当前形状的体积）</summary>
    Public ReadOnly Property Volume As Integer
        Get
            Return GetAllSpots().Count
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"space[{Volume} spots] t={clock:F2} iter={steps}"
    End Function

End Class
