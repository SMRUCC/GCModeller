Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Public Module TableExport

    <Extension>
    Public Iterator Function SVTable(result As PanGenomeResult) As IEnumerable(Of SVTable)
        Dim cat As New CategoryHashSet(result)

        For Each var As StructuralVariation In result.StructuralVariations.SafeQuery
            Yield New SVTable(var) With {
                .Category = cat.MakeCategory(.FamilyID),
                .Dispensable = .FamilyID Like cat.DispensableGeneFamilies,
                .SingleCopyOrtholog = .FamilyID Like cat.SingleCopyOrthologFamilies
            }
        Next
    End Function

    <Extension>
    Public Iterator Function PAVTable(result As PanGenomeResult) As IEnumerable(Of PAVTable)
        Dim cat As New CategoryHashSet(result)

        For Each family In result.PAVMatrix.SafeQuery
            Yield New PAVTable With {
                .FamilyID = family.Key,
                .PAV = family.Value,
                .ClusterGenes = result.GeneFamilies(family.Key),
                .Category = cat.MakeCategory(.FamilyID),
                .Dispensable = .FamilyID Like cat.DispensableGeneFamilies,
                .SingleCopyOrtholog = .FamilyID Like cat.SingleCopyOrthologFamilies
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

        Public Function MakeCategory(id As String) As GeneCategoryType
            If id Like CoreGeneFamilies Then
                Return GeneCategoryType.Core
            ElseIf id Like SpecificGeneFamilies Then
                Return GeneCategoryType.Unique
            ElseIf id Like SoftCoreGeneFamilies Then
                Return GeneCategoryType.SoftCore
            ElseIf id Like ShellGeneFamilies Then
                Return GeneCategoryType.Shell
            Else
                Return GeneCategoryType.Cloud
            End If
        End Function

    End Class

End Module
