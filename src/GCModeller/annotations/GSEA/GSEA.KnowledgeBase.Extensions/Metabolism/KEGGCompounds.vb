#Region "Microsoft.VisualBasic::dd502f615ae81b4866aaacbc71cf4228, annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\KEGGCompounds.vb"

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

    '   Total Lines: 139
    '    Code Lines: 103 (74.10%)
    ' Comment Lines: 23 (16.55%)
    '    - Xml Docs: 82.61%
    ' 
    '   Blank Lines: 13 (9.35%)
    '     File Size: 5.50 KB


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
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports Pathway = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway

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
    ''' a indexer for do map selection, which means only the KEGG maps 
    ''' that contains the symbols of these KO id then will be selected 
    ''' for create the background cluster model.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGeneralBackground(Of T As Map)(maps As IEnumerable(Of T),
                                                         Optional KO As Index(Of String) = Nothing,
                                                         Optional mapIdPattern$ = "map\d+") As Background

        ' The total number of metabolites in background genome. 
        Dim backgroundSize% = 0
        Dim clusters As New List(Of Cluster)
        Dim names As NamedValue(Of String)()
        Dim members As BackgroundGene()
        Dim ko00001 = BriteHText.Load_ko00001.Deflate(mapIdPattern).ToArray
        Dim pathway_class = ko00001 _
            .GroupBy(Function(gene)
                         Return gene.subcategory.Split.First.ParseInteger.ToString
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.First
                          End Function)

        If KO Is Nothing Then
            KO = New String() {}
        End If

        For Each map As T In maps
            names = map.shapes.mapdata _
                .Select(Function(a) a.Names) _
                .IteratesALL _
                .GroupBy(Function(n) n.Name) _
                .Select(Function(duplicated)
                            ' the id data has been removes duplicated at here
                            Return duplicated.First
                        End Function) _
                .ToArray

            If KO.Count > 0 AndAlso Not names.Any(Function(id) id.Name Like KO) Then
                Call VBDebugger.EchoLine($"Skip {map.name}")
                Continue For
            Else
                members = names _
                    .Where(Function(a) a.Name.IsPattern("[CDG]\d+")) _
                    .Select(Function(c)
                                Return New BackgroundGene With {
                                    .name = c.Value,
                                    .accessionID = c.Name,
                                    .[alias] = {c.Name},
                                    .locus_tag = New NamedValue(c),
                                    .term_id = BackgroundGene.UnknownTerms(c.Name).ToArray
                                }
                            End Function) _
                    .ToArray

                ' skip of the empty cluster?
                If members.IsNullOrEmpty Then
                    Continue For
                End If
            End If

            Dim term As New Cluster With {
                .description = map.name,
                .ID = map.EntryId,
                .names = map.name,
                .members = members
            }
            Dim int As String = map.EntryId.Match("\d+").ParseInteger.ToString

            If pathway_class.ContainsKey(int) Then
                Dim label = pathway_class(int)

                term.class = label.class
                term.category = label.category
            End If

            clusters += term
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
                                    .term_id = BackgroundGene.UnknownTerms(c.name).ToArray
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
