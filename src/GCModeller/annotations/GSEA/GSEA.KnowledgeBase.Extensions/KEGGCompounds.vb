#Region "Microsoft.VisualBasic::fb9721781e031defda6a20ef21e7b398, GSEA\GSEA.KnowledgeBase.Extensions\KEGGCompounds.vb"

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

    ' Module KEGGCompounds
    ' 
    '     Function: CreateBackground
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

''' <summary>
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.
''' </summary>
Public Module KEGGCompounds

    ''' <summary>
    ''' Create GSEA background model from LC-MS metabolism analysis result.
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(org As OrganismInfo, maps As IEnumerable(Of Pathway)) As Background
        ' The total number of metabolites in background genome. 
        Dim backgroundSize% = 0
        Dim clusters As New List(Of Cluster)

        For Each map As Pathway In maps
            clusters += New Cluster With {
                .description = map.description,
                .ID = map.EntryId,
                .names = map.name,
                .members = map.compound _
                    .Select(Function(c)
                                Return New BackgroundGene With {
                                    .name = c.text,
                                    .accessionID = c.name,
                                    .[alias] = {c.name},
                                    .locus_tag = c,
                                    .term_id = {c.name}
                                }
                            End Function) _
                    .ToArray
            }
        Next

        backgroundSize = clusters _
            .Select(Function(c) c.members) _
            .IteratesALL _
            .Select(Function(c) c.accessionID) _
            .Distinct _
            .Count

        Return New Background With {
            .build = Now,
            .clusters = clusters,
            .comments = "Background model apply for GSEA of LC-MS metabolism analysis, created by GCModeller.",
            .name = org.FullName,
            .size = backgroundSize,
            .id = org.code
        }
    End Function
End Module
