#Region "Microsoft.VisualBasic::27232113435b6c5d0db6dbdb74b529e0, ..\GCModeller\models\Networks\Microbiome\UniProtExtensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Module UniProtExtensions

    <Extension>
    Public Iterator Function PopulateModels(repo As TaxonomyRepository,
                                            taxonomyList As IEnumerable(Of Metagenomics.Taxonomy),
                                            Optional distinct As Boolean = True) As IEnumerable(Of TaxonomyRef)
        Dim hitsID As New Index(Of String)
        ' Dim taxonomyGroup = repo.TaxonomyGroup

        For Each taxonomy As Metagenomics.Taxonomy In taxonomyList
            For Each hit As TaxonomyRef In repo.Selects(range:=taxonomy)
                If distinct Then
                    If Not hit.TaxonID.IsOneOfA(hitsID) Then
                        Call hitsID.Add(hit.TaxonID)
                        Yield hit
                    End If
                Else
                    Yield hit
                End If
            Next
        Next
    End Function
End Module
