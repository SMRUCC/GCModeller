#Region "Microsoft.VisualBasic::e4971e89386973168f642fe06a4c74c3, analysis\ProteinTools\ProteinTools.Interactions\SwissTCS\SwissRegulon.vb"

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

    '     Module SwissRegulon
    ' 
    '         Function: __getCrossTalk, __getOptions, __trim, CreateCrossTalks, CrossTalk
    '                   Download, DownloadTcsSequence, GetScores, GetSpeciesList, GetTCSList
    ' 
    '         Sub: __downloads, Download
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SwissTCS

    <Package("swiss-regulon.TCS.pl",
                    Category:=APICategories.ResearchTools,
                    Description:="Burger, L. and E. van Nimwegen (2008). 
                    ""Accurate prediction Of protein-protein interactions from sequence alignments Using a Bayesian method."" 
                    Mol Syst Biol 4: 165.
	<p>Accurate and large-scale prediction of protein-protein interactions directly from amino-acid sequences is one of the great challenges in computational biology. 
                    Here we present a new Bayesian network method that predicts interaction partners using only multiple alignments of amino-acid sequences of interacting 
                    protein domains, without tunable parameters, and without the need for any training examples. We first apply the method to bacterial two-component systems 
                    and comprehensively reconstruct two-component signaling networks across all sequenced bacteria. Comparisons of our predictions with known interactions 
                    show that our method infers interaction partners genome-wide with high accuracy. To demonstrate the general applicability of our method we show that 
                    it also accurately predicts interaction partners in a recent dataset of polyketide synthases. Analysis of the predicted genome-wide two-component 
                    signaling networks shows that cognates (interacting kinase/regulator pairs, which lie adjacent on the genome) and orphans (which lie isolated) form 
                    two relatively independent components of the signaling network in each genome. In addition, while most genes are predicted to have only a small 
                    number of interaction partners, we find that 10% of orphans form a separate class of 'hub' nodes that distribute and integrate signals to and 
                    from up to tens of different interaction partners.</p>",
                    Url:="http://swissregulon.unibas.ch/cgi-bin/TCS.pl",
                    Publisher:="erik.vannimwegen@unibas.ch")>
    Public Module SwissRegulon

        Const ENTRY_URL As String = "http://swissregulon.unibas.ch/cgi-bin/TCS.pl"
        Const [OPTION] As String = "<option value=""[a-z0-9_]+"">"
        Const SECTION As String = "<select name=""bacterium"">.+</select>"

        <ExportAPI("Get.Species", Info:="Downloads the bacterial species genome list from the WebAPI")>
        Public Function GetSpeciesList() As String()
            Dim webPage As String = ENTRY_URL.GET
            webPage = Regex.Match(webPage, SECTION, RegexOptions.Singleline).Value
            Return __getOptions(webPage)
        End Function

        ''' <summary>
        ''' {0} [kinase][receiver] 
        ''' {1} gene_id
        ''' {2} speciesId
        ''' </summary>
        ''' <remarks></remarks>
        Const SCORE As String = "http://swissregulon.unibas.ch/cgi-bin/TCS.pl?{0}={1}&bacterium={2}"
        Const SECTION_START As String = "<form method=""POST"" action=""get_scores2.pl"">"
        Const tblITEM As String = "<td>.+?</td>"
        Const VALUE__ As String = """>.+</font>"

        <ExportAPI("Get.TCS.Scores")>
        Public Function GetScores(geneId As String, speciesId As String, component As TCSComponentTypes) As CrossTalks()
            Dim Url As String = String.Format(SCORE, component.ToString, geneId, speciesId)
            Dim WebPage As String = Url.GET
            WebPage = Mid(WebPage, InStr(WebPage, SECTION_START) + 200)

            Dim items As String() = (From m As Match
                                     In Regex.Matches(WebPage, tblITEM, RegexOptions.IgnoreCase + RegexOptions.Singleline)
                                     Select m.Value).ToArray
            Dim chunkBuffer As String() = New String(2) {}
            Dim lstCTk As List(Of CrossTalks) = New List(Of CrossTalks)

            For i As Integer = 0 To items.Count - 1 Step 3
                Call Array.ConstrainedCopy(items, i, chunkBuffer, 0, 3)
                Call lstCTk.Add(__getCrossTalk(chunkBuffer))
            Next

            Return lstCTk.ToArray
        End Function

        Private Function __getCrossTalk(chunkBuffer As String()) As CrossTalks
            Dim sHisk As String = Regex.Match(chunkBuffer(0), VALUE__, RegexOptions.IgnoreCase).Value
            Dim RR As String = Regex.Match(chunkBuffer(1), VALUE__, RegexOptions.IgnoreCase).Value
            Dim pp As String = chunkBuffer(2)

            sHisk = Mid(sHisk, 3, Len(sHisk) - 10).Trim
            RR = Mid(RR, 3, Len(RR) - 10).Trim
            pp = Mid(pp, 5, Len(pp) - 10).Trim

            Dim CTk As New CrossTalks With {
                .Kinase = sHisk,
                .Regulator = RR,
                .Probability = Val(pp)
            }

            Return CTk
        End Function

        Private Function __getOptions(strText As String) As String()
            Dim matches = Regex.Matches(strText, [OPTION], RegexOptions.Singleline + RegexOptions.IgnoreCase)
            Dim items = (From m As Match In matches Select m.Value).ToArray
            items = (From str In items Select Regex.Match(str, """.+""").Value.Replace("""", "")).ToArray
            Return items
        End Function

        Const OPTGROUP As String = "<optgroup label=""[a-z]+"">.+?</optgroup>"

        <ExportAPI("Get.Tcs.System")>
        Public Function GetTCSList(speciesId As String) As String()()
            Dim url = String.Format(SCORE, TCSComponentTypes.kinase.ToString, "geneId", speciesId)
            Dim pageContent As String = url.GET
            Dim Groups = Regex.Matches(pageContent, OPTGROUP, RegexOptions.Singleline)
            Dim returnList = (From mi As Match In Groups Select __getOptions(mi.Value)).ToArray
            Return returnList
        End Function

        <ExportAPI("Tcs.Downloads")>
        Public Function Download(<Parameter("Name.Species", "One of the element value from the API function Get.Species")> species As String,
                                 <Parameter("DIR.Export")> ExportDIR As String) As Boolean
            Dim sysComponents As String()() = SwissRegulon.GetTCSList(species)

            If sysComponents.IsNullOrEmpty Then Return False

            Dim SK As String() = sysComponents(0)
            Dim RR As String() = sysComponents(1)
            Dim skDir As String = String.Format("{0}/{1}/HisK/", ExportDIR, species)
            Dim rrDir As String = String.Format("{0}/{1}/RR/", ExportDIR, species)

            Call FileIO.FileSystem.CreateDirectory(skDir)
            Call FileIO.FileSystem.CreateDirectory(rrDir)
            Call __downloads(SK, RR, skDir, rrDir, species)

            Return CreateCrossTalks(skDir, rrDir).Save($"{ExportDIR}/{species}.csv", False)
        End Function

        Private Sub __downloads(SK As String(), RR As String(), skDIR As String, RrDIR As String, species As String)
            For Each locusTag As String In SK
                Dim File As String = skDIR & locusTag & ".csv"

                If Not File.FileExists Then
                    Call SwissRegulon.GetScores(locusTag, species, TCSComponentTypes.kinase).SaveTo(File, False)
                    Call Threading.Thread.Sleep(3)
                End If
            Next

            For Each locusTag As String In RR
                Dim File As String = RrDIR & locusTag & ".csv"

                If Not File.FileExists Then
                    Call SwissRegulon.GetScores(locusTag, species, TCSComponentTypes.receiver).SaveTo(File, False)
                    Call Threading.Thread.Sleep(3)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 下载完整的互作数据库
        ''' </summary>
        ''' <param name="ExportDIR"></param>
        <ExportAPI("Download.Db.Entire")>
        Public Sub Download(ExportDIR As String)
            For Each id As String In SwissRegulon.GetSpeciesList
                Call Download(species:=id, ExportDIR:=ExportDIR)
            Next
        End Sub

        <ExportAPI("Matrix.CrossTalks")>
        Public Function CreateCrossTalks(dirHisK As String, dirRR As String) As IO.File
            Dim HisKList = (From Path As String In FileIO.FileSystem.GetFiles(dirHisK) Select Path.LoadCsv(Of CrossTalks)(False).ToArray).ToArray.ToVector
            Dim RRList = (From Path As String In FileIO.FileSystem.GetFiles(dirRR) Select Path.LoadCsv(Of CrossTalks)(False).ToArray).ToArray.ToVector
            Dim ChunkBuffer As CrossTalks() = {HisKList, RRList}.ToVector

            Call $"{HisKList.Length} hisK and {RRList.Length} respone regulator...".__DEBUG_ECHO

            Dim skIdList = (From item As CrossTalks In ChunkBuffer Select item.Kinase Distinct Order By Kinase Ascending).ToArray
            Dim rrIdList = (From item As CrossTalks In ChunkBuffer Select item.Regulator Distinct Order By Regulator Ascending).ToArray

            Dim CsvFile As New File
            Call CsvFile.AppendLine(rrIdList)
            Call CsvFile.First.InsertAt("HisK -> RR", 0)

            For Each HisK_Id As String In skIdList
                Dim row As RowObject = New RowObject From {HisK_Id}
                Dim Scores = (From RRId As String In rrIdList
                              Let t = (From item As CrossTalks
                                   In ChunkBuffer.AsParallel
                                       Where String.Equals(item.Kinase, HisK_Id) AndAlso String.Equals(item.Regulator, RRId)
                                       Select item).ToArray
                              Select If(t.IsNullOrEmpty, 0, t.First.Probability)).ToArray

                Call row.AddRange((From n As Double In Scores
                                   Let strValue As String = n.ToString
                                   Select strValue).ToArray)
                Call CsvFile.AppendLine(row)
            Next

            Return CsvFile
        End Function

        <ExportAPI("TCS.Sequence.Downloads")>
        Public Function DownloadTcsSequence(DIR As String) As FastaFile
            Dim profiles As CrossTalks() = (From path As String
                                            In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.csv").AsParallel
                                            Let data = path.LoadCsv(Of CrossTalks)(False)
                                            Where Not data.IsNullOrEmpty
                                            Select data.ToArray).ToArray.ToVector
            Dim lstId As String() = {(From cTk As CrossTalks
                                      In profiles
                                      Select cTk.Kinase).ToArray, (From cTk As CrossTalks
                                                                   In profiles
                                                                   Select cTk.Regulator).ToArray}.ToVector.Distinct.ToArray
            lstId = (From sId As String
                     In lstId
                     Let Trimed As String = __trim(sId)
                     Select Trimed
                     Distinct
                     Order By Trimed Ascending).ToArray
            Dim LQuery = (From sId As String
                          In lstId
                          Let gFa As SequenceModel.FASTA.FastaSeq = SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.Downloads(DIR, sId)
                          Where Not gFa Is Nothing
                          Select gFa).ToArray
            Return New FastaFile(LQuery)
        End Function

        Private Function __trim(sId As String) As String
            Dim p As Integer = InStr(sId, "(")

            If p > 0 Then
                Return Mid(sId, 1, p - 1)
            Else
                Return sId
            End If
        End Function

        <Extension> Public Function CrossTalk(profiles As IEnumerable(Of CrossTalks), HisK As String, RR As String) As Double
            Dim LQuery = (From ctk As CrossTalks
                          In profiles
                          Where Graph.Abstract.Contains(ctk, HisK) AndAlso Graph.Abstract.Contains(ctk, RR)
                          Select ctk).FirstOrDefault
            If LQuery Is Nothing Then
                Return 0
            Else
                Return LQuery.Probability
            End If
        End Function
    End Module
End Namespace
