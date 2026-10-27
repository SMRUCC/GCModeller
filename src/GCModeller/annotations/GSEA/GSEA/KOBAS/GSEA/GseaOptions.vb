Namespace KOBAS

    ''' <summary>
    ''' 基因排序方法，对应 KOBAS <c>-m/--method</c> 参数。
    ''' </summary>
    Public Enum GeneRankingMethod
        ''' <summary>
        ''' Signal-to-noise ratio：<c>(mean0 - mean1) / (sd0 + sd1)</c>。
        ''' </summary>
        Snr
        ''' <summary>
        ''' 两样本 t 统计量：<c>(mean0 - mean1) / sqrt(sd0²/n0 + sd1²/n1)</c>。
        ''' </summary>
        TTest
    End Enum

    ''' <summary>
    ''' KOBAS GSEA 的运行配置。
    ''' </summary>
    ''' <remarks>
    ''' 对应 KOBAS 3.0 <c>scripts/run_gsea.py</c> 的命令行参数：
    ''' <c>-m/--method</c>、<c>-w/--weighted_score_type</c>、<c>-n/--npermutation</c>、
    ''' <c>-u/--min_size</c>、<c>-l/--max_size</c>。
    ''' </remarks>
    Public Class GseaOptions

        ''' <summary>
        ''' 基因排序方法，默认 <see cref="GeneRankingMethod.Snr"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property Method As GeneRankingMethod = GeneRankingMethod.Snr

        ''' <summary>
        ''' running sum 的权重指数（KOBAS 的 <c>weighted_score_type</c>）：
        ''' <c>0</c> 表示不加权（Kolmogorov-Smirnov），<c>1</c> 表示加权，
        ''' <c>2</c> 表示过度加权，也可以取 0~2 之间的任意正数。默认 <c>1</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property Weight As Double = 1

        ''' <summary>
        ''' 表型标签置换的次数（<c>0</c> 表示跳过显著性评估）。默认 1000。
        ''' </summary>
        ''' <returns></returns>
        Public Property Permutations As Integer = 1000

        ''' <summary>
        ''' 基因集命中数下限（闭区间）。默认 15。
        ''' </summary>
        ''' <returns></returns>
        Public Property MinSize As Integer = 15

        ''' <summary>
        ''' 基因集命中数上限（闭区间）。默认 500。
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxSize As Integer = 500

        ''' <summary>
        ''' 置换打乱所使用的随机种子，保证多次运行结果一致。默认 20261026。
        ''' </summary>
        ''' <returns></returns>
        Public Property Seed As Integer = 20261026

        ''' <summary>
        ''' 是否把每个基因集的 running sum 序列保留在结果之中。
        ''' 真实规模数据下该数组较大，可以关闭以节省内存。默认开启。
        ''' </summary>
        ''' <returns></returns>
        Public Property KeepRunningSum As Boolean = True

        ''' <summary>
        ''' 是否输出过程日志。默认开启。
        ''' </summary>
        ''' <returns></returns>
        Public Property Verbose As Boolean = True
    End Class
End Namespace
