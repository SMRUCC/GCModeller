#Region "Microsoft.VisualBasic::b62a6ff609f27e8d40e21998348e45da, GCModeller\annotations\GSEA\GSEA.KnowledgeBase.Extensions\BBHLibrary.vb"

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

    '   Total Lines: 74
    '    Code Lines: 53
    ' Comment Lines: 13
    '   Blank Lines: 8
    '     File Size: 3.02 KB


    ' Module BBHLibrary
    ' 
    '     Function: CreateBackground
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' Module for create GSEA background model from bbh annotation result.
''' </summary>
Public Module BBHLibrary

    ''' <summary>
    ''' Create GSEA background model from bbh annotation result.
    ''' </summary>
    ''' <param name="annotations"></param>
    ''' <param name="backgroundSize">
    ''' The total number of genes in background genome. 
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(annotations As IEnumerable(Of BiDirectionalBesthit), define As GetClusterTerms,
                                     Optional backgroundSize% = -1,
                                     Optional outputAll As Boolean = True,
                                     Optional genomeName$ = "Unknown") As Background

        ' [clusterName => members]
        Dim clusters As New Dictionary(Of String, List(Of BackgroundGene))
        Dim clusterNotes As New Dictionary(Of String, NamedValue(Of String))
        Dim counts%

        For Each gene As BiDirectionalBesthit In annotations
            ' map KO term to cluster id list
            Dim clusterList = define(gene.HitName)

            counts += 1

            For Each cluster As NamedValue(Of String) In clusterList
                If Not clusters.ContainsKey(cluster.Name) Then
                    clusters(cluster.Name) = New List(Of BackgroundGene)
                    clusterNotes(cluster.Name) = cluster
                End If

                clusters(cluster.Name) += New BackgroundGene With {
                    .accessionID = gene.QueryName,
                    .[alias] = {gene.HitName},
                    .locus_tag = New NamedValue With {
                        .name = gene.QueryName
                    },
                    .term_id = BackgroundGene.UnknownTerms(gene.HitName).ToArray,
                    .name = gene.description
                }
            Next
        Next

        Return New Background With {
            .build = Now,
            .clusters = clusters _
                .Where(Function(c)
                           If outputAll Then
                               Return True
                           Else
                               Return c.Value > 0
                           End If
                       End Function) _
                .Select(Function(c)
                            Return c.Value.CreateCluster(c.Key, clusterNotes(c.Key))
                        End Function) _
                .ToArray,
            .comments = "GSEA background model build from BBH annotation result by GCModeller",
            .name = genomeName,
            .size = backgroundSize Or counts.When(backgroundSize <= 0)
        }
    End Function
End Module
