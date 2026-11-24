Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Definitions

    ''' <summary>
    ''' 不同数据库(MetaCyc / BioCyc / KEGG)的分子实体编号映射。
    ''' 
    ''' 模拟引擎内部只使用「语义角色」(water / glucose / ammonium ...)，
    ''' 由本模块把这些角色翻译成目标数据库之中的化合物编号。
    ''' </summary>
    Public Module DbContextMapping

        Private Function initMassValue(initMass As Double) As Func(Of Double)
            If initMass.IsNaNImaginary Then
                Return Function() 10000 * randf.NextDouble
            Else
                Return Function() initMass
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="allCompounds"></param>
        ''' <param name="initMass">NaN for random value</param>
        ''' <returns></returns>
        Public Function MetaCyc(allCompounds As IEnumerable(Of String), Optional initMass# = 100) As Definition
            Dim val As Func(Of Double) = initMassValue(initMass)
            Dim initStatus As Dictionary(Of String, Double) = allCompounds _
                .ToDictionary(Function(cid) cid,
                              Function(cid)
                                  Return val()
                              End Function)
            Dim ntBase As New NucleicAcid With {
                .A = "ADENOSINE",
                .C = "CYTIDINE",
                .G = "GUANOSINE",
                .U = "URIDINE",
                .T = "THYMIDINE"
            }
            Dim aaResidue As New AminoAcid With {
                .A = "L-ALPHA-ALANINE",
                .C = "CYS",
                .D = "L-ASPARTATE",
                .E = "GLT",
                .F = "PHE",
                .G = "GLY",
                .H = "HIS",
                .I = "ILE",
                .K = "LYS",
                .L = "LEU",
                .M = "MET",
                .N = "ASN",
                .P = "PRO",
                .Q = "GLN",
                .R = "ARG",
                .S = "SER",
                .T = "THR",
                .V = "VAL",
                .W = "TRP",
                .Y = "TYR",
                .U = "L-SELENOCYSTEINE", ' 特殊氨基酸映射
                .O = "Protein-L-pyrrolysine",
                .B = "Protein-L-asx",   ' Asp或Asn
                .Z = "Protein-L-glx"    ' Glu或Gln
            }

            Return New Definition With {
                .ADP = "ADP",
                .ATP = "ATP",
                .Water = "WATER",
                .Oxygen = "OXYGEN-MOLECULE",
                .NucleicAcid = ntBase,
                .AminoAcid = aaResidue,
                .PPI = "PPI",
                .PI = "Pi",
                .GTP = "GTP",
                .GDP = "GDP",
                .Ammonia = "AMMONIA",
                .CultureMedium = "Extracellular",
                .CultureMediumComponents = MetaCycMedium(),
                .Ions = MetaCycIons(),
                .Cofactors = MetaCycCofactors(),
                .WasteProducts = MetaCycWasteProducts(),
                .context = {New CellContext With {.name = "default", .status = initStatus}},
                .GenericCompounds = New Dictionary(Of String, GeneralCompound) From {
                    {"Peptides", GeneralCompound.Protein},
                    {"DNA-Holder", GeneralCompound.DNA},
                    {"RNA-Holder", GeneralCompound.RNA},
                    {"Compounds-Holder-Class", GeneralCompound.Compound}
                },
                .transmembrane = New Transmembrane With {
                    .passive = {"WATER", "OXYGEN-MOLECULE", "CARBON-DIOXIDE", "AMMONIA"}
                }
            }
        End Function

        ''' <summary>
        ''' BioCyc(EcoCyc)数据库的实体映射。
        ''' 
        ''' EcoCyc 与 MetaCyc 的化合物编号同源，因此直接复用 <see cref="MetaCyc"/> 的定义。
        ''' </summary>
        ''' <param name="allCompounds"></param>
        ''' <param name="initMass">NaN for random value</param>
        ''' <returns></returns>
        Public Function BioCyc(allCompounds As IEnumerable(Of String), Optional initMass# = 100) As Definition
            Return MetaCyc(allCompounds, initMass)
        End Function

        ''' <summary>
        ''' Get the KEGG compound <see cref="Definition"/>
        ''' </summary>
        ''' <param name="initMass">
        ''' NaN for random value
        ''' </param>
        ''' <returns></returns>
        Public Function KEGG(allCompounds As IEnumerable(Of String), Optional initMass# = 100) As Definition
            Dim val As Func(Of Double) = initMassValue(initMass)
            Dim initStatus As Dictionary(Of String, Double) = allCompounds _
                .ToDictionary(Function(cid) cid,
                              Function(cid)
                                  Return val()
                              End Function)
            Dim ntBase As New NucleicAcid With {
                .A = "C00212",
                .C = "C00475",
                .G = "C00387",
                .U = "C00299"
            }
            Dim aaResidue As New AminoAcid With {
                .A = "C00041",
                .U = "C05688",
                .G = "C00037",
                .C = "C00097",
                .D = "C00049",
                .E = "C00025",
                .F = "C00079",
                .H = "C00135",
                .I = "C00407",
                .K = "C00047",
                .L = "C00123",
                .M = "C00073",
                .N = "C00152",
                .O = "C16138",
                .P = "C00148",
                .Q = "C00064",
                .R = "C00062",
                .S = "C00065",
                .T = "C00188",
                .V = "C00183",
                .W = "C00078",
                .Y = "C00082"
            }

            Return New Definition With {
                .ADP = "C00008",
                .ATP = "C00002",
                .Water = "C00001",
                .Oxygen = "C00007",
                .NucleicAcid = ntBase,
                .AminoAcid = aaResidue,
                .PPI = "C00013",
                .PI = "C00009",
                .GTP = "C00044",
                .GDP = "C00035",
                .Ammonia = "C00014",
                .CultureMedium = "Extracellular",
                .CultureMediumComponents = KeggMedium(),
                .Ions = KeggIons(),
                .Cofactors = KeggCofactors(),
                .WasteProducts = KeggWasteProducts(),
                .context = {New CellContext With {.name = "default", .status = initStatus}},
                .GenericCompounds = New Dictionary(Of String, GeneralCompound) From {
                    {"C00017", GeneralCompound.Protein},
                    {"C00039", GeneralCompound.DNA},
                    {"C00046", GeneralCompound.RNA}
                },
                .transmembrane = New Transmembrane With {
                    .passive = {"C00001", "C00007", "C00011", "C00014"}
                }
            }
        End Function

#Region "MetaCyc / BioCyc 配方"

        ''' <summary>
        ''' 默认的基础培养基配方(MetaCyc 编号，浓度单位 mM)。
        ''' 
        ''' 只包含无机盐、碳源、氮源与微量元素，不含维生素与氨基酸 —— 后者由用户通过
        ''' 培养基 csv 文件补充；模型之中不存在的编号会在初始化的时候被自动过滤掉。
        ''' </summary>
        ''' <returns></returns>
        Public Function MetaCycMedium() As MediumComponent()
            Return {
                New MediumComponent With {.role = "glucose", .id = "GLC", .concentration = 10.0},
                New MediumComponent With {.role = "oxygen", .id = "OXYGEN-MOLECULE", .concentration = 1.0},
                New MediumComponent With {.role = "ammonium", .id = "AMMONIUM", .concentration = 5.0},
                New MediumComponent With {.role = "phosphate", .id = "Pi", .concentration = 1.0},
                New MediumComponent With {.role = "sulfate", .id = "SULFATE", .concentration = 1.0},
                New MediumComponent With {.role = "potassium", .id = "K+", .concentration = 1.0},
                New MediumComponent With {.role = "magnesium", .id = "MG+2", .concentration = 0.5},
                New MediumComponent With {.role = "calcium", .id = "CA+2", .concentration = 0.1},
                New MediumComponent With {.role = "chloride", .id = "CL-", .concentration = 1.0},
                New MediumComponent With {.role = "iron2", .id = "FE+2", .concentration = 0.01},
                New MediumComponent With {.role = "iron3", .id = "FE+3", .concentration = 0.01},
                New MediumComponent With {.role = "zinc", .id = "ZN+2", .concentration = 0.001},
                New MediumComponent With {.role = "manganese", .id = "MN+2", .concentration = 0.001},
                New MediumComponent With {.role = "copper", .id = "CU+2", .concentration = 0.0005},
                New MediumComponent With {.role = "cobalt", .id = "CO+2", .concentration = 0.0005},
                New MediumComponent With {.role = "nickel", .id = "NI+2", .concentration = 0.0005},
                New MediumComponent With {.role = "water", .id = "WATER", .concentration = 55.5},
                New MediumComponent With {.role = "proton", .id = "PROTON", .concentration = 0.0001}
            }
        End Function

        Public Function MetaCycIons() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"K", "K+"},
                {"Na", "NA+"},
                {"Mg", "MG+2"},
                {"Ca", "CA+2"},
                {"Cl", "CL-"},
                {"Fe2", "FE+2"},
                {"Fe3", "FE+3"},
                {"Zn", "ZN+2"},
                {"Mn", "MN+2"},
                {"Cu", "CU+2"},
                {"Co", "CO+2"},
                {"Ni", "NI+2"},
                {"Mo", "MOO4-2"},
                {"H", "PROTON"},
                {"OH", "OH-"}
            }
        End Function

        Public Function MetaCycCofactors() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"NAD", "NAD"},
                {"NADH", "NADH"},
                {"NADP", "NADP"},
                {"NADPH", "NADPH"},
                {"FAD", "FAD"},
                {"FADH2", "FADH2"},
                {"CO-A", "CO-A"},
                {"ACETYL-COA", "ACETYL-COA"},
                {"THIAMINE-PP", "THIAMINE-PYROPHOSPHATE"},
                {"PYRIDOXAL-P", "PYRIDOXAL_PHOSPHATE"},
                {"COBALAMIN", "COBALAMIN"}
            }
        End Function

        Public Function MetaCycWasteProducts() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"CO2", "CARBON-DIOXIDE"},
                {"HCO3", "HCO3"},
                {"LACTATE", "L-LACTATE"},
                {"ETHANOL", "ETOH"},
                {"FORMATE", "FORMATE"},
                {"ACETATE", "ACET"},
                {"H2", "HYDROGEN-MOLECULE"},
                {"H2S", "HS"},
                {"NH3", "AMMONIA"}
            }
        End Function

#End Region

#Region "KEGG 配方"

        ''' <summary>
        ''' 默认的基础培养基配方(KEGG 编号，浓度单位 mM)
        ''' </summary>
        ''' <returns></returns>
        Public Function KeggMedium() As MediumComponent()
            Return {
                New MediumComponent With {.role = "glucose", .id = "C00031", .concentration = 10.0},
                New MediumComponent With {.role = "oxygen", .id = "C00007", .concentration = 1.0},
                New MediumComponent With {.role = "ammonium", .id = "C00014", .concentration = 5.0},
                New MediumComponent With {.role = "phosphate", .id = "C00009", .concentration = 1.0},
                New MediumComponent With {.role = "sulfate", .id = "C00059", .concentration = 1.0},
                New MediumComponent With {.role = "potassium", .id = "C00238", .concentration = 1.0},
                New MediumComponent With {.role = "magnesium", .id = "C00305", .concentration = 0.5},
                New MediumComponent With {.role = "calcium", .id = "C00076", .concentration = 0.1},
                New MediumComponent With {.role = "chloride", .id = "C00115", .concentration = 1.0},
                New MediumComponent With {.role = "iron2", .id = "C14818", .concentration = 0.01},
                New MediumComponent With {.role = "iron3", .id = "C14819", .concentration = 0.01},
                New MediumComponent With {.role = "zinc", .id = "C00038", .concentration = 0.001},
                New MediumComponent With {.role = "manganese", .id = "C19610", .concentration = 0.001},
                New MediumComponent With {.role = "copper", .id = "C00070", .concentration = 0.0005},
                New MediumComponent With {.role = "cobalt", .id = "C00175", .concentration = 0.0005},
                New MediumComponent With {.role = "nickel", .id = "C19609", .concentration = 0.0005},
                New MediumComponent With {.role = "water", .id = "C00001", .concentration = 55.5},
                New MediumComponent With {.role = "proton", .id = "C00080", .concentration = 0.0001}
            }
        End Function

        Public Function KeggIons() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"K", "C00238"},
                {"Na", "C01330"},
                {"Mg", "C00305"},
                {"Ca", "C00076"},
                {"Cl", "C00115"},
                {"Fe2", "C14818"},
                {"Fe3", "C14819"},
                {"Zn", "C00038"},
                {"Mn", "C19610"},
                {"Cu", "C00070"},
                {"Co", "C00175"},
                {"Ni", "C19609"},
                {"Mo", "C06232"},
                {"H", "C00080"},
                {"OH", "C01377"}
            }
        End Function

        Public Function KeggCofactors() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"NAD", "C00003"},
                {"NADH", "C00004"},
                {"NADP", "C00006"},
                {"NADPH", "C00005"},
                {"FAD", "C00016"},
                {"FADH2", "C01352"},
                {"CO-A", "C00010"},
                {"ACETYL-COA", "C00024"},
                {"THIAMINE-PP", "C00068"},
                {"PYRIDOXAL-P", "C00018"},
                {"COBALAMIN", "C02823"}
            }
        End Function

        Public Function KeggWasteProducts() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"CO2", "C00011"},
                {"HCO3", "C00288"},
                {"LACTATE", "C00186"},
                {"ETHANOL", "C00469"},
                {"FORMATE", "C00058"},
                {"ACETATE", "C00033"},
                {"H2", "C00282"},
                {"H2S", "C00283"},
                {"NH3", "C00014"}
            }
        End Function

#End Region

    End Module
End Namespace
