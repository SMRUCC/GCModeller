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

    Public Class ScoreWeights

        Public Thermo As Double = 0.4
        Public Enzyme As Double = 0.3
        Public Length As Double = 0.3

    End Class

    Public Class PathScores

        Public DeltaGTotal As Double
        Public ThermoScore As Double
        Public EnzymeScore As Double
        Public LengthScore As Double
        Public GlobalScore As Double
        Public NumSteps As Int32

    End Class

    ''' <summary>正向路径步骤（供 JSON 输出）</summary>
    Public Class ForwardStep

        Public RuleId As String
        Public RuleName As String
        ''' <summary>
        ''' 规则定义方向的适用态
        ''' </summary>
        Public Orientation As String
        Public Substrates As New List(Of String)()      ' SMILES
        Public Products As New List(Of String)()        ' SMILES
        Public DeltaG As Double
        Public EnzymeTier As Int32

    End Class

    Public Module Scoring

        Public Function Sigmoid(x As Double) As Double
            If x > 35 Then Return 1.0
            If x < -35 Then Return 0.0
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

        ''' <summary>对一条完整逆合成路径评分</summary>
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
        ''' 正向组装：逆合成步骤序列反转，每步翻转规则方向——
        ''' 前体（+共底物）→ 被分解化合物 [readme.md §一 "正向组装成完整路径"]。
        ''' </summary>
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
