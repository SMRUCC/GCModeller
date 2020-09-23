#Region "Microsoft.VisualBasic::cd8e1152fce8c4261f674812af3daec1, data\ExternalDBSource\DEG\Web\WebParser.vb"

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

    '     Module WebParser
    ' 
    '         Function: fillDetails, GetGenomeList, getSummary, getTotalPages, parseDEGList
    '                   (+2 Overloads) ParseDEGList
    ' 
    '         Sub: ParserWorkflow
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace DEG.Web

    Public Module WebParser

        Const dataAPI As String = "http://origin.tubic.org/deg/public/index.php/browse/bacteria"
        Const summaryApi$ = "http://origin.tubic.org/deg/public/index.php/organism/bacteria/%s.html"

        Public Sub ParserWorkflow(save As String)
            Dim genomes = GetGenomeList().ToArray
            Dim cache$ = $"{save}/.essentialgene.org"
            Dim web As New WebQuery(Of Genome)(Function(genome) sprintf(summaryApi, genome.ID), Function(g) g.ID, Function(s, type) s, Nothing, cache)

            For Each genome As Genome In genomes
                ' summary + first page
                Dim html$ = web.Query(Of String)(genome, "*.html")
                Dim saveXml$ = $"{save}/{genome.Organism.NormalizePathString}.Xml"
                Dim internalCache$ = $"{cache}/{genome.Organism.NormalizePathString}"

                genome.summary = html.getSummary
                genome.EssentialGenes = genome.ParseDEGList(html, internalCache).ToArray
                genome.GetXml.SaveTo(saveXml)
            Next
        End Sub

        <Extension>
        Private Function getSummary(html As String) As Summary
            Dim summaryTable = html.GetTablesHTML.First
            Dim rows = summaryTable.GetRowsHTML
            Dim meta = rows _
                .Select(Function(r) r.GetColumnsHTML) _
                .ToDictionary(Function(r)
                                  Return r(0).StripHTMLTags _
                                      .NormalizePathString _
                                      .Replace(" ", "_")
                              End Function,
                              Function(r) r(1).StripHTMLTags)
            Dim summary As New Summary

            Call meta.GetJson.__DEBUG_ECHO

            With meta
                summary.Backup = !Backup
                summary.EssentialGenes = !Essential_Genes
                summary.Medium = !Medium
                summary.Method = !Method
                summary.Pubmed = !Pubmed
                summary.Reference = !Reference
                summary.RefSeq = !RefSeq
                summary.Organism = !Organism
            End With

            Return summary
        End Function

        Public Iterator Function GetGenomeList() As IEnumerable(Of Genome)
            Dim table = dataAPI.GET.GetTablesHTML.FirstOrDefault

            If table.StringEmpty Then
                Return
            End If

            Dim rows = table.GetRowsHTML

            For Each row As String In rows.Skip(1)
                Dim columns = row.GetColumnsHTML

                Yield New Genome With {
                    .ID = columns(2).href.BaseName,
                    .Organism = columns(1).StripHTMLTags,
                    .numOfDEG = columns(2).StripHTMLTags,
                    .Conditions = columns(3),
                    .Reference = columns(4)
                }
            Next
        End Function

        Const listAPI$ = "http://origin.tubic.org/deg/public/index.php/query/bacteria/degac/%s.html?lineage=bacteria&field=degac&term=%s&page=%s"

        <Extension>
        Public Function ParseDEGList(genome As Genome) As IEnumerable(Of EssentialGene)
            Return genome.ParseDEGList(sprintf(listAPI, genome.ID, genome.ID, 1).GET)
        End Function

        Private Function getTotalPages(html As String) As Integer
            Dim alist = html.Matches("<a.+?</a>").ToArray
            Dim pagelink = alist.Where(Function(a) a.class = "page-link").ToArray
            Dim spans = pagelink.Last.Matches("<span.+?</span>").ToArray
            Dim textPrimary = spans.Where(Function(s) s.class = "text-primary").ToArray

            Return textPrimary.Last.StripHTMLTags
        End Function

        <Extension>
        Private Iterator Function ParseDEGList(genome As Genome, html As String, Optional cache$ = "./") As IEnumerable(Of EssentialGene)
            Dim allPages As Integer = getTotalPages(html)
            Dim web As New WebQuery(Of Integer)(Function(page) sprintf(listAPI, genome.ID, genome.ID, page), Function(page) CStr(page), Function(s, type) s, cache:=cache)
            Dim detailsCache$ = $"{cache}/details/"

            For Each gene As EssentialGene In html.parseDEGList(detailsCache)
                Yield gene
            Next

            For Each geneList In web.Query(Of String)(Enumerable.Range(2, allPages), "*.html")
                For Each gene In geneList.parseDEGList(detailsCache)
                    Yield gene
                Next
            Next
        End Function

        Const detailsAPI As String = "http://origin.tubic.org/deg/public/index.php/information/bacteria/%s.html"

        <Extension>
        Private Iterator Function parseDEGList(html As String, cache$) As IEnumerable(Of EssentialGene)
            Dim table$ = html.GetTablesHTML.Last
            Dim rows = table.GetRowsHTML
            Dim web As New WebQuery(Of EssentialGene)(Function(g) sprintf(detailsAPI, g.ID), Function(g) g.ID, Function(s, type) s, Nothing, cache)
            Dim parseList = Iterator Function() As IEnumerable(Of EssentialGene)
                                For Each row As String In rows.Skip(1)
                                    Dim columns = row.GetColumnsHTML

                                    Yield New EssentialGene With {
                                        .ID = columns(1).StripHTMLTags,
                                        .Name = columns(2),
                                        .FunctionDescrib = columns(3),
                                        .Organism = columns(4)
                                    }
                                Next
                            End Function

            For Each gene As EssentialGene In parseList()
                Dim details As String = web.Query(Of String)(gene, "*.html")

                Yield gene.fillDetails(details)
            Next
        End Function

        <Extension>
        Private Function fillDetails(gene As EssentialGene, html$) As EssentialGene
            Dim table = html.GetTablesHTML.FirstOrDefault

            If table.StringEmpty Then
                Return gene
            End If

            Dim rows = table.GetRowsHTML
            Dim details = rows.Skip(1) _
                .Select(Function(r) r.GetColumnsHTML) _
                .ToDictionary(Function(c)
                                  Return c.First.Replace(" ", "_")
                              End Function,
                              Function(c) c.Last)

            With details
                gene.geneRef = !Gene_Ref
                gene.RefSeq = !RefSeq
                gene.UniProt = !Uniprot_accession
                gene.COG = !COG
                gene.GO = !GO_annotation.Split
                gene.FuncClass = !Function_Class
                gene.Condition = !Condition
                gene.Reference = !Reference
                gene.Nt = !Nucleotide_sequence.paragraph.FirstOrDefault.StripHTMLTags
                gene.Aa = !Amino_acid_sequence.paragraph.FirstOrDefault.StripHTMLTags
            End With

            Return gene
        End Function
    End Module
End Namespace
