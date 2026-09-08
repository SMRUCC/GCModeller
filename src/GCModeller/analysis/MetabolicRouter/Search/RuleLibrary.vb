' ============================================================================
' RuleLibrary.vb — 内置广义反应规则库 + 用户 TSV 扩展
' ----------------------------------------------------------------------------
' [readme.md §2] 规则由酶催化化学逻辑抽象（反应中心 + 原子映射），不绑定具体
'   反应实例——同一规则可应用于任何包含该反应中心的底物。
' [readme.md §4] ΔG 为启发式基团贡献代理值（kJ/mol，正向）；酶可得性以层级
'   代理（1=常见 EC 家族 2=一般 3=特化）——Selenzyme/BridgIT 反应指纹相似度
'   需外部工具，此处为文档化简化。
' 用户规则 TSV：id <TAB> name <TAB> dG <TAB> tier <TAB> reversible <TAB> reactant <TAB> product
' ============================================================================

Imports System.Globalization
Imports System.IO
Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 反应规则库：内置广义规则 + 用户 TSV 扩展。
    ''' </summary>
    Public Module RuleLibrary

        ''' <summary>
        ''' 内置规则库：9 条核心酶促变换（氧化还原 / 转氨 / 醛缩 / 脱羧 / 水合 /
        ''' 磷酸化 / 酯水解 / Claisen / 互变异构）。
        ''' </summary>
        ''' <returns>内置 <see cref="Rule"/> 列表（每次调用返回新实例）。</returns>
        ''' <remarks>
        ''' 规则由酶催化的化学逻辑抽象而来，不绑定具体底物，因此可作用于任何含有相同
        ''' 反应中心的分子。ΔG 为启发式基团贡献代理值，酶层级为 1=常见 EC 家族 / 2=一般 / 3=特化。
        ''' </remarks>
        Public Function BuiltinRules() As List(Of Rule)
            Dim rules As New List(Of Rule)()
            ' 产物侧 [C!O:1]：排除带 -OH 邻居的碳（羧基 C），否则逆向还原会把 -COOH 变成偕二醇 -CH(OH)2
            rules.Add(New Rule("R001", "醇脱氢酶（氧化/还原）",
                "[C:1]-[OH1:2]", "[C!O:1]=[O:2]", 18.0, EnzymeTiers.Common))
            rules.Add(New Rule("R002", "转氨酶（酮↔胺）",
                "[CD3H0!O:1]=[O:2]", "[CD3H1!O:1]-[N:3].[O:2]", 10.0, EnzymeTiers.Common))
            ' 产物侧 [C!O:1]：逆向（aldol 加成）时 cls1 为亲电羰基碳，若命中羧基 C 会生成
            '   偕二醇 C(OH)2（价态合法、闸门不拦）；cls4 命中羧基则加成后超价，已被价态闸门自动拒绝。
            '   正向（裂解）匹配反应物侧，不受此约束影响。
            rules.Add(New Rule("R003", "醛缩酶（β-羟羰基裂解/aldol）",
                "[C:1](-[OH1:2])-[C:3]-[C:4]=[O:5]", "[C!O:1]=[O:2].[C:3]-[C:4]=[O:5]", -15.0, EnzymeTiers.Common))
            rules.Add(New Rule("R004", "脱羧酶（羧基离去 CO2）",
                "[C:1]-[C:2](=[O:3])-[OH1:4]", "[C:1].[O:3]=[C:2]=[O:4]", -28.0, EnzymeTiers.Common))
            ' 反应物侧 [C!O:1]：正向水合时 cls1 若已带 -OH（烯醇 C=C），加成后会变成偕二醇 C(OH)2
            ' 产物侧 [C!=O:1]：反应中心碳自带 -OH（故不能用 !O），需排除还带 =O 的羧基碳，
            '   否则逆向脱水会把羧基变成烯酮累积双键 C(=C)(=O)
            rules.Add(New Rule("R005", "水合酶（C=C 水合/脱水）",
                "[C!O:1]=[C:2]", "[C!=O:1](-[O:3])-[C:2]", -12.0, EnzymeTiers.Common))
            rules.Add(New Rule("R006", "激酶（羟基磷酸化）",
                "[OH1:1]", "[O:1]-[P:2](=[O:3])-[O-:4]", 25.0, EnzymeTiers.Common))
            rules.Add(New Rule("R007", "酯酶（酯水解/酯化）",
                "[C:1](=[O:2])-[O:3]", "[C:1](=[O:2])-[O:5].[O:3]", -20.0, EnzymeTiers.Common))
            rules.Add(New Rule("R008", "硫解酶（Claisen 裂解）",
                "[C:1](=[O:2])-[CH2:3]-[C:4](=[O:5])", "[C:1](=[O:2])-[S:6].[C:3]-[C:4]=[O:5]", -25.0, EnzymeTiers.General))
            ' 反应物侧 [C!O:2]：排除羧基碳，否则正向互变会把 -COOH 变成烯二醇 C=C(OH)2
            rules.Add(New Rule("R009", "酮-烯醇互变异构酶",
                "[C:1]-[C!O:2]=[O:3]", "[C:1]=[C:2]-[OH1:3]", -2.0, EnzymeTiers.General))
            Return rules
        End Function

        ''' <summary>
        ''' 货币/辅底物 SMILES：规则应用产生的这类碎片不计入前体，直接视为"底盘中天然存在"。
        ''' </summary>
        ''' <returns>SMILES 列表（默认含水、二氧化碳、氨、硫化氢、磷酸）。</returns>
        Public Function CurrencySmiles() As List(Of String)
            Return New List(Of String) From {"O", "O=C=O", "N", "S", "OP(=O)(O)O"}
        End Function

        ''' <summary>用户规则 TSV 加载（追加到内置库）</summary>
        Public Function LoadRulesTsv(path As String) As List(Of Rule)
            Dim rules As New List(Of Rule)()
            For Each raw In File.ReadLines(path)
                Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                If line.Length = 0 OrElse line.StartsWith("#"c) Then Continue For
                Dim cols = line.Split(ControlChars.Tab)
                If cols.Length < 7 Then Continue For
                Dim dg As Double = 0
                Double.TryParse(cols(2).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, dg)
                Dim tier As Int32 = 2
                Int32.TryParse(cols(3).Trim(), tier)
                Dim rev As Boolean = cols(4).Trim().ToLowerInvariant() = "true" OrElse cols(4).Trim() = "1"
                rules.Add(New Rule(cols(0).Trim(), cols(1).Trim(), cols(5).Trim(), cols(6).Trim(), dg, CType(tier, EnzymeTiers), rev))
            Next
            Return rules
        End Function

    End Module

End Namespace
