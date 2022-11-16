#Region "Microsoft.VisualBasic::8f1466aa96142c0614133afb49a3514a, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\API.vb"

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

    '   Total Lines: 262
    '    Code Lines: 195
    ' Comment Lines: 27
    '   Blank Lines: 40
    '     File Size: 12.55 KB


    '     Module API
    ' 
    '         Function: __entryParser, __genesParser, __queryEntryParser, __xRefParser, CutSequence
    '                   HandleDownload, HandleDownloads, HandleQuery, Query, QueryURL
    '                   (+2 Overloads) Transform, xRefParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' 在KEGG之中的直系同源数据
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("KEGG.DBGET.SSDB", Publisher:="xie.guigang@gmail.com",
                      Category:=APICategories.UtilityTools,
                      Description:="KEGG SSDB (Sequence Similarity DataBase) contains the information about amino acid sequence similarities among all protein-coding genes in the complete genomes, which is computationally generated from the GENES database in KEGG. All possible pairwise genome comparisons are performed by the SSEARCH program, and the gene pairs with the Smith-Waterman similarity score of 100 or more are entered in SSDB, together with the information about best hits and bidirectional best hits (best-best hits). SSDB is thus a huge weighted, directed graph, which can be used for searching orthologs and paralogs, as well as conserved gene clusters with additional consideration of positional correlations on the chromosome. 

The relationship of gene x in genome A and gene y in genome B is defined as follows:
forward best:
reverse best:
best-best:	x is compared against all genes in genome B and y is found as top-scoring
y is compared against all genes in genome A and x is found as top-scoring
both of these relationships hold
(Note) The option to search reverse best hits is discontinued; ""forward best"" is now simply called ""best"".",
                      Url:="http://www.kegg.jp/kegg/ssdb/")>
    Public Module API

        Const ORTHOLOGY_URL_QUERY As String = "http://www.genome.jp/dbget-bin/www_bfind_sub?mode=bfind&max_hit=1000&locale=en&serv=kegg&dbkey=orthology&keywords={0}&page=1"

        <ExportAPI("Query")>
        Public Function HandleQuery(GeneName As String) As KEGG.WebServices.QueryEntry()
            Dim html As String = String.Format(ORTHOLOGY_URL_QUERY, GeneName).GET
            Return __queryEntryParser(html)
        End Function

        Private Function __queryEntryParser(html As String) As QueryEntry()
            Dim matches = Regex.Matches(html, QUERY_RESULT_LINK_ITEM)

            If matches.Count = 0 Then Return New QueryEntry() {}

            Dim Result As QueryEntry() = New QueryEntry(matches.Count - 1) {}

            For i As Integer = 0 To Result.Count - 1
                Dim match = matches(i).Value
                Dim Description As String = Regex.Match(match, QUERY_RESULT_LINK1).Value,
                    value As String

                value = Regex.Match(match, QUERY_RESULT_LINK2).Value
                value = Mid(value, 2, Len(value) - 2)

                Description = Mid(Description, 2)
                Description = Mid(Description, 1, Len(Description) - 1)

                Result(i) = New QueryEntry With {
                    .description = Description,
                    .locusID = value,
                    .speciesID = "ko"
                }
            Next

            Return Result
        End Function

        Const ORTHOLOGY_WEBFORM As String = "http://www.genome.jp/dbget-bin/www_bget?ko:{0}"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="EntryList">The entry point list of the kegg orthology.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("HandleDownloads")>
        Public Function HandleDownloads(<Parameter("List.Entry", "The entry point list of the kegg orthology.")>
                                        EntryList As IEnumerable(Of QueryEntry)) As QueryEntry()
            Dim LQuery = (From EntryPoint As QueryEntry
                          In EntryList
                          Select HandleDownload(EntryPoint.locusID)).ToArray
            Return LQuery.ToVector
        End Function

        Const GENE_ENTRY As String = "<a href=""/dbget-bin/www_bget.+?>.+?</a>" '.*?<a"
        Const GENE_RECORD As String = "<tr>.+?</tr>"
        Const GENE_LINK As String = "<td>.+?<[/]td>"

        'PRP: <a href="/dbget-bin/www_bget?prp:M062_18515">M062_18515</a>

        ''' <summary>
        ''' 返回基因列表，之后可以使用这个基因列表来进行蛋白质或者核酸序列的下载
        ''' </summary>
        ''' <param name="KO_ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("HandleDownload")>
        Public Function HandleDownload(KO_ID As String) As QueryEntry()
            Dim URL As String = String.Format(ORTHOLOGY_WEBFORM, KO_ID)
            Dim WebForm As New WebForm(URL)
            Return __genesParser(WebForm, KO_ID)
        End Function

        <ExportAPI("Query")>
        Public Function Query(ko As String) As Orthology
            Dim url As String = String.Format(SSDB.API.ORTHOLOGY_WEBFORM, ko)
            Return API.QueryURL(url)
        End Function

        <ExportAPI("Query.From.URL")>
        Public Function QueryURL(url As String) As Orthology
            Dim WebForm As New WebForm(url)
            Dim Orthology As New SSDB.Orthology With {
                .Entry = WebForm.GetValue("Entry").FirstOrDefault
            }

            If Not String.IsNullOrEmpty(Orthology.Entry) Then
                Orthology.Entry = Regex.Match(Orthology.Entry, "[A-Z]+\d{5}").Value
            End If

            Orthology.xref = New OrthologyTerms With {
                .Terms = xRefParser(WebForm.GetRaw("Other DBs"))
            }
            Orthology.references = WebForm.References
            Orthology.modules = PathwayWebParser.__parseHTML_ModuleList(WebForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module)
            Orthology.Definition = WebForm.GetValue("Definition").FirstOrDefault.StripHTMLTags
            Orthology.Name = WebForm.GetValue("Name").FirstOrDefault.TrimNewLine.Trim
            Orthology.pathway = PathwayWebParser.__parseHTML_ModuleList(WebForm.GetValue("Pathway").FirstOrDefault, LIST_TYPES.Pathway)
            Orthology.disease = PathwayWebParser.__parseHTML_ModuleList(WebForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease)
            Orthology.genes = __genesParser(WebForm, Orthology.Entry)
            Orthology.Name = Orthology.Name.StripHTMLTags.GetTagValue(, True).Value
            Orthology.Definition = Orthology.Definition.StripHTMLTags.TrimNewLine.Trim.GetTagValue(, True).Value
            Orthology.EC = Regex.Match(Orthology.Definition, "\[EC.+?\]", RegexOptions.IgnoreCase).Value

            If Not String.IsNullOrEmpty(Orthology.EC) Then
                Orthology.EC = Mid(Orthology.EC, 5, Len(Orthology.EC) - 5)
            End If

            Return Orthology
        End Function

        Const xRef As String = "<div style=""float:left"">.+?</div><div.*?</div>"

        Public Function xRefParser(str As String()) As [Property]()
            If str.IsNullOrEmpty Then
                Return {}
            End If
            Dim DBs As String() = (From ss As String
                                   In str
                                   Select (From m As Match
                                           In Regex.Matches(ss, xRef, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
                                           Select m.Value)).ToVector
            Dim Values = DBs.Select(Function(lnk) __xRefParser(lnk)).ToVector
            Return Values
        End Function

        Private Function __xRefParser(lnk As String) As [Property]()
            Dim Name As String = Regex.Match(lnk, "<div.+?</div>").Value.GetValue.Split(":"c).First
            Dim Links = (From m As Match In Regex.Matches(lnk, "<a.+?</a>") Select m.Value).ToArray
            Dim values As [Property]() = Links _
                .Select(Function(ss)
                            Return New [Property] With {
                                .name = Name,
                                .value = ss.href,
                                .Comment = ss.GetValue
                            }
                        End Function) _
                .ToArray

            Return values
        End Function

        <ExportAPI("getHits")>
        Public Function Transform(result As SSDB.OrthologREST) As SSDB.Ortholog()
            Return SSDB.Ortholog.CreateObjects(result)
        End Function

        <ExportAPI("ImportsDB")>
        Public Function Transform(<Parameter("source.DIR")> source As String) As SSDB.Ortholog()
            Dim Xmls As IEnumerable(Of String) = ls - l - wildcards("*.xml") <= source
            Dim LQuery = (From xml As String
                          In Xmls.AsParallel
                          Let result As SSDB.OrthologREST = xml.LoadXml(Of SSDB.OrthologREST)
                          Select SSDB.Ortholog.CreateObjects(result)).ToVector
            Return LQuery
        End Function

        ''' <summary>
        ''' <paramref name="vector"/>参数表示核酸链的方向，1表示正义链，-1表示互补链
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="org$"></param>
        ''' <param name="vector$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CutSequence(loci As Location, org$, Optional vector% = 1) As FastaSeq
            With loci.Normalization
                Dim url$ = $"http://www.genome.jp/dbget-bin/cut_sequence_genes.pl?FROM={ .Left}&TO={ .Right}&VECTOR={vector}&ORG={org}"
                Dim html$ = url.GET
                Dim seq$

                seq = Regex.Match(html, "<PRE>.+</PRE>", RegexICSng).Value
                seq = seq _
                    .StripHTMLTags _
                    .StripBlank _
                    .LineTokens _
                    .Skip(1) _
                    .JoinBy("")

                Return New FastaSeq With {
                    .Headers = {
                        url
                    },
                    .SequenceData = seq
                }
            End With
        End Function
    End Module
End Namespace
