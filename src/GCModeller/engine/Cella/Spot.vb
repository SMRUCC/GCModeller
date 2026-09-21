' ============================================================
' Spot.vb - 空间格点
' ============================================================
' 环境空间中的一个格点，持有：
'   * Medium —— 该格点的胞外培养基（代谢物 id => 浓度），细胞通过跨膜
'               转运系统从中摄取营养，摄取后浓度下降。
'   * cells  —— 位于该格点上的虚拟细胞。
' ============================================================

Imports Microsoft.VisualBasic.Imaging

Public Class Spot

    ''' <summary>
    ''' 该格点上的胞外培养基成分（代谢物 id => 浓度）
    ''' </summary>
    Public Property Medium As Dictionary(Of String, Double)

    ''' <summary>位于该格点上的虚拟细胞</summary>
    Public Property cells As New List(Of VirtualCella)

    ''' <summary>三维空间索引</summary>
    Public Property index As SpatialIndex3D

    ''' <summary>
    ''' 归一化径向位置：0 = 类器官球心，1 = 最外层；非球形空间为 -1。
    ''' 这是「空间极性 / 位置信号」的几何来源，驱动细胞命运的空间分带。
    ''' </summary>
    Public Property NormalizedRadius As Double = -1.0

    ''' <summary>是否处于类器官表层（直接接触培养基的格点）</summary>
    Public ReadOnly Property IsSurface As Boolean
        Get
            Return NormalizedRadius >= 0.72
        End Get
    End Property

    ''' <summary>pH</summary>
    Public Property ph As Double = 7.0

    ''' <summary>温度（摄氏度）</summary>
    Public Property temperature As Double = 37.0

    ''' <summary>
    ''' 推进一个时间步：先更新胞外培养基（补充 / 扩散由外部处理），
    ''' 再驱动格点上的每一个细胞
    ''' </summary>
    Public Sub Tick(dt As Double)
        For Each cella As VirtualCella In cells.ToArray()
            Call cella.Tick(dt)
        Next
    End Sub

    ''' <summary>该格点上是否还有存活的细胞</summary>
    Public ReadOnly Property HasCells As Boolean
        Get
            Return cells.Count > 0
        End Get
    End Property

    ''' <summary>该格点上的细胞数量</summary>
    Public ReadOnly Property CellCount As Integer
        Get
            Return cells.Count
        End Get
    End Property

    ''' <summary>
    ''' 该格点的营养指标：按蓝图指定的营养代谢物（未指定时取培养基全部成分）求和。
    ''' 既用于饥饿判定（生长底物耗尽的信号），也用于趋化性的梯度计算。
    ''' </summary>
    Public Function NutrientLevel(blueprint As CellaBlueprint) As Double
        If Medium Is Nothing OrElse Medium.Count = 0 Then
            Return 0.0
        End If

        Dim sum As Double = 0.0

        For Each item In Medium
            If blueprint Is Nothing OrElse blueprint.IsNutrient(item.Key) Then
                Dim v As Double = item.Value

                If v > 0 AndAlso Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    sum += v
                End If
            End If
        Next

        Return sum
    End Function

    ''' <summary>趋化性使用的配体池（与 <see cref="NutrientLevel"/> 同一套指标）</summary>
    Public Function LigandLevel(blueprint As CellaBlueprint) As Double
        Return NutrientLevel(blueprint)
    End Function

    ''' <summary>按物种统计该格点内的细胞数</summary>
    Public Function PopulationBySpecies() As Dictionary(Of String, Integer)
        Dim out As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each cella As VirtualCella In cells
            Dim key As String = If(cella.Species, "(unknown)")

            If out.ContainsKey(key) Then
                out(key) += 1
            Else
                out(key) = 1
            End If
        Next

        Return out
    End Function

    Public Overrides Function ToString() As String
        Return $"({index.X},{index.Y},{index.Z}) with {cells.Count} cells"
    End Function

End Class
