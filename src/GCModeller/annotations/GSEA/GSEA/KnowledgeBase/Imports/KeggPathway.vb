#Region "Microsoft.VisualBasic::518954bd06afb4cfd7b3cf378cb126fc, GCModeller\annotations\GSEA\GSEA\KnowledgeBase\Imports\KeggPathway.vb"

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

    '   Total Lines: 58
    '    Code Lines: 45
    ' Comment Lines: 8
    '   Blank Lines: 5
    '     File Size: 2.04 KB


    ' Module KeggPathway
    ' 
    '     Function: CreateModel, getGeneCluster, GetGeneMembers
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

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
    Public Function CreateModel(models As IEnumerable(Of Pathway)) As Background
        Dim clusters As Cluster() = models _
            .Select(Function(p) p.getGeneCluster) _
            .Where(Function(c) c.size > 0 AndAlso Not c.ID.StringEmpty) _
            .ToArray
        Dim model As New Background With {
            .build = Now,
            .clusters = clusters,
            .id = "",
            .comments = "",
            .name = "",
            .size = .clusters.BackgroundSize
        }

        Return model
    End Function

    <Extension>
    Private Function getGeneCluster(model As Pathway) As Cluster
        Return New Cluster With {
            .description = model.description,
            .ID = model.EntryId,
            .members = model.GetGeneMembers.ToArray,
            .names = model.name
        }
    End Function

    <Extension>
    Public Iterator Function GetGeneMembers(model As Pathway) As IEnumerable(Of BackgroundGene)
        For Each gene As GeneName In model.genes.SafeQuery
            Yield New BackgroundGene With {
                .accessionID = gene.geneId,
                .name = gene.geneName,
                .[alias] = {gene.geneId, gene.geneName},
                .locus_tag = New NamedValue With {.name = gene.geneId, .text = gene.description},
                .term_id = New String() {gene.KO} _
                    .JoinIterates(gene.EC) _
                    .ToArray
            }
        Next
    End Function
End Module
