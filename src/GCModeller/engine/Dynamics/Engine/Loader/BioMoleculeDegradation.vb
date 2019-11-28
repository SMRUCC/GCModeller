Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    ''' <summary>
    ''' 构建出生物大分子的降解过程
    ''' </summary>
    Public Class BioMoleculeDegradation : Inherits FluxLoader

        Public Property proteinMatures As Channel()

        Public Sub New(loader As Loader)
            MyBase.New(loader)
        End Sub

        Public Overrides Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Return proteinDegradation(cell).AsList + RNADegradation(cell)
        End Function

        Private Iterator Function proteinDegradation(cell As CellularModule) As IEnumerable(Of Channel)
            ' protein complex -> polypeptide + compounds
            ' polypeptide -> aminoacid
            Dim proteinComplex$
            Dim peptideId$
            Dim geneIDindex = cell.Genotype.centralDogmas _
                .Where(Function(cd) Not cd.polypeptide.StringEmpty) _
                .ToDictionary(Function(cd) cd.polypeptide,
                              Function(cd)
                                  Return cd.geneID
                              End Function)
            Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)
            Dim composition As ProteinComposition
            Dim aaResidue As Variable()
            Dim geneID$

            For Each complex As Channel In proteinMatures
                proteinComplex = complex.right.First(Function(c) c.Mass.ID.EndsWith(".complex")).Mass.ID
                peptideId = proteinComplex.Replace(".complex", "")
                geneID = geneIDindex(peptideId)
                composition = proteinMatrix(geneID)
                aaResidue = composition _
                    .Where(Function(i) i.Value > 0) _
                    .Select(Function(aa)
                                Dim aaName = loader.define.AminoAcid(aa.Name)
                                Return MassTable.variable(aaName, aa.Value)
                            End Function) _
                    .ToArray

                Yield New Channel(MassTable.variables({proteinComplex}, 1), MassTable.variables({peptideId}, 1)) With {
                    .ID = $"proteinComplexDegradationOf{proteinComplex}",
                    .forward = New Controls With {.baseline = 10},
                    .reverse = New Controls With {.baseline = 0},
                    .bounds = New Boundary With {
                        .forward = 1000,
                        .reverse = 0
                    }
                }

                Yield New Channel(MassTable.variables({peptideId}, 1), aaResidue) With {
                    .ID = $"polypeptideDegradationOf{peptideId}",
                    .forward = New Controls With {.baseline = 10},
                    .reverse = New Controls With {.baseline = 0},
                    .bounds = New Boundary With {
                        .forward = 1000,
                        .reverse = 0
                    }
                }
            Next
        End Function

        Private Iterator Function RNADegradation(cell As CellularModule) As IEnumerable(Of Channel)
            Dim centralDogmas = loader.GetCentralDogmaFluxLoader
            Dim composition As RNAComposition
            Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
            Dim ntBase As Variable()

            ' rna -> nt base
            For Each rna As String In centralDogmas.componentRNA.AsList + centralDogmas.mRNA
                composition = rnaMatrix(rna)
                ntBase = composition _
                    .Where(Function(i) i.Value > 0) _
                    .Select(Function(base)
                                Dim baseName = loader.define.NucleicAcid(base.Name)
                                Return MassTable.variable(baseName, base.Value)
                            End Function) _
                    .ToArray

                ' 降解过程是不可逆的
                Yield New Channel(MassTable.variables({rna}, 1), ntBase) With {
                    .ID = $"RNADegradationOf{rna}",
                    .forward = New Controls With {.baseline = 10},
                    .reverse = New Controls With {.baseline = 0},
                    .bounds = New Boundary With {
                        .forward = 1000,
                        .reverse = 0
                    }
                }
            Next
        End Function
    End Class
End Namespace