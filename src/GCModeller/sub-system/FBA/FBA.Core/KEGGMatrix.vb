Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Module KEGGMatrix

    <Extension>
    Public Function CreateKeggMatrix(keggNetwork As IEnumerable(Of Reaction)) As Matrix
        Dim graph As Equation() = keggNetwork.Select(Function(r) r.ReactionModel).ToArray
        Dim allCompounds As String() = graph _
            .Select(Function(r) r.GetMetabolites) _
            .IteratesALL _
            .Select(Function(sp) sp.ID) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim matrix As Double()() = allCompounds _
            .Select(Function(id)
                        Return graph _
                            .Select(Function(r) r.GetCoEfficient(id)) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New Matrix With {
            .Matrix = matrix,
            .Compounds = allCompounds,
            .Flux = graph _
                .ToDictionary(Function(flux) flux.Id,
                              Function(flux)
                                  Return New DoubleRange(-10, 10)
                              End Function),
            .Targets = graph.Select(Function(r) r.Id).ToArray
        }
    End Function

End Module
