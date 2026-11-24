Namespace KOBAS

    ''' <summary>
    ''' SNBEA（子网络/通路水平富集）分析的一行结果。
    ''' </summary>
    Public Class SnbeaResult

        ''' <summary>
        ''' 基因集编号。
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

        ''' <summary>
        ''' 基因集名称。
        ''' </summary>
        ''' <returns></returns>
        Public Property Names As String

        ''' <summary>
        ''' 基因集描述。
        ''' </summary>
        ''' <returns></returns>
        Public Property Description As String

        ''' <summary>
        ''' 参与打分并且出现在表达矩阵之中的基因数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property SetSize As Integer

        ''' <summary>
        ''' 检验统计量（PLAGE 为两样本 t 值，GAGE 为 <c>Σt / √n</c> 的 z 值）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Statistic As Double

        ''' <summary>
        ''' 原始 p 值。
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double

        ''' <summary>
        ''' 多重检验校正之后的 p 值。
        ''' </summary>
        ''' <returns></returns>
        Public Property AdjustedPValue As Double

        ''' <summary>
        ''' 该基因集在表达矩阵之中的成员基因。
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As String()

        Public Overrides Function ToString() As String
            Return $"{ID} n={SetSize} stat={Statistic.ToString("F3")} p={PValue.ToString("G4")}"
        End Function
    End Class
End Namespace
