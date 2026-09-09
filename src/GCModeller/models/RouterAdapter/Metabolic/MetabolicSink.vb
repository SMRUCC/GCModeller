' ============================================================================
' MetabolicSink.vb — 内部代谢模型可移植的底盘汇白名单
' ----------------------------------------------------------------------------
' 与 BioCycSink 不同（那份白名单是 EcoCyc 的 frame id，换库即失效），内部代谢模型
' 可能来自 KEGG / ModelSEED / 任意第三方，id 命名约定完全不同。因此这里用
' "化合物名称 / 同义名" 的归一化匹配（见 SinkBuilder.Normalize：忽略大小写、连字符
' 与空格差异），同一份白名单可以跨库使用。
'
' 注意：小规模手写网络的反应数很少，"枢纽度 ≥ coreDegree" 可能选不出多少汇成员，
' 此时依赖本白名单兜底；必要时把 coreDegree 调低或直接用 SinkModes.All。
' ============================================================================

Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter.Data

Public Module MetabolicSink

    ''' <summary>
    ''' 中心代谢 / 底盘常见代谢物的名称白名单（归一化后按 id、name、synonym 匹配）。
    ''' 覆盖：糖酵解 / PPP / TCA、莽草酸途径、氨基酸、多胺、核苷酸辅因子、能量通货与无机小分子。
    ''' </summary>
    ''' <remarks>
    ''' 白名单按分区排序：糖酵解/PPP/糖 → TCA/乙醛酸 → 莽草酸途径 → 氨基酸 → 多胺 →
    ''' 核苷酸辅因子 → 无机小分子与常见共底物
    ''' </remarks>
    Public ReadOnly Property CentralMetaboliteNames As String() = {
        "glucose", "glucose 6-phosphate", "glucose-6-phosphate", "glucose 1-phosphate",
        "fructose 6-phosphate", "fructose 1,6-bisphosphate", "fructose-bisphosphate",
        "glyceraldehyde 3-phosphate", "d-glyceraldehyde 3-phosphate",
        "dihydroxyacetone phosphate", "glycerone phosphate",
        "3-phosphoglycerate", "3-phospho-d-glycerate", "2-phosphoglycerate", "2-phospho-d-glycerate",
        "phosphoenolpyruvate", "pyruvate", "pyruvic acid", "lactate", "acetate", "acetaldehyde", "ethanol",
        "glycerol", "1,3-bisphosphoglycerate",
        "ribulose 5-phosphate", "xylulose 5-phosphate", "ribose 5-phosphate", "ribose-phosphate",
        "sedoheptulose 7-phosphate", "erythrose 4-phosphate", "d-erythrose 4-phosphate",
        "6-phosphogluconate", "gluconate", "glucarate", "ribulose", "arabinose",
        "d-glucarate", "5-phospho-alpha-d-ribose 1-diphosphate", "prpp", "phosphoribosyl pyrophosphate",
        "citrate", "isocitrate", "2-oxoglutarate", "alpha-ketoglutarate", "2-ketoglutarate",
        "succinyl-coa", "succinate", "fumarate", "malate", "oxaloacetate", "oxaloacetic acid",
        "glyoxylate", "cis-aconitate", "l-malate",
        "shikimate", "shikimic acid", "shikimate 3-phosphate", "3-dehydroquinate",
        "3-dehydroshikimate", "5-enolpyruvylshikimate-3-phosphate", "epsp",
        "chorismate", "isochorismate", "anthranilate", "prephenate", "phenylpyruvate",
        "4-hydroxyphenylpyruvate", "2,3-dihydroxybenzoate",
        "glutamate", "l-glutamate", "glutamine", "l-glutamine", "aspartate", "l-aspartate",
        "alanine", "l-alanine", "serine", "l-serine", "glycine", "threonine", "l-threonine",
        "lysine", "l-lysine", "methionine", "l-methionine", "cysteine", "l-cysteine",
        "valine", "l-valine", "leucine", "l-leucine", "isoleucine", "l-isoleucine",
        "phenylalanine", "l-phenylalanine", "tyrosine", "l-tyrosine", "tryptophan", "l-tryptophan",
        "proline", "l-proline", "histidine", "l-histidine", "arginine", "l-arginine",
        "aspartate 4-semialdehyde", "homoserine", "l-homoserine", "ornithine", "l-ornithine",
        "citrulline", "aspartate-semialdehyde",
        "putrescine", "spermidine", "agmatine", "cadaverine", "1,4-diaminobutane",
        "atp", "adp", "amp", "gtp", "gdp", "gmp", "utp", "udp", "ump", "ctp", "cdp", "cmp",
        "pppi", "ppi", "diphosphate", "pyrophosphate",
        "nad", "nadh", "nadp", "nadph", "nadh(2)", "nadh2",
        "fad", "fadh2", "fmn", "fmnh2", "coenzyme a", "coa", "acetyl-coa", "coenzyme a(2-)",
        "s-adenosylmethionine", "s-adenosylhomocysteine", "s-adenosyl-l-methionine",
        "tetrahydrofolate", "thf", "acyl carrier protein", "acp",
        "glutathione", "oxidized glutathione", "l-glutathione",
        "flavin adenine dinucleotide", "thiamine diphosphate", "pyridoxal phosphate",
        "pyridoxal 5-phosphate", "plp",
        "water", "h2o", "carbon dioxide", "co2", "proton", "h+", "hydrogen", "ammonia",
        "nh3", "ammonium", "nh4+", "oxygen", "o2", "hydrogen peroxide", "h2o2",
        "phosphate", "orthophosphate", "phosphoric acid", "sulfate", "sulfide", "nitrate",
        "bicarbonate", "formate", "methanol", "formaldehyde", "hydrogen sulfide"
    }

    ''' <summary>
    ''' 构建内部代谢模型对应的汇集合（委托给通用 <see cref="SinkBuilder"/>，
    ''' 白名单使用跨数据库可移植的名称匹配）。
    ''' </summary>
    ''' <param name="structures">compound id → 已净化结构</param>
    ''' <param name="degrees">compound id → 在反应中出现的总次数</param>
    ''' <param name="mode">Core / All</param>
    ''' <param name="coreDegree">Core 模式下判定"枢纽代谢物"的最少反应出现次数；
    ''' 小规模手写网络建议取 2，真实库取 4。</param>
    ''' <returns>汇集合，每项为 (compound id, SMILES)。</returns>
    Public Function Build(structures As Dictionary(Of String, CompoundStructure),
                          degrees As Dictionary(Of String, Integer),
                          mode As SinkModes,
                          Optional coreDegree As Integer = 4) As List(Of (String, smiles As String))

        Return SinkBuilder.Build(structures, degrees, mode, coreDegree,
                                 nameWhitelist:=CentralMetaboliteNames)
    End Function

End Module
