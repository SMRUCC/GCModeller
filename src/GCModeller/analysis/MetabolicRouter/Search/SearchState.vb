Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 搜索状态：一个"尚未落入汇集合的化合物集合"（RetroPath2.0 的化合物空间语义）。
    ''' </summary>
    ''' <remarks>
    ''' 每一步逆反应把状态中的某个化合物替换为它的若干前体；前体中已属于汇集合的被移除，
    ''' 其余进入 <see cref="Pending"/>。当 <see cref="Pending"/> 为空时即得到一条完整路径。
    ''' </remarks>
    Public Class SearchState

        ''' <summary>
        ''' 待分解的化合物（尚未落入底盘汇集合者），每项为 (分子指纹, 分子)。
        ''' </summary>
        Public Pending As New List(Of (key As String, mol As Molecule))()

        ''' <summary>
        ''' 从目标走到当前状态所经过的逆合成步骤（按发生顺序排列）。
        ''' </summary>
        Public Steps As New List(Of RetroStep)()

        ''' <summary>
        ''' 本分支已出现过的化合物指纹集合，用于循环消除：前体重复出现即剪枝。
        ''' </summary>
        Public Used As HashSet(Of String)

        ''' <summary>
        ''' 状态指纹：把待分解化合物的指纹排序后拼接，用于状态去重与束剪枝。
        ''' </summary>
        ''' <returns>确定性字符串；<see cref="Pending"/> 为空时返回空串。</returns>
        Public Function StateKey() As String
            Return Pending.Select(Function(t) t.Item1).OrderBy(Function(x) x, StringComparer.Ordinal).JoinBy("|")
        End Function

        ''' <summary>
        ''' 当前状态中所有待分解化合物的原子总数，用作束剪枝的次要排序键（偏好更小的中间体）。
        ''' </summary>
        ''' <returns>原子总数。</returns>
        Public Function TotalAtoms() As Int32
            Return Aggregate tup In Pending Let mol = tup.mol Into Sum(mol.NumAtoms())
        End Function

    End Class

    ''' <summary>
    ''' 一次搜索过程的运行统计（用于结果报告与性能诊断）。
    ''' </summary>
    Public Class SearchStats

        ''' <summary>尝试过的规则应用次数（正向与逆向各计一次）。</summary>
        Public ApplicationsTried As Int64 = 0
        ''' <summary>生成的状态总数（含被剪枝的）。</summary>
        Public StatesGenerated As Int64 = 0
        ''' <summary>至少产生过一次有效应用的规则次数。</summary>
        Public RulesApplied As Int64 = 0
        ''' <summary>
        ''' 被「元素多重集预过滤」直接跳过的规则应用次数：
        ''' 分子里根本不含模式所需的元素，匹配必然失败，无需尝试。
        ''' </summary>
        Public RulesPrefiltered As Int64 = 0
        ''' <summary>实际到达的最大搜索深度。</summary>
        Public MaxDepthReached As Int32 = 0
        ''' <summary>搜索耗时（毫秒），由调用方在搜索结束后回填。</summary>
        Public ElapsedMs As Int64 = 0

    End Class
End Namespace