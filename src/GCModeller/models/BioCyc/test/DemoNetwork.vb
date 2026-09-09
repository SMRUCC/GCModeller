' ============================================================================
' DemoNetwork.vb — 知识库手写的自包含 E. coli 代谢网络（MetabolicCompound / MetabolicReaction）
' ----------------------------------------------------------------------------
' 用途：为 MetabolicAdapter 提供演示数据——不依赖任何外部数据库文件，直接以 GCModeller
' 内部标准模型对象构建一套小型大肠杆菌代谢网络。
'
' 覆盖的代谢图：
'   · 糖酵解：glucose-6P → fructose-6P → fructose-1,6-bisP → GAP/DHAP → PEP（含 PGK/PGM/enolase）
'   · 莽草酸途径：PEP + E4P → DAHP → DHQ → shikimate → shikimate-3P → EPSP → chorismate
'   · 肠杆菌素（铁载体，次级代谢）：chorismate → isochorismate → DHB → DHB-Ser → enterobactin
'   · 色氨酸酶：L-tryptophan → indole + pyruvate + NH4
'   · 多胺：L-arginine → agmatine → putrescine；L-ornithine → putrescine
'   · 海藻糖：glucose-6P ×2 → trehalose-6P → trehalose + Pi
'
' 结构说明（如实的已知边界）：
'   · SMILES 全部写在 RetroPath 支持子集内（Kekulé 大写式，允许 @ 与 / \，会被净化器剔除）；
'   · 化学计量数与辅因子细节做了演示级简化（如 3 × DHB-Ser → enterobactin 只写 1 份底物、
'     GAPDH 的 NADH 氧化还原写全但 PGK 直接产 ATP 等），仅用于演示算法流程；
'   · 个别分子（DHQ、isochorismate、trehalose-6P 等）使用简化写法，拓扑正确性优先于精确结构。
' ============================================================================

Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Public Module DemoNetwork

    ''' <summary>构建演示网络的化合物集合（SMILES 均为知识库手写，经净化后可解析）。</summary>
    Public Function GetCompounds() As List(Of MetabolicCompound)
        Dim list As New List(Of MetabolicCompound)

        ' ----- 糖酵解 -----
        C(list, "GLC6P", "D-glucose 6-phosphate", "OC[C@H]1O[C@@H](O)[C@H](O)[C@@H](O)[C@@H]1OP(=O)(O)O")
        C(list, "F6P", "D-fructose 6-phosphate", "OC[C@H]1O[C@H](O)[C@H](OP(=O)(O)O)[C@@H](O)[C@@H]1O")
        C(list, "F16BP", "D-fructose 1,6-bisphosphate", "OC[C@H]1O[C@@H](O)[C@H](OP(=O)(O)O)[C@@H](O)[C@H]1OP(=O)(O)O")
        C(list, "GAP", "D-glyceraldehyde 3-phosphate", "C(=O)[C@H](O)COP(=O)(O)O")
        C(list, "DHAP", "dihydroxyacetone phosphate", "C(C(=O)COP(=O)(O)O)O")
        C(list, "13BPG", "1,3-bisphospho-D-glycerate", "OC(=O)COP(=O)(O)OC(=O)O")
        C(list, "3PG", "3-phospho-D-glycerate", "OC(=O)C(O)COP(=O)(O)O")
        C(list, "2PG", "2-phospho-D-glycerate", "OC(=O)C(OP(=O)(O)O)CO")
        C(list, "PEP", "phosphoenolpyruvate", "C=C(OP(=O)(O)O)C(=O)O")
        C(list, "PYRUVATE", "pyruvate", "CC(=O)C(=O)O")
        C(list, "LACTATE", "lactate", "CC(O)C(=O)O")

        ' ----- 莽草酸途径 -----
        C(list, "E4P", "D-erythrose 4-phosphate", "O=CC(O)C(O)COP(=O)(O)O")
        C(list, "DAHP", "3-dehydro-D-arabino-heptulosonate 7-phosphate", "O=CC(O)C(O)C(=O)COP(=O)(O)O")
        C(list, "DHQ", "3-dehydroquinate", "OC(=O)C1C(O)CC(=O)C(O)C1")
        C(list, "SHIKIMATE", "shikimate", "C1([CH](O)[CH](O)[CH](CC(C(=O)[O-])=1)O)")
        C(list, "SHIKIMATE3P", "shikimate 3-phosphate", "C1([CH](O)[CH](OP(=O)(O)O)[CH](CC(C(=O)[O-])=1)O)")
        C(list, "EPSP", "5-enolpyruvylshikimate 3-phosphate", "C=C(C(=O)[O-])O[CH]1(CC(C(=O)[O-])=C[CH](OP(=O)([O-])[O-])[CH](O)1)")
        C(list, "CHORISMATE", "chorismate", "C=C(C(=O)[O-])O[CH]1([CH](O)C=CC(C([O-])=O)=C1)")
        C(list, "ISOCHORISMATE", "isochorismate", "C=C(C(=O)[O-])O[CH]1([CH](O)C=CC([O-])=C1C(=O)[O-])")
        C(list, "DHB", "2,3-dihydroxybenzoate", "OC(=O)C1=CC=C(O)C(O)=C1")

        ' ----- 肠杆菌素（铁载体）-----
        C(list, "DHB-SER", "N-(2,3-dihydroxybenzoyl)-L-serine", "OC(=O)C(NC(=O)C1=CC=C(O)C(O)=C1)CO")
        C(list, "ENTEROBACTIN", "enterobactin",
          "C2([CH](NC(=O)C1(C(O)=C(C=CC=1)O))C(OC[CH](C(OC[CH](C(=O)O2)NC(=O)C3(C(O)=C(C=CC=3)O))=O)NC(=O)C4(C=CC=C(C(O)=4)O))=O)")

        ' ----- 色氨酸 / 吲哚 -----
        C(list, "L-TRYPTOPHAN", "L-tryptophan", "N[CH](CC1=CNC2=C1C=CC=C2)C(=O)O")
        C(list, "INDOLE", "indole", "C1(C=CC2(NC=CC(C=1)=2))")

        ' ----- 多胺 -----
        C(list, "L-ARGININE", "L-arginine", "NC(=N)NCCC(N)C(=O)O")
        C(list, "AGMATINE", "agmatine", "NC(=N)NCCCCN")
        C(list, "L-ORNITHINE", "L-ornithine", "NCCCC(N)C(=O)O")
        C(list, "PUTRESCINE", "putrescine", "C([NH3+])CCC[NH3+]")

        ' ----- 海藻糖 -----
        C(list, "TREHALOSE6P", "alpha,alpha-trehalose 6-phosphate",
          "C([CH]1(O[CH]([CH]([CH]([CH]1O)O)O)O[CH]2([CH]([CH]([CH]([CH](O2)COP(=O)(O)O)O)O)O)))O")
        C(list, "TREHALOSE", "alpha,alpha-trehalose",
          "C([CH]1(O[CH]([CH]([CH]([CH]1O)O)O)O[CH]2([CH]([CH]([CH]([CH](O2)CO)O)O)O)))O")

        ' ----- 辅因子 / 能量通货 / 无机小分子 -----
        C(list, "ATP", "ATP", "C(OP(=O)([O-])OP(=O)([O-])OP(=O)([O-])[O-])[CH]3(O[CH](N1(C2(C(N=C1)=C(N)N=CN=2)))[CH](O)[CH](O)3)")
        C(list, "ADP", "ADP", "C(OP(=O)([O-])OP(=O)([O-])[O-])[CH]3(O[CH](N1(C2(C(N=C1)=C(N)N=CN=2)))[CH](O)[CH](O)3)")
        C(list, "AMP", "AMP", "C(OP(=O)([O-])[O-])[CH]3(O[CH](N1(C2(C(N=C1)=C(N)N=CN=2)))[CH](O)[CH](O)3)")
        C(list, "NAD", "NAD+",
          "C5(C(C(N)=O)=CC=C([N+]([CH]4(O[CH](COP(OP(OC[CH]3(O[CH](N1(C2(C(N=C1)=C(N)N=CN=2)))[CH](O)[CH](O)3))(=O)[O-])(=O)[O-])[CH](O)[CH](O)4))=5)")
        C(list, "NADH", "NADH",
          "C5(C(C(N)=O)=CC=C(N([CH]4(O[CH](COP(OP(OC[CH]3(O[CH](N1(C2(C(N=C1)=C(N)N=CN=2)))[CH](O)[CH](O)3))(=O)[O-])(=O)[O-])[CH](O)[CH](O)4))=5)")
        C(list, "L-SERINE", "L-serine", "OC(C(N)C(=O)O)CO")
        C(list, "GLYCINE", "glycine", "NCC(=O)O")
        C(list, "L-ASPARTATE", "L-aspartate", "OC(=O)CC(N)C(=O)O")
        C(list, "L-GLUTAMATE", "L-glutamate", "OC(=O)CCC(N)C(=O)O")
        C(list, "PI", "phosphate", "[O-]P(=O)(O)[O-]")
        C(list, "PPI", "pyrophosphate", "[O-]P(=O)([O-])OP(=O)([O-])[O-]")
        C(list, "WATER", "water", "O")
        C(list, "CO2", "carbon dioxide", "O=C=O")
        C(list, "NH4", "ammonium", "[NH4+]")

        Return list
    End Function

    ''' <summary>构建演示网络的反应集合（id / 名称 / EC / 可逆 / 左右两侧）。</summary>
    Public Function GetReactions() As List(Of MetabolicReaction)
        Dim list As New List(Of MetabolicReaction)

        ' ----- 糖酵解 -----
        R(list, "PGI", "glucose-6-phosphate isomerase", {"5.3.1.9"}, False,
          {"GLC6P"}, {"F6P"})
        R(list, "PFK", "phosphofructokinase", {"2.7.1.11"}, False,
          {"F6P", "ATP"}, {"F16BP", "ADP"})
        R(list, "FBA", "fructose-bisphosphate aldolase", {"4.1.2.13"}, True,
          {"F16BP"}, {"GAP", "DHAP"})
        R(list, "TPI", "triose-phosphate isomerase", {"5.3.1.1"}, True,
          {"DHAP"}, {"GAP"})
        R(list, "GAPDH", "glyceraldehyde-3-phosphate dehydrogenase", {"1.2.1.12"}, True,
          {"GAP", "PI", "NAD"}, {"13BPG", "NADH"})
        R(list, "PGK", "phosphoglycerate kinase", {"2.7.2.3"}, True,
          {"13BPG", "ADP"}, {"3PG", "ATP"})
        R(list, "PGM", "phosphoglycerate mutase", {"5.4.2.11"}, False,
          {"3PG"}, {"2PG"})
        R(list, "ENO", "enolase", {"4.2.1.11"}, False,
          {"2PG"}, {"PEP", "WATER"})
        R(list, "PYK", "pyruvate kinase", {"2.7.1.40"}, False,
          {"PEP", "ADP"}, {"PYRUVATE", "ATP"})

        ' ----- 莽草酸途径 -----
        R(list, "AROG", "DAHP synthase", {"2.5.1.54"}, False,
          {"PEP", "E4P"}, {"DAHP", "PI"})
        R(list, "AROB", "3-dehydroquinate synthase", {"4.2.3.4"}, False,
          {"DAHP"}, {"DHQ"})
        R(list, "AROD-E", "dehydroquinate dehydratase + shikimate dehydrogenase", {"4.2.1.10", "1.1.1.25"}, False,
          {"DHQ", "NAD"}, {"SHIKIMATE", "WATER", "NADH"})
        R(list, "AROK", "shikimate kinase", {"2.7.1.71"}, False,
          {"SHIKIMATE", "ATP"}, {"SHIKIMATE3P", "ADP"})
        R(list, "AROA", "EPSP synthase", {"2.5.1.19"}, False,
          {"SHIKIMATE3P", "PEP"}, {"EPSP", "PI"})
        R(list, "AROC", "chorismate synthase", {"4.2.3.5"}, False,
          {"EPSP"}, {"CHORISMATE", "PI"})

        ' ----- 肠杆菌素（次级代谢：铁载体）-----
        R(list, "ENTC", "isochorismate synthase", {"5.4.4.2"}, False,
          {"CHORISMATE"}, {"ISOCHORISMATE"})
        R(list, "ENTB", "isochorismatase", {"3.3.1.-"}, False,
          {"ISOCHORISMATE", "WATER"}, {"DHB", "PYRUVATE"})
        R(list, "ENTE", "enterobactin synthase component E", {"6.3.2.14"}, False,
          {"DHB", "L-SERINE", "ATP"}, {"DHB-SER", "AMP", "PPI"})
        R(list, "ENTF", "enterobactin synthase component F", {"6.3.2.14"}, False,
          {"DHB-SER"}, {"ENTEROBACTIN", "WATER"})

        ' ----- 色氨酸酶（次级代谢：信号分子）-----
        R(list, "TNAA", "tryptophanase", {"4.1.99.1"}, False,
          {"L-TRYPTOPHAN", "WATER"}, {"INDOLE", "PYRUVATE", "NH4"})

        ' ----- 多胺（次级代谢）-----
        R(list, "SPEA", "arginine decarboxylase", {"4.1.1.19"}, False,
          {"L-ARGININE"}, {"AGMATINE", "CO2"})
        R(list, "SPEB", "agmatinase", {"3.5.3.12"}, False,
          {"AGMATINE", "WATER"}, {"PUTRESCINE", "NH4"})
        R(list, "SPECL", "ornithine decarboxylase", {"4.1.1.17"}, False,
          {"L-ORNITHINE"}, {"PUTRESCINE", "CO2"})

        ' ----- 海藻糖（渗透保护）-----
        R(list, "OTSA", "trehalose-6-phosphate synthase", {"2.4.1.15"}, False,
          {"GLC6P", "GLC6P"}, {"TREHALOSE6P", "PI"})
        R(list, "OTSB", "trehalose-6-phosphate phosphatase", {"3.1.3.12"}, False,
          {"TREHALOSE6P"}, {"TREHALOSE", "PI"})

        Return list
    End Function

    ''' <summary>构建一个 MetabolicCompound。</summary>
    Private Sub C(list As List(Of MetabolicCompound), id As String, name As String, smiles As String)
        list.Add(New MetabolicCompound With {
            .id = id,
            .name = name,
            .synonym = {name},
            .smiles = smiles
        })
    End Sub

    ''' <summary>构建一个 MetabolicReaction。</summary>
    Private Sub R(list As List(Of MetabolicReaction), id As String, name As String,
                  ec As String(), reversible As Boolean,
                  left As String(), right As String())
        list.Add(New MetabolicReaction With {
            .id = id,
            .name = name,
            .ECNumbers = ec,
            .is_reversible = reversible,
            .gibbs = 0,
            .left = left.Select(Function(cid) New CompoundSpecieReference(1, cid)).ToArray,
            .right = right.Select(Function(cid) New CompoundSpecieReference(1, cid)).ToArray
        })
    End Sub

End Module
