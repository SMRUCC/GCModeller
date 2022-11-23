#Region "Microsoft.VisualBasic::8bd79287121cb433ae92a0aaa57569a4, GCModeller\annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\KEGGCompounds.vb"

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

    '   Total Lines: 121
    '    Code Lines: 95
    ' Comment Lines: 15
    '   Blank Lines: 11
    '     File Size: 4.56 KB


    ' Module KEGGCompounds
    ' 
    '     Function: CreateBackground, CreateGeneralBackground
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

''' <summary>
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, 
''' used for LC-MS metabolism data analysis.
''' </summary>
Public Module KEGGCompounds

    ''' <summary>
    ''' Create general reference GSEA background model from LC-MS metabolism analysis result.
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="KO">
    ''' a indexer for do map selection
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGeneralBackground(Of T As Map)(maps As IEnumerable(Of T), Optional KO As Index(Of String) = Nothing) As Background
        ' The total number of metabolites in background genome. 
        Dim backgroundSize% = 0
        Dim clusters As New List(Of Cluster)
        Dim names As NamedValue(Of String)()

        If KO Is Nothing Then
            KO = New String() {}
        End If

        For Each map As T In maps
            names = map.shapes _
                .Select(Function(a) a.Names) _
                .IteratesALL _
                .GroupBy(Function(n) n.Name) _
                .Select(Function(duplicated) duplicated.First) _
                .ToArray

            If Not names.Any(Function(id) id.Name Like KO) Then
                Call $"Skip {map.Name}".__INFO_ECHO
                Continue For
            End If

            clusters += New Cluster With {
                .description = map.Name,
                .ID = map.id,
                .names = map.Name,
                .members = names _
                    .Where(Function(a) a.Name.IsPattern("[CDG]\d+")) _
                    .Select(Function(c)
                                Return New BackgroundGene With {
                                    .name = c.Value,
                                    .accessionID = c.Name,
                                    .[alias] = {c.Name},
                                    .locus_tag = New NamedValue(c),
                                    .term_id = {c.Name}
                                }
                            End Function) _
                    .ToArray
            }
        Next

        backgroundSize = clusters.BackgroundSize

        Return New Background With {
            .build = Now,
            .clusters = clusters,
            .comments = "Background model apply for GSEA of LC-MS metabolism analysis, created by GCModeller.",
            .name = "KEGG reference maps",
            .size = backgroundSize,
            .id = "reference"
        }
    End Function

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
