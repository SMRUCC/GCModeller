' Hamming 距离过滤接口 (SIMD 后续替换边界)
'
' DIAMOND 在种子命中点周围 48 个氨基酸窗口内计算查询与参考的 Hamming 距离,
' 作为第一级廉价初筛,将命中数量削减 1-2 个数量级。
'
' 本接口封装了"判断一对命中是否通过 Hamming 初筛"的边界,
' 当前提供标量实现(<see cref="HammingFilter"/>),后续可替换为
' System.Runtime.Intrinsics.X86 (SSE2: pcmpeqb / pmovmskb / popcnt) 向量化实现,
' 而调用方(DiamondBlastp 流水线)无需改动。

Namespace DIAMOND

    ''' <summary>
    ''' 在种子命中周围窗口计算 Hamming 距离并判定是否通过的过滤器。
    ''' </summary>
    Public Interface IHammingFilter

        ''' <summary>
        ''' 判断查询序列在 <paramref name="qPos"/> 与参考序列在 <paramref name="sPos"/>
        ''' 的种子命中是否通过 Hamming 距离初筛。
        ''' </summary>
        ''' <param name="query">完整查询序列(原始氨基酸字符)。</param>
        ''' <param name="qPos">种子在查询中的起始位置。</param>
        ''' <param name="subject">完整参考序列(原始氨基酸字符)。</param>
        ''' <param name="sPos">种子在参考中的起始位置。</param>
        ''' <returns>通过初筛返回 True。</returns>
        Function Pass(query As String, qPos As Integer, subject As String, sPos As Integer) As Boolean

        ''' <summary>
        ''' 计算并返回该命中窗口的实际 Hamming 距离(供排序/诊断使用)。
        ''' </summary>
        Function Distance(query As String, qPos As Integer, subject As String, sPos As Integer) As Integer
    End Interface
End Namespace
