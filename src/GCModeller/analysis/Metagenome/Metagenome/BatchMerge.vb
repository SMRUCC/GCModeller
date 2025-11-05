Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module BatchMerge

    <Extension>
    Public Iterator Function BatchCombine(batch1 As OTUTable(), batch2 As OTUTable()) As IEnumerable(Of OTUTable)
        Dim merge As NamedCollection(Of OTUTable)() = batch1 _
            .JoinIterates(batch2) _
            .GroupBy(Function(otu) otu.taxonomy.ToString) _
            .Select(Function(otu) New NamedCollection(Of OTUTable)(otu.Key, otu.ToArray)) _
            .ToArray
        Dim otu_id As i32 = 1

        For Each otu As NamedCollection(Of OTUTable) In merge
            Dim vec As Dictionary(Of String, Double) = otu _
                .Select(Function(a) a.Properties) _
                .IteratesALL _
                .GroupBy(Function(a) a.Key) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Sum(Function(xi) xi.Value)
                              End Function)

            Yield New OTUTable With {
                .ID = $"otu{++otu_id}",
                .Properties = vec,
                .taxonomy = otu.First.taxonomy
            }
        Next
    End Function
End Module
