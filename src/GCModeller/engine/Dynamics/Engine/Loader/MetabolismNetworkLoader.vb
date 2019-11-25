
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    ''' <summary>
    ''' 构建代谢网络
    ''' </summary>
    Public Class MetabolismNetworkLoader : Inherits FluxLoader

        Public Sub New(loader As Loader)
            MyBase.New(loader)
        End Sub

        Public Overrides Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Dim KOfunctions = cell.Genotype.centralDogmas _
                .Where(Function(cd) Not cd.orthology.StringEmpty) _
                .Select(Function(cd) (cd.orthology, cd.polypeptide)) _
                .GroupBy(Function(pro) pro.Item1) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(ortholog)
                                  Return ortholog _
                                      .Select(Function(map) map.Item2) _
                                      .ToArray
                              End Function)

            For Each reaction As Reaction In cell.Phenotype.fluxes
                Yield fluxByReaction(reaction, KOfunctions)
            Next
        End Function

        Private Function fluxByReaction(reaction As Reaction, KOfunctions As Dictionary(Of String, String())) As Channel
            Dim left = MassTable.variables(reaction.substrates)
            Dim right = MassTable.variables(reaction.products)
            Dim bounds As New Boundary With {
                .forward = reaction.bounds.Max,
                .reverse = reaction.bounds.Min
            }

            ' KO
            Dim enzymeProteinComplexes As String() = reaction.enzyme _
                .SafeQuery _
                .Distinct _
                .OrderBy(Function(KO) KO) _
                .ToArray
            ' protein id
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Where(AddressOf KOfunctions.ContainsKey) _
                .Select(Function(ko) KOfunctions(ko)) _
                .IteratesALL _
                .Distinct _
                .ToArray
            ' mature protein complex
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Select(Function(id) id & ".complex") _
                .ToArray

            If reaction.is_enzymatic AndAlso enzymeProteinComplexes.Length = 0 Then
                bounds = {0, 10}
            End If

            Dim metabolismFlux As New Channel(left, right) With {
                .bounds = bounds,
                .ID = reaction.ID,
                .forward = New Controls With {
                    .activation = MassTable _
                        .variables(enzymeProteinComplexes, 2) _
                        .ToArray,
                    .baseline = 15
                },
                .reverse = New Controls With {.baseline = 15}
            }

            Return metabolismFlux
        End Function
    End Class
End Namespace