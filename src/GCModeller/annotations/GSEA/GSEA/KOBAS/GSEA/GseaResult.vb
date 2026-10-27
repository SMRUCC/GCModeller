Namespace KOBAS

    ''' <summary>
    ''' 一个基因集的 GSEA 分析结果。
    ''' </summary>
    Public Class GseaResult

        ''' <summary>
        ''' 基因集编号（GMT 第一列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

        ''' <summary>
        ''' 基因集名称（GMT 第二列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Names As String

        ''' <summary>
        ''' 基因集描述。
        ''' </summary>
        ''' <returns></returns>
        Public Property Description As String

        ''' <summary>
        ''' 该基因集在排序列表之中的命中数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property Size As Integer

        ''' <summary>
        ''' 观测到的富集分数。
        ''' </summary>
        ''' <returns></returns>
        Public Property ES As Double

        ''' <summary>
        ''' 归一化富集分数；跳过置换检验时等于 <see cref="ES"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property NES As Double

        ''' <summary>
        ''' 名义 p 值；跳过置换检验时为 <see cref="Double.NaN"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double

        ''' <summary>
        ''' 错误发现率；跳过置换检验时为 <see cref="Double.NaN"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property FDR As Double

        ''' <summary>
        ''' 富集分数在 running sum 序列之中的位置。
        ''' </summary>
        ''' <returns></returns>
        Public Property ESIndex As Integer

        ''' <summary>
        ''' running sum 序列；当关闭 <see cref="GseaOptions.KeepRunningSum"/> 时为 <c>Nothing</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property RES As Double()

        ''' <summary>
        ''' 核心富集基因（峰前或峰后的命中基因）。
        ''' </summary>
        ''' <returns></returns>
        Public Property CoreGenes As String()

        ''' <summary>
        ''' 该基因集在排序列表之中的全部命中基因。
        ''' </summary>
        ''' <returns></returns>
        Public Property HitGenes As String()

        Public Overrides Function ToString() As String
            Return $"{ID} ES={ES.ToString("F4")} NES={NES.ToString("F4")} p={PValue.ToString("G4")}"
        End Function
    End Class
End Namespace
