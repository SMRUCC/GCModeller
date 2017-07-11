Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Public Module PseAA

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ALL">``{proteinID, GO-list}``</param>
    ''' <param name="threshold">``[0, 1]`` percentage</param>
    <Extension> Public Function Construct(ALL As IEnumerable(Of NamedValue(Of String())), Optional threshold# = 0.1) As NamedValue(Of Vector)()
        Dim data = ALL.ToArray
        Dim n As Dictionary(Of String, Double) = data _
            .Values _
            .IteratesALL _
            .GroupBy(Function(id) id) _
            .ToDictionary(Function(id) id.Key,
                          Function(count)
                              Return CDbl(count.Count)
                          End Function)
        ' cuts
        Dim lowerBound = n.Values.GKQuantile().Query(threshold)
        Dim categories As Index(Of String) =
            n.Keys _
            .Where(Function(id) n(id) >= lowerBound) _
            .OrderBy(Function(id) id) _
            .Indexing
        Dim length% = categories.Count
        Dim vectors = data _
            .Select(Function(protein)
                        Dim seq = protein.Value _
                            .SafeQuery _
                            .Where(Function(id) categories.IndexOf(id) > -1) _
                            .Select(Function(id) categories.IndexOf(id)) _
                            .ToArray
                        Dim v As Double() = New Double(length - 1) {}

                        For Each i In seq
                            v(i) = 1
                        Next

                        Return New NamedValue(Of Vector) With {
                            .Name = protein.Name,
                            .Value = v
                        }
                    End Function) _
            .ToArray
        Return vectors
    End Function
End Module
