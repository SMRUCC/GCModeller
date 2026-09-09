Imports System.Text.RegularExpressions

''' <summary>
''' 与数据源无关的反应契约——规则挖掘引擎（<see cref="RuleMiner"/>）的唯一输入。
''' </summary>
''' <remarks>
''' 之所以要这一层：不同数据源的"反应"对象差异很大（BioCyc 的 <c>reactions</c> 带
''' <c>REACTION-DIRECTION</c>，需要先按方向把左右翻转；GCModeller 内部的
''' <c>MetabolicReaction</c> 没有方向枚举、一律按 left→right 理解）。把这些差异全部
''' 收敛到各自的数据源映射层之后，挖掘引擎只看到"已归正的底物/产物 id 列表"，
''' 从而保证不同数据源挖出的规则语义完全一致。
'''
''' 各字段的取值约定：
''' <list type="bullet">
''' <item><see cref="ReactantIds"/> / <see cref="ProductIds"/>：必须已完成方向归正，
''' 且已去重、去空、按字典序排列（挖掘结果依赖确定性顺序）；</item>
''' <item><see cref="ECNumbers"/>：非空即视为"常见酶家族"，酶层级取
''' <see cref="EnzymeTiers.Common"/>；</item>
''' <item><see cref="IsSpontaneous"/>：无 EC 但可自发发生时取
''' <see cref="EnzymeTiers.General"/>；两者皆无则取
''' <see cref="EnzymeTiers.Specialized"/>。</item>
''' </list>
''' </remarks>
Public Class ReactionSpec

    ''' <summary>
    ''' 反应唯一标识。会直接作为规则的 <see cref="Rule.Id"/>，
    ''' 便于把搜索结果回溯到原始反应条目。
    ''' </summary>
    Public Property Id As String

    ''' <summary>
    ''' 反应名称（对应规则的 <see cref="Rule.Name"/>）。映射层应按
    ''' "常用名 → 系统名 → id" 的顺序回退取值。
    ''' </summary>
    Public Property Name As String

    ''' <summary>已按方向归正的底物 compound id 集合（去重、去空、字典序）。</summary>
    Public Property ReactantIds As List(Of String)

    ''' <summary>已按方向归正的产物 compound id 集合（去重、去空、字典序）。</summary>
    Public Property ProductIds As List(Of String)

    ''' <summary>EC 号集合；非空即判定为"常见酶家族"。</summary>
    Public Property ECNumbers As String()

    ''' <summary>是否无需酶催化即可发生。</summary>
    Public Property IsSpontaneous As Boolean

    ''' <summary>正向（底物 → 产物）的 ΔG，单位 kJ/mol。</summary>
    Public Property Gibbs As Double

    ''' <summary>生理条件下是否可逆。</summary>
    Public Property IsReversible As Boolean

    ''' <summary>
    ''' 便捷构造：从左右两侧的化合物 id 序列生成一条已排序、去重的反应契约。
    ''' </summary>
    ''' <param name="id">反应唯一标识。</param>
    ''' <param name="name">反应名称。</param>
    ''' <param name="reactants">底物侧的化合物引用（可为 Nothing）。</param>
    ''' <param name="products">产物侧的化合物引用（可为 Nothing）。</param>
    ''' <param name="ecNumbers">EC 号集合。</param>
    ''' <param name="isSpontaneous">是否自发。</param>
    ''' <param name="gibbs">正向 ΔG。</param>
    ''' <param name="isReversible">是否可逆。</param>
    ''' <returns>已归正并排序的 <see cref="ReactionSpec"/>。</returns>
    Public Shared Function Create(id As String, name As String,
                                  reactants As IEnumerable(Of String),
                                  products As IEnumerable(Of String),
                                  Optional ecNumbers As String() = Nothing,
                                  Optional isSpontaneous As Boolean = False,
                                  Optional gibbs As Double = 0,
                                  Optional isReversible As Boolean = False) As ReactionSpec
        Return New ReactionSpec With {
            .Id = id,
            .Name = name,
            .ReactantIds = RuleMiner.CompoundIds(reactants),
            .ProductIds = RuleMiner.CompoundIds(products),
            .ECNumbers = ecNumbers,
            .IsSpontaneous = isSpontaneous,
            .Gibbs = gibbs,
            .IsReversible = isReversible
        }
    End Function

    Public Overrides Function ToString() As String
        Return $"{Id}: {String.Join(" + ", ReactantIds)} => {String.Join(" + ", ProductIds)}"
    End Function

End Class
