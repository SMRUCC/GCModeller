' ============================================================================
' Scoring.vb — 路径多维评分与正向组装 [readme.md §4]
' ----------------------------------------------------------------------------
' 热力学：ΔG_path = Σ 每步规则 ΔG（启发式基团贡献代理）→ thermo = σ(−ΔG/10)
' 酶可得性：enzyme = mean(1/tier)——Selenzyme/BridgIT 指纹相似度的文档化代理
' 路径长度：length = 1/nSteps
' 全局：w_thermo·thermo + w_enzyme·enzyme + w_length·length（默认 0.4/0.3/0.3）
' 组装：逆合成步骤反转+翻转方向 = 正向生物合成路径（底物=前体+共底物，产物=子化合物）
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 路径评分权重：热力学、酶可得性、长度三项的加权系数。
    ''' </summary>
    Public Class ScoreWeights

        ''' <summary>热力学可行性权重（默认 0.4）。</summary>
        Public Thermo As Double = 0.4
        ''' <summary>酶可得性权重（默认 0.3）。</summary>
        Public Enzyme As Double = 0.3
        ''' <summary>路径长度权重（默认 0.3）。</summary>
        Public Length As Double = 0.3

    End Class

    ''' <summary>
    ''' 一条完整路径的多维评分结果。
    ''' </summary>
    Public Class PathScores

        ''' <summary>路径各步 ΔG 之和（kJ/mol）。</summary>
        Public DeltaGTotal As Double
        ''' <summary>热力学可行性得分 = σ(−ΔG_total / 10)，取值 (0,1)，越大越可行。</summary>
        Public ThermoScore As Double
        ''' <summary>酶可得性得分 = mean(1/tier)，取值 (0,1]。</summary>
        Public EnzymeScore As Double
        ''' <summary>长度得分 = 1 / 步数，路径越短越高。</summary>
        Public LengthScore As Double
        ''' <summary>加权全局得分，用于对所有候选路径统一排序。</summary>
        Public GlobalScore As Double
        ''' <summary>路径包含的反应步数。</summary>
        Public NumSteps As Int32

    End Class

    ''' <summary>正向路径步骤（供 JSON 输出）</summary>
    Public Class ForwardStep

        ''' <summary>规则 ID。</summary>
        Public RuleId As String
        ''' <summary>规则名称。</summary>
        Public RuleName As String
        ''' <summary>
        ''' 规则定义方向的适用态
        ''' </summary>
        Public Orientation As String
        ''' <summary>该步的底物（前体 + 共底物），以 SMILES 表示。</summary>
        Public Substrates As New List(Of String)()      ' SMILES
        ''' <summary>该步的产物，以 SMILES 表示。</summary>
        Public Products As New List(Of String)()        ' SMILES
        ''' <summary>该步的 ΔG（kJ/mol）。</summary>
        Public DeltaG As Double
        ''' <summary>该步的酶可得性层级（1/2/3）。</summary>
        Public EnzymeTier As EnzymeTiers

    End Class

    ''' <summary>
    ''' 路径评分与正向组装：把逆合成步骤序列翻转为生物合成方向的步骤列表，
    ''' 并按热力学 / 酶可得性 / 路径长度给出全局分。
    ''' </summary>
    Public Module Scoring

        ''' <summary>
        ''' Logistic 函数（数值稳定版），用于把 ΔG 映射成 (0,1) 的热力学得分。
        ''' </summary>
        ''' <param name="x">自变量。</param>
        ''' <returns>1 / (1 + e^(−x))；|x| &gt; 35 时直接返回 0 或 1，避免溢出。</returns>
        Public Function Sigmoid(x As Double) As Double
            If x > 35 Then Return 1.0
            If x < -35 Then Return 0.0
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

        ''' <summary>
        ''' 对一条完整逆合成路径评分：汇总各步 ΔG，计算热力学 / 酶可得性 / 长度三项分与全局分。
        ''' </summary>
        ''' <param name="state">已完成（<see cref="SearchState.Pending"/> 为空）的搜索状态。</param>
        ''' <param name="w">三项指标的加权系数。</param>
        ''' <returns>该路径的评分明细。</returns>
        ''' <remarks>
        ''' 全局分 = w.Thermo × 热力学 + w.Enzyme × 酶可得性 + w.Length × 长度。
        ''' FBA 通量未纳入（需 GEM 模型，见 readme.md §5 已知边界）。
        ''' </remarks>
        Public Function ScorePath(state As SearchState, w As ScoreWeights) As PathScores
            Dim ps As New PathScores()
            ps.NumSteps = state.Steps.Count
            ps.DeltaGTotal = state.Steps.Sum(Function(s) s.DeltaG)
            ps.ThermoScore = Sigmoid(-ps.DeltaGTotal / 10.0)
            ps.EnzymeScore = If(ps.NumSteps > 0,
                                state.Steps.Average(Function(s) 1.0 / s.EnzymeTier), 1.0)
            ps.LengthScore = 1.0 / Math.Max(1, ps.NumSteps)
            ps.GlobalScore = w.Thermo * ps.ThermoScore + w.Enzyme * ps.EnzymeScore +
                             w.Length * ps.LengthScore
            Return ps
        End Function

        ''' <summary>
        ''' 正向组装：把逆合成步骤序列反转，每步翻转方向，得到"汇前体 → 目标"的生物合成顺序。
        ''' </summary>
        ''' <param name="state">已完成的搜索状态。</param>
        ''' <returns>正向步骤列表，每步含底物/产物 SMILES、ΔG 与酶层级。</returns>
        ''' <remarks>
        ''' 每步的 Substrates = 该逆合成步得到的前体（彼此为共底物关系，主前体在前），
        ''' Products = 被分解的化合物本身。
        ''' </remarks>
        Public Function AssembleForward(state As SearchState) As List(Of ForwardStep)
            Dim steps As New List(Of ForwardStep)()
            For Each rs In state.Steps.AsEnumerable().Reverse()
                Dim fs As New ForwardStep With {
                    .RuleId = rs.RuleId, .RuleName = rs.RuleName,
                    .Orientation = If(rs.Orientation = "applied", "reverse-of-retro", rs.Orientation),
                    .DeltaG = rs.DeltaG,
                    .EnzymeTier = rs.EnzymeTier}
                ' 正向底物 = 逆合成前体（各前体间为共底物关系，主前体在前）
                For Each p In rs.Precursors
                    fs.Substrates.Add(SmilesIO.Write(p.Item2))
                Next
                ' 正向产物 = 被分解化合物本身
                fs.Products.Add(SmilesIO.Write(rs.SubstrateMol))
                steps.Add(fs)
            Next
            Return steps
        End Function

    End Module

End Namespace
