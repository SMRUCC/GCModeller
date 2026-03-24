Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Module TableExport

    <Extension>
    Public Iterator Function SVTable(result As PanGenomeResult) As IEnumerable(Of SVTable)
        For Each var As StructuralVariation In result.StructuralVariations.SafeQuery
            Yield New SVTable(var)
        Next
    End Function

    <Extension>
    Public Iterator Function PAVTable(result As PanGenomeResult) As IEnumerable(Of PAVTable)
        For Each family In result.PAVMatrix.SafeQuery
            Yield New PAVTable With {
                .FamilyID = family.Key,
                .PAV = family.Value,
                .ClusterGenes = result.GeneFamilies(family.Key)
            }
        Next
    End Function

End Module
