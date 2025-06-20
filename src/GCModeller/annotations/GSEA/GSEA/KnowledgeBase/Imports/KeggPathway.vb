﻿#Region "Microsoft.VisualBasic::139aefd09fc3becd4b835e7d75db8e97, annotations\GSEA\GSEA\KnowledgeBase\Imports\KeggPathway.vb"

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

    '   Total Lines: 75
    '    Code Lines: 60 (80.00%)
    ' Comment Lines: 8 (10.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (9.33%)
    '     File Size: 2.89 KB


    ' Module KeggPathway
    ' 
    '     Function: CreateModel, getGeneCluster, GetGeneMembers
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Enum OmicsData
    Transcriptomics = 1
    Metabolomics = 2
    MultipleOmics = 3
End Enum

''' <summary>
''' helper for create kegg pathway maps enrichment background model
''' </summary>
Public Module KeggPathway

    ''' <summary>
    ''' create a gene background model based on the given of kegg pathway object
    ''' </summary>
    ''' <param name="models"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateModel(models As IEnumerable(Of Pathway), Optional omics As OmicsData = OmicsData.Transcriptomics) As Background
        Dim clusters As Cluster() = models _
            .Select(Function(p) p.getGeneCluster(omics)) _
            .Where(Function(c) c.size > 0 AndAlso Not c.ID.StringEmpty) _
            .ToArray
        Dim model As New Background With {
            .build = Now,
            .clusters = clusters,
            .id = "",
            .comments = "",
            .name = If(omics = OmicsData.Transcriptomics,
                "Background Model for Metabolomics",
                "Background Model for Gene Expression"
            ),
            .size = .clusters.BackgroundSize
        }

        Return model
    End Function

    <Extension>
    Private Function getGeneCluster(model As Pathway, omics As OmicsData) As Cluster
        Return New Cluster With {
            .description = model.description,
            .ID = model.EntryId,
            .members = If(omics = OmicsData.Metabolomics,
                model.GetMetaboliteMembers,
                model.GetGeneMembers
            ).ToArray,
            .names = model.name
        }
    End Function

    <Extension>
    Public Iterator Function GetMetaboliteMembers(model As Pathway, Optional kegg_code As String = Nothing) As IEnumerable(Of BackgroundGene)
        For Each gene As NamedValue(Of String) In model.GetCompoundSet
            Yield New BackgroundGene With {
                .accessionID = gene.Name,
                .name = gene.Value,
                .[alias] = {
                    If(kegg_code.StringEmpty, gene.Name, $"{kegg_code}:{gene.Name}"),
                    gene.Name
                }.Distinct.ToArray,
                .locus_tag = New NamedValue With {
                    .name = gene.Name,
                    .text = gene.Description
                }
            }
        Next
    End Function

    <Extension>
    Public Iterator Function GetGeneMembers(model As Pathway, Optional kegg_code As String = Nothing) As IEnumerable(Of BackgroundGene)
        Dim term_populate As IEnumerable(Of NamedValue)

        For Each gene As GeneName In model.genes.SafeQuery
            term_populate = Iterator Function() As IEnumerable(Of NamedValue)
                                If Not gene.KO.StringEmpty Then
                                    Yield New NamedValue With {.name = "KO", .text = gene.KO}
                                End If
                                If Not gene.EC.IsNullOrEmpty Then
                                    For Each id As String In gene.EC
                                        Yield New NamedValue With {.name = "ECNumber", .text = id}
                                    Next
                                End If
                            End Function()

            Yield New BackgroundGene With {
                .accessionID = gene.geneId,
                .name = gene.geneName,
                .[alias] = {
                    If(kegg_code.StringEmpty, gene.geneId, $"{kegg_code}:{gene.geneId}"),
                    gene.geneName
                },
                .locus_tag = New NamedValue With {
                    .name = gene.geneId,
                    .text = gene.description
                },
                .term_id = term_populate.ToArray
            }
        Next
    End Function
End Module
