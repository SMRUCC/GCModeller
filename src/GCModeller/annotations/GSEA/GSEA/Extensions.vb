#Region "Microsoft.VisualBasic::a440374b67542d91b531114e6e24d678, GCModeller\annotations\GSEA\GSEA\Extensions.vb"

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

    '   Total Lines: 60
    '    Code Lines: 54
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 2.19 KB


    ' Module Extensions
    ' 
    '     Function: BackgroundFromCatalog, createClusters, createGenes
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Function BackgroundFromCatalog(catalog As Dictionary(Of String, CatalogProfiling),
                                          Optional id$ = Nothing,
                                          Optional name$ = "n/a",
                                          Optional size% = -1,
                                          Optional comments$ = "none") As Background

        Dim background As New Background With {
            .build = Now,
            .clusters = catalog.createClusters.ToArray,
            .comments = comments,
            .id = id Or App.NextTempName.AsDefault,
            .name = name,
            .size = size
        }

        Return background
    End Function

    <Extension>
    Private Iterator Function createClusters(catalog As Dictionary(Of String, CatalogProfiling)) As IEnumerable(Of Cluster)
        For Each category As KeyValuePair(Of String, CatalogProfiling) In catalog
            For Each subcat As KeyValuePair(Of String, CatalogList) In category.Value.SubCategory
                Yield New Cluster With {
                    .description = subcat.Value.Description,
                    .ID = subcat.Value.Catalog,
                    .names = subcat.Value.Description,
                    .members = subcat.Value _
                        .createGenes _
                        .ToArray
                }
            Next
        Next
    End Function

    <Extension>
    Private Iterator Function createGenes(list As CatalogList) As IEnumerable(Of BackgroundGene)
        For Each id As String In list.IDs
            Yield New BackgroundGene With {
                .accessionID = id,
                .[alias] = {id},
                .locus_tag = New NamedValue With {
                    .name = id,
                    .text = id
                },
                .name = id,
                .term_id = {id}
            }
        Next
    End Function
End Module
