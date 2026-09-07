Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Module MetabolicHelper

    <Extension>
    Public Function BuildMatrix(gem As IEnumerable(Of Equation)) As Matrix
        Dim allCompounds As String() = gem _
            .Select(Function(r) r.GetMetabolites) _
            .IteratesALL _
            .Select(Function(sp) sp.ID) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim matrix As Double()() = allCompounds _
            .Select(Function(id)
                        Return gem _
                            .Select(Function(r)
                                        Return r.GetCoEfficient(id, directional:=True)
                                    End Function) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New Matrix With {
            .Matrix = matrix,
            .Compounds = allCompounds,
            .Flux = gem _
                .ToDictionary(Function(flux) flux.Id,
                              Function(flux)
                                  Return New DoubleRange(-10, 10)
                              End Function),
            .Targets = gem.Select(Function(r) r.Id).ToArray
        }
    End Function
End Module
