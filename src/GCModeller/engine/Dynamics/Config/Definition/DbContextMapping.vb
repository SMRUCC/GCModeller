Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Definitions

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
                .context = {New CellContext With {.name = "default", .status = initStatus}},
                .GenericCompounds = New Dictionary(Of String, GeneralCompound) From {
                    {"Peptides", GeneralCompound.Protein},
                    {"DNA-Holder", GeneralCompound.DNA},
                    {"RNA-Holder", GeneralCompound.RNA},
                    {"Compounds-Holder-Class", GeneralCompound.Compound}
                },
                .transmembrane = New Transmembrane With {
                    .passive = {"WATER", "OXYGEN-MOLECULE"}
                }
            }
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
                .context = {New CellContext With {.name = "default", .status = initStatus}},
                .GenericCompounds = New Dictionary(Of String, GeneralCompound) From {
                    {"C00017", GeneralCompound.Protein},
                    {"C00039", GeneralCompound.DNA},
                    {"C00046", GeneralCompound.RNA}
                }
            }
        End Function
    End Module
End Namespace