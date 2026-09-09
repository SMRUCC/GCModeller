' ============================================================================
' BioCycSink.vb — BioCyc 特有的底盘汇白名单
' ----------------------------------------------------------------------------
' 通用的汇构建逻辑在 Data/SinkBuilder.vb，本文件只提供 BioCyc 特有的那一半：
' 一份以 EcoCyc frame id 为准的中心代谢白名单（换用其它 PGDB 时不存在的 id 会被
' 自动忽略，不会报错）。
' ============================================================================

Public Module BioCycSink

    ''' <summary>
    ''' 中心代谢白名单：糖酵解/PPP/TCA/氨基酸/核苷酸前体/关键分支点/辅因子。
    ''' 这里的 id 均取自 EcoCyc 29.0 的 compounds.dat（UNIQUE-ID）。
    ''' </summary>
    Public ReadOnly Property CoreWhitelist As String() = {
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
    }

    ''' <summary>
    ''' 构建 BioCyc 库对应的汇集合（委托给通用 <see cref="SinkBuilder"/>，
    ''' 白名单使用 EcoCyc 的 frame id）。
    ''' </summary>
    ''' <param name="structures">compound id → 已净化结构</param>
    ''' <param name="degrees">compound id → 在反应中出现的总次数</param>
    ''' <param name="mode">Core / All</param>
    ''' <param name="coreDegree">Core 模式下判定"枢纽代谢物"的最少反应出现次数</param>
    ''' <returns>汇集合，每项为 (compound id, SMILES)。</returns>
    Public Function Build(structures As Dictionary(Of String, CompoundStructure),
                          degrees As Dictionary(Of String, Integer),
                          mode As SinkModes,
                          Optional coreDegree As Integer = 4) As List(Of (String, smiles As String))

        Return SinkBuilder.Build(structures, degrees, mode, coreDegree,
                                 idWhitelist:=CoreWhitelist)
    End Function

End Module
