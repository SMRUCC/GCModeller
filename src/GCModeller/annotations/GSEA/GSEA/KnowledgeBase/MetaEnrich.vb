﻿#Region "Microsoft.VisualBasic::03bd8b817b0d6f4c51bd89f7b7c85b6c, annotations\GSEA\GSEA\KnowledgeBase\MetaEnrich.vb"

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

    '   Total Lines: 54
    '    Code Lines: 43 (79.63%)
    ' Comment Lines: 6 (11.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (9.26%)
    '     File Size: 2.09 KB


    ' Module MetaEnrich
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Function: CastBackground, CastCluster
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module MetaEnrich

    Public Delegate Function GraphQuery(geneId As String) As IEnumerable(Of NamedCollection(Of String))

    ''' <summary>
    ''' create a new background model based on the enrichment analysis result
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="query"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CastBackground(background As IEnumerable(Of EnrichmentResult), query As GraphQuery) As Background
        Return New Background With {
            .build = Now,
            .clusters = background _
                .Select(Function(a) a.CastCluster(query)) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function CastCluster(background As EnrichmentResult, query As GraphQuery) As Cluster
        Return New Cluster With {
            .ID = background.term,
            .names = background.name,
            .description = background.description,
            .members = background.IDs _
                .Select(AddressOf query.Invoke) _
                .IteratesALL _
                .Select(Function(a) a.value) _
                .IteratesALL _
                .Distinct _
                .Select(Function(id)
                            Return New BackgroundGene With {
                                .accessionID = id,
                                .[alias] = {id},
                                .locus_tag = New NamedValue With {
                                    .name = id,
                                    .text = id
                                },
                                .name = id,
                                .term_id = BackgroundGene.UnknownTerms(id).ToArray
                            }
                        End Function) _
                .ToArray
        }
    End Function

End Module
