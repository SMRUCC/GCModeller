Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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

    Private Class CategoryHashSet

        Public CoreGeneFamilies As Index(Of String)
        Public DispensableGeneFamilies As Index(Of String)
        Public SpecificGeneFamilies As Index(Of String)
        Public SingleCopyOrthologFamilies As Index(Of String)
        Public SoftCoreGeneFamilies As Index(Of String)
        Public ShellGeneFamilies As Index(Of String)
        Public CloudGeneFamilies As Index(Of String)

        Sub New(result As PanGenomeResult)
            CoreGeneFamilies = result.CoreGeneFamilies
            DispensableGeneFamilies = result.DispensableGeneFamilies
            SpecificGeneFamilies = result.SpecificGeneFamilies
            SingleCopyOrthologFamilies = result.SingleCopyOrthologFamilies
            SoftCoreGeneFamilies = result.SoftCoreGeneFamilies
            ShellGeneFamilies = result.ShellGeneFamilies
            CloudGeneFamilies = result.CloudGeneFamilies
        End Sub

    End Class

End Module
