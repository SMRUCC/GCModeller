''' <summary>
''' The gene expression data samples file.(基因的表达数据样本)
''' </summary>
''' <remarks></remarks>
Public Class DataFrameRow

    Public Property geneID As String
    ''' <summary>
    ''' This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property experiments As Double()

    ''' <summary>
    ''' Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property samples As Integer
        Get
            If experiments Is Nothing Then
                Return 0
            Else
                Return experiments.Length
            End If
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0} -> {1}", geneID, String.Join(", ", experiments))
    End Function
End Class