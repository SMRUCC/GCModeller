#Region "Microsoft.VisualBasic::a708fa6475441a1dee263a95537a60e8, meme_suite\MEME.DocParser\MAST\DocumentParser.vb"

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

    '     Module DocumentParser
    ' 
    '         Function: __createMEMEOutPut, __footPrintMatched, __getSiteId, __matchedResult, LoadDocument
    '                   LoadDocument_v410, MatchMEMEAndMast, SaveMastCsv, SaveMastXml, SaveMatches
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml

Namespace DocumentFormat.MAST.HTML

    <Package("MEME.MAST", Description:="Document parser for the mast html output.", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module DocumentParser

        ''' <summary>
        ''' 较老版本的MAST程序输出的解析函数
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="FootPrintMode">当本参数为真的时候，表明为footprint模式，则会将所有的匹配位点列出来</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Mast.Load_HTML", Info:="Old version of mast program output parser")>
        Public Function LoadDocument(url As String, <Parameter("Mode.Footprint")> FootPrintMode As Boolean) As MASTHtml
            Dim PageContent As String = Strings.Split(url.GET(), "<h4>Top Scoring Sequences <a").Last
            Dim MAST As MASTHtml = New MASTHtml
            Dim InputData As String = Regex.Match(PageContent, "<form>.+?</form>", RegexOptions.Singleline).Value
            PageContent = Regex.Match(PageContent, "<tbody>.+?</tbody>", RegexOptions.Singleline).Value
            Dim Tokens As String() = (From match As Match In Regex.Matches(PageContent, "<tr .+?</tr>", RegexOptions.Singleline) Select match.Value).ToArray
            Dim Matches As List(Of MatchedSite) = New List(Of MatchedSite)

            For Each Token As String In Tokens
                Dim result = MatchedSite.TryParse(Token)
                If Not result.IsNullOrEmpty Then Call Matches.AddRange(result)
            Next

            MAST.MatchedSites = Matches.ToArray

            Dim MASTData = If(FootPrintMode, __footPrintMatched(MAST, InputData), __matchedResult(MAST, InputData))
            Return MASTData
        End Function

        ''' <summary>
        ''' MAST version 4.10.0 (Release date: Wed May 21 10:35:36 2014 +1000)
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="FootPrintMode">当本参数为真的时候，表明</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("MAST410.Load_Html", Info:="MAST version 4.10.0 (Release date: Wed May 21 10:35:36 2014 +1000)")>
        Public Function LoadDocument_v410(url As String, <Parameter("Mode.Footprints")> FootPrintMode As Boolean) As MASTHtml
            Dim PageContent As String = Strings.Split(url.GET(), "<h4>Top Scoring Sequences <a").Last
            Dim InputData As String = Regex.Match(PageContent, "<form>.+?</form>", RegexOptions.Singleline).Value
            Dim ChunkBuffer As String() = (From m As Match
                                           In Regex.Matches(PageContent, "<tbody>.+?</tbody>", RegexOptions.Singleline)
                                           Select m.Value).ToArray
            PageContent = ChunkBuffer.Last
            Dim Tokens As String() = (From match As Match
                                      In Regex.Matches(PageContent, "<tr .+?</tr>", RegexOptions.Singleline)
                                      Select match.Value).ToArray
            Dim Matches As List(Of MatchedSite) = New List(Of MatchedSite)

            For Each Token As String In Tokens
                Dim result = MatchedSite.TryParse(Token)
                If Not result.IsNullOrEmpty Then Call Matches.AddRange(result)
            Next

            Dim MAST As MASTHtml = New MASTHtml With {
                .MatchedSites = Matches.ToArray
            }
            Dim MASTData As MASTHtml = If(FootPrintMode,
                __footPrintMatched(MAST, InputData),
                __matchedResult(MAST, InputData))
            Return MASTData
        End Function

        Private Function __footPrintMatched(MastHtml As MASTHtml, Input As String) As MASTHtml
            Dim hits = (From m As Match
                        In Regex.Matches(Input, "<input type=""hidden"" id=""seq_\d+_\d+_hits"" value="".+?"">", RegexOptions.Singleline)
                        Let Tokens As String() = Strings.Split(m.Value, vbLf).Skip(1).ToArray
                        Let lcl As String = m.Value.GetXmlAttrValue("id")
                        Select lcl,
                            Tokens.Take(Tokens.Length - 1).ToArray).ToArray
            Dim FootprintHitsTable = (From strValue As String
                                      In hits.First.ToArray
                                      Let Tokens As String() = Strings.Split(strValue, vbTab)
                                      Select Starts = Tokens(0),
                                          MotifId = CInt(Tokens(1).Split(CChar("_")).Last),
                                          Strand = Tokens(2),
                                          pValue = Tokens(3),
                                          Hit = Tokens(4)).ToArray
            Dim Footprint_Dict As Dictionary(Of Integer, Dictionary(Of Integer, MatchedSite)) =
                New Dictionary(Of Integer, Dictionary(Of Integer, MatchedSite))

            For Each MotifId As Integer In (From item In FootprintHitsTable Let id As Integer = item.MotifId Select id Distinct)
                Call Footprint_Dict.Add(MotifId, New Dictionary(Of Integer, MatchedSite))
            Next

            For Each site In MastHtml.MatchedSites
                Call Footprint_Dict(site.MotifId).Add(site.Starts, site)
            Next

            For Each hit In FootprintHitsTable
                Dim sites = Footprint_Dict(hit.MotifId)
                Dim site = sites(hit.Starts)
                site.Strand = hit.Strand
            Next

            Return MastHtml
        End Function

        Private Function __matchedResult(MastHtml As MASTHtml, Input As String) As MASTHtml
            Dim Hits = (From m As Match
                        In Regex.Matches(Input, "<input type=""hidden"" id=""seq_\d+_\d+_hits"" value="".+?"">", RegexOptions.Singleline)
                        Let Tokens As String() = Strings.Split(m.Value, vbLf).Skip(1).ToArray
                        Let lcl As String = m.Value.GetXmlAttrValue("id")
                        Select lcl, Tokens.Take(Tokens.Length - 1).ToArray).ToArray

            If Hits.IsNullOrEmpty Then Return MastHtml
            If Hits.Length = 1 Then 'FootPrint模式，只有全基因组一条序列
                Return __footPrintMatched(MastHtml, Input)
            End If

            Dim HitsTable = (From Value In Hits
                             Select hitsValue = (From strValue As String
                                                 In Value.ToArray
                                                 Let Tokens As String() = Strings.Split(strValue, vbTab)
                                                 Select Starts = Tokens(0),
                                                     MotifId = CInt(Tokens(1).Split(CChar("_")).Last),
                                                     Strand = Tokens(2),
                                                     pValue = Tokens(3),
                                                     Hit = Tokens(4)).ToArray,
                                 lcl = CInt(Val(Regex.Match(Value.lcl, "\d+_hits").Value))).ToArray

            Dim Get_Sitelcls = (From site As MatchedSite
                                In MastHtml.MatchedSites
                                Select siteId = __getSiteId(site.SequenceId), site).ToArray
            Dim MAST_lcl = (From siteId As Integer
                            In (From item In Get_Sitelcls Select item.siteId Distinct).ToArray
                            Let sites As MatchedSite() = (From site In Get_Sitelcls Where siteId = site.siteId Select site.site).ToArray
                            Select siteId, sites).ToDictionary(Function(item) item.siteId.ToString)

            For Each site In HitsTable

                Dim lcl As String = site.lcl.ToString

                If Not MAST_lcl.ContainsKey(lcl) Then
                    Continue For
                End If

                Dim Dict As Dictionary(Of Integer, Dictionary(Of Integer, MatchedSite)) =
                    New Dictionary(Of Integer, Dictionary(Of Integer, MatchedSite))

                For Each MotifId As Integer In (From item As MatchedSite
                                                In MAST_lcl(lcl).sites
                                                Let id As Integer = item.MotifId
                                                Select id
                                                Distinct).ToArray
                    Call Dict.Add(MotifId, New Dictionary(Of Integer, MatchedSite))
                Next

                For Each Matched_Site As MatchedSite In MAST_lcl(lcl).sites
                    Call Dict(Matched_Site.MotifId).Add(Matched_Site.Starts, Matched_Site)
                Next

                For Each hit In site.hitsValue
                    If Not Dict.ContainsKey(hit.MotifId) Then
                        Continue For
                    End If

                    Dim sites As Dictionary(Of Integer, MatchedSite) = Dict(hit.MotifId)
                    If Not sites.ContainsKey(hit.Starts) Then
                        Continue For
                    End If
                    Dim Matched_Site As MatchedSite = sites(hit.Starts)
                    Matched_Site.Strand = hit.Strand
                Next
            Next

            Return MastHtml
        End Function

        Private Function __getSiteId(s As String) As Integer
            Return CInt(Regex.Match(s, "lcl\.\d+").Value.Split(CChar(".")).Last)
        End Function

        <ExportAPI("MAST.Match.MEME")>
        Public Function MatchMEMEAndMast(MEME As MEME.HTML.MEMEHtml, MAST As MASTHtml) As MEME.HTML.MEMEOutput()
            Dim Motifs As Dictionary(Of Integer, MEME.HTML.Motif) =
                MEME.Motifs.ToDictionary(Function(item As MEME.HTML.Motif) item.Id)
            Dim LQuery As MEME.HTML.MEMEOutput()() = (From motif As MEME.HTML.Motif
                                                     In MEME.Motifs
                                                      Select __createMEMEOutPut(MEME.ObjectId, motif, MAST)).ToArray
            Dim result As MEME.HTML.MEMEOutput() = LQuery.ToVector
            Return result
        End Function

        <ExportAPI("Write.Csv.MEME_Matches")>
        Public Function SaveMatches(data As IEnumerable(Of MEME.HTML.MEMEOutput), <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        Private Function __createMEMEOutPut(ObjectId As String,
                                            motif As MEME.HTML.Motif,
                                            sites As MASTHtml) As MEME.HTML.MEMEOutput()
            Dim LQuery = (From site As MatchedSite
                          In sites.MatchedSites
                          Where site.MotifId = motif.Id
                          Select New MEME.HTML.MEMEOutput With {
                .Ends = site.Ends,
                .Evalue = motif.Evalue,
                .Id = site.MotifId,
                .InformationContent = motif.InformationContent,
                .LogLikelihoodRatio = motif.LogLikelihoodRatio,
                .MAST_EValue = site.EValue,
                .MAST_PValue = site.PValue,
                .ObjectId = ObjectId,
                .MatchedMotif = ObjectId & "." & motif.Id,
                .RegularExpression = motif.RegularExpression,
                .RelativeEntropy = motif.RelativeEntropy,
                .Name = site.SequenceId,
                .Start = site.Starts,
                .Strand = site.Strand,
                .Width = motif.Width}).ToArray
            Return LQuery
        End Function

        Const MOTIF_BLOCKS As String = "<div class=""block_motif"" style="".+?"" title="".+?""></div>"

        <ExportAPI("Write.Csv.MAST")>
        Public Function SaveMastCsv(data As MASTHtml, <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.MatchedSites.SaveTo(SaveTo, False)
        End Function

        <ExportAPI("Write.Xml.MAST")>
        Public Function SaveMastXml(data As MASTHtml, <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.GetXml.SaveTo(SaveTo)
        End Function
    End Module
End Namespace
