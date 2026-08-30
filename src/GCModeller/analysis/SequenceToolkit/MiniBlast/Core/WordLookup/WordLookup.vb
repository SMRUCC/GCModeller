' ============================================================================
' WordLookup.vb — 查询序列 word 查找表
' ----------------------------------------------------------------------------
' [README §一.2] Word 匹配：BLASTN 精确匹配；BLASTP 邻域词（得分 ≥ T）。
'
' NtWordLookup  — 连续 word 的 base-4 打包编码（W ≤ 28，Long 键）
' DcWordLookup  — dc-megablast 非连续模板种子（Ma, Xu & Altschul 2003），
'                 11/18 模板只对 care 位打包编码，don't-care 位容忍错配
' AaWordLookup  — 蛋白邻域词：对查询每个 word 递归枚举所有得分 ≥ T 的
'                 数据库 word（24 字母空间，按列最大得分上界剪枝）
' ============================================================================

Namespace Core

    ''' <summary>word 查找表统一接口（扫描器按此多态调用）</summary>
    Public Interface IWordLookup

        ReadOnly Property WordSize As Integer

        ReadOnly Property Span As Integer

        ''' <summary>从 pos 开始打包 word 键；无法作种子（含歧义等）返回 Long.MinValue</summary>
        Function PackAt(codes As Int32(), pos As Integer) As Long

        Function TryGetPositions(key As Long, ByRef positions As List(Of Integer)) As Boolean

    End Interface


End Namespace
