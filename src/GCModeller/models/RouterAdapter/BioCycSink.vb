' ============================================================================
' BioCycSink.vb — 底盘内源代谢物集合（汇）构建
' ----------------------------------------------------------------------------
' [readme_alg.md §一] 汇 = 底盘菌株（如 E. coli）内源代谢物集合，通常从 GEM 模型提取。
' 逆向搜索的终止条件就是"全部前体落入汇集合"。
'
' 两种模式：
'   Core（默认）：枢纽代谢物（在反应中出现的次数 ≥ coreDegree）+ 中心代谢白名单
'     + 辅因子。次级代谢途径的专有中间体不会被收录，因此搜索必须一路反推到中心
'     代谢分支点（如 chorismate），得到的是真正的多步生物合成通路。
'   All：全部有可用结构的化合物都算内源。此时目标自身若也在库内，BeamSearch 会
'     直接判定"目标已属于汇"而返回 0 条路径，因此查询时需要排除目标自身。
'
' 辅因子（NAD/ATP/CoA/ACP/SAM…）在两种模式下都进汇：它们确实内源存在，逆向生成时
' 即刻终止分支，等价于货币分子语义——避免把 ATP/CoA 当成"待合成前体"导致搜索爆炸。
' ============================================================================

Public Module BioCycSink

    Public Enum SinkModes
        ''' <summary>核心中心代谢子集（枢纽代谢物 + 中心代谢白名单 + 辅因子）</summary>
        Core = 0
        ''' <summary>全部有可用结构的化合物（查询时排除目标自身）</summary>
        All = 1
    End Enum

    ''' <summary>
    ''' 中心代谢白名单：糖酵解/PPP/TCA/氨基酸/核苷酸前体/关键分支点。
    ''' 这里的 id 均取自 EcoCyc 29.0 的 compounds.dat（UNIQUE-ID）；
    ''' 换用其它 PGDB 时不存在的 id 会被自动忽略，不会报错。
    ''' </summary>
    Private ReadOnly coreWhitelist As New HashSet(Of String)(
        New String() {
            "PYRUVATE", "PHOSPHO-ENOL-PYRUVATE", "ACETYL-COA", "SUC-COA", "CO-A", "ACET", "ACETALD", "ETOH",
            "FORMATE", "2-OXOBUTANOATE", "2-KETOGLUTARATE", "OXALACETIC_ACID", "CIT", "Isocitrate", "SUC", "FUM",
            "MAL", "GLYOX", "GLUCONATE", "D-GLUCARATE", "2-PG", "G3P", "GAP", "DIHYDROXY-ACETONE-PHOSPHATE",
            "ERYTHROSE-4P", "RIBULOSE-5P", "XYLULOSE-5-PHOSPHATE", "CPD-15318", "CPD-24813", "D-Glucose",
            "PRPP", "CHORISMATE", "SHIKIMATE", "HOMO-SER",
            "GLT", "GLN", "L-ASPARTATE", "L-ALPHA-ALANINE", "SER", "GLY", "THR", "LYS", "MET", "CYS",
            "VAL", "LEU", "ILE", "PHE", "TYR", "TRP", "PRO", "HIS", "ARG",
            "ATP", "ADP", "AMP", "GTP", "GDP", "GMP", "UTP", "UDP", "CTP", "CDP", "PPI", "PI",
            "NAD", "NADH", "NADP", "NADPH", "FAD", "FADH2", "FMN",
            "S-ADENOSYLMETHIONINE", "S-ADENOSYL-L-HOMOCYSTEINE", "THF", "ACP",
            "GLUTATHIONE", "OXIDIZED-GLUTATHIONE", "WATER", "CARBON-DIOXIDE", "PROTON", "AMMONIA", "HYDROGEN-PEROXIDE"
        }, StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' 构建汇集合。
    ''' </summary>
    ''' <param name="structures">compound id → 已净化结构</param>
    ''' <param name="degrees">compound id → 在反应中出现的总次数</param>
    ''' <param name="mode">Core / All</param>
    ''' <param name="coreDegree">Core 模式下判定"枢纽代谢物"的最少反应出现次数</param>
    Public Function Build(structures As Dictionary(Of String, CompoundStructure),
                          degrees As Dictionary(Of String, Integer),
                          mode As SinkModes,
                          Optional coreDegree As Integer = 4) As List(Of (String, smiles As String))

        Dim sink As New List(Of (String, smiles As String))()
        Dim ids As New List(Of String)(structures.Keys)

        ids.Sort(StringComparer.Ordinal)

        For Each id As String In ids
            Dim st As CompoundStructure = structures(id)

            If mode = SinkModes.All Then
                sink.Add((id, st.Smiles))
                Continue For
            End If

            Dim deg As Integer = 0
            degrees.TryGetValue(id, deg)

            If deg >= coreDegree OrElse coreWhitelist.Contains(id) Then
                sink.Add((id, st.Smiles))
            End If
        Next

        Return sink
    End Function

End Module
