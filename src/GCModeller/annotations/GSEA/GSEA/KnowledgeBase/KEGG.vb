#Region "Microsoft.VisualBasic::7be653583dc448869e921f2532a84e55, GCModeller\annotations\GSEA\GSEA\KnowledgeBase\KEGG.vb"

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

    '   Total Lines: 49
    '    Code Lines: 46
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 2.21 KB


    ' Module KEGG
    ' 
    '     Function: KO_category, subtypeClusters
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Module KEGG

    Public Function IDCategoryFromBackground(background As Background) As ClassProfiles
        Dim KO_category = PathwayProfiles.GetPathwayClass.Values _
            .IteratesALL _
            .GroupBy(Function(p) p.EntryId) _
            .ToDictionary(Function(p) p.Key,
                          Function(group)
                              Return group.First
                          End Function)
        Dim [class] As New ClassProfiles With {
            .Catalogs = New Dictionary(Of String, CatalogProfiling)
        }

        For Each cluster As Cluster In background.clusters
            Dim pathId As String = cluster.ID.Match("\d+")
            Dim brite As Pathway = KO_category.TryGetValue(pathId)

            If Not brite Is Nothing Then
                Call [class] _
                    .GetClass(brite.class) _
                    .GetCategory(brite.category) _
                    .Add(cluster.memberIds)
            End If
        Next

        Return [class]
    End Function

    <Extension>
    Public Function KO_category(category As BriteHText) As IEnumerable(Of Cluster)
        Return category.categoryItems _
            .SafeQuery _
            .Select(Function(subtype)
                        Return subtype.subtypeClusters
                    End Function) _
            .IteratesALL
    End Function

    <Extension>
    Private Function subtypeClusters(subtype As BriteHText) As IEnumerable(Of Cluster)
        Return subtype.categoryItems _
            .SafeQuery _
            .Select(Function(pathway)
                        Return pathway.subtypeCluster
                    End Function)
    End Function

    <Extension>
    Private Function subtypeCluster(pathway As BriteHText) As Cluster
        Return New Cluster With {
            .ID = "map" & pathway.entryID,
            .description = pathway _
                .ToString _
                .Replace("[BR:ko]", "") _
                .Replace("[PATH:ko]", "") _
                .Trim,
            .names = pathway.description _
                .Replace("[BR:ko]", "") _
                .Replace("[PATH:ko]", "") _
                .Trim,
            .members = pathway.categoryItems _
                .SafeQuery _
                .Select(Function(ko)
                            Return New BackgroundGene With {
                                .accessionID = ko.entryID,
                                .[alias] = {ko.entryID},
                                .locus_tag = New NamedValue With {.name = ko.entryID, .text = ko.description},
                                .name = ko.description,
                                .term_id = {ko.entryID}
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
