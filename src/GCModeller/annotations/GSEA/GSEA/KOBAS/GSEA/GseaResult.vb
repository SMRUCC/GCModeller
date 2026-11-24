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

    ''' <summary>
    ''' GSEA 结果的整理工具。
    ''' </summary>
    Public Module GseaResultExtensions

        ''' <summary>
        ''' 按富集分数的符号把结果拆分为两个表型方向（KOBAS <c>output_set</c> 的等价实现）。
        ''' </summary>
        ''' <param name="results">GSEA 分析结果。</param>
        ''' <returns>
        ''' 元组：<c>Positive</c> 为表型 A（<c>phnameA</c>）一侧，按 ES 降序；
        ''' <c>Negative</c> 为表型 B（<c>phnameB</c>）一侧，按 ES 升序（最负者在前）。
        ''' </returns>
        Public Function SplitPhenotypes(results As GseaResult()) As (Positive As GseaResult(), Negative As GseaResult())
            Dim positive As GseaResult() = results _
                .Where(Function(item) item.ES > 0) _
                .OrderByDescending(Function(item) item.ES) _
                .ToArray
            Dim negative As GseaResult() = results _
                .Where(Function(item) item.ES < 0) _
                .OrderBy(Function(item) item.ES) _
                .ToArray

            Return (positive, negative)
        End Function
    End Module
End Namespace
