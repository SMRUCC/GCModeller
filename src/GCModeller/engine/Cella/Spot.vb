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

    Public Overrides Function ToString() As String
        Return $"({index.X},{index.Y},{index.Z}) with {cells.Count} cells"
    End Function

End Class
