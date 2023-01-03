#Region "Microsoft.VisualBasic::0b863b01c1ec10b85c50450ad2192ac7, GCModeller\annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\MultipleOmics.vb"

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

    '   Total Lines: 53
    '    Code Lines: 39
    ' Comment Lines: 8
    '   Blank Lines: 6
    '     File Size: 1.83 KB


    ' Module MultipleOmics
    ' 
    '     Function: CreateOmicsBackground, getCluster
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices

''' <summary>
''' helper for create enrichment background model for multiple omics data analysis
''' </summary>
Public Module MultipleOmics

    ''' <summary>
    ''' GSEA background model of kegg compound + genes
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="filter_compoundId"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateOmicsBackground(model As IEnumerable(Of Pathway),
                                          Optional filter_compoundId As Boolean = True,
                                          Optional kegg_code As String = Nothing) As Background
        Dim clusters As Cluster() = model _
            .Select(Function(m) getCluster(m, filter_compoundId, kegg_code)) _
            .Where(Function(c) c.size > 0 AndAlso Not c.ID.StringEmpty) _
            .ToArray

        Return New Background With {
            .clusters = clusters,
            .build = Now,
            .comments = "KEGG pathway multiple omics analysis",
            .id = "",
            .name = "",
            .size = .clusters.BackgroundSize
        }
    End Function

    ''' <summary>
    ''' create background model of combine genes with compounds
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    Private Function getCluster(model As Pathway, filter_compoundId As Boolean, kegg_code As String) As Cluster
        Dim molecules As New List(Of BackgroundGene)(model.GetGeneMembers(kegg_code))
        Dim cid As NamedValue

        For Each compound As NamedValue In model.compound.SafeQuery
            If filter_compoundId AndAlso Not compound.name.IsPattern("C\d+") Then
                Continue For
            Else
                cid = New NamedValue With {
                    .name = "compound",
                    .text = compound.name
                }
            End If

            Call molecules.Add(New BackgroundGene With {
                .accessionID = compound.name,
                .[alias] = {compound.name},
                .term_id = {cid},
                .name = compound.text,
                .locus_tag = compound
            })
        Next

        Return New Cluster With {
            .ID = model.EntryId,
            .names = model.name,
            .description = model.description,
            .members = molecules.ToArray
        }
    End Function
End Module
