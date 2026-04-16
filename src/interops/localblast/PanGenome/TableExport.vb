#Region "Microsoft.VisualBasic::da14a56f70ab6b68483a4bcd2e05d619, localblast\PanGenome\TableExport.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 72
    '    Code Lines: 61 (84.72%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (15.28%)
    '     File Size: 2.94 KB


    ' Module TableExport
    ' 
    '     Function: PAVTable, SVTable
    '     Class CategoryHashSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: MakeCategory
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

