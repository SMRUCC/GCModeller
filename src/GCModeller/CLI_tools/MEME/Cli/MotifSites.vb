#Region "Microsoft.VisualBasic::a132d81856f6713552727e26bd82cedb, CLI_tools\MEME\Cli\MotifSites.vb"

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

    ' Module CLI
    ' 
    '     Function: __loadSimilarityHits, ExportTestMotifs, LoadSimilarityHits, MotifSites2Fasta, MotifSiteSummary
    '               UnionSimilarity
    ' 
    '     Sub: __EXPORT
    '     Class SimilarityHit
    ' 
    '         Properties: HitId, IsMatch, MotifId, Site
    ' 
    '         Function: GetHitSites, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/MotifSites.Fasta", Usage:="/MotifSites.Fasta /in <mast_motifsites.csv> [/out <out.fasta>]")>
    Public Function MotifSites2Fasta(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".fasta")
        Dim sites = [in].LoadCsv(Of MastSites)
        Dim fasta = sites.Select(
            Function(site) New FastaSeq With {
                .SequenceData = site.SequenceData,
                .Headers = {
                    $"{site.Gene}:{site.ATGDist} {site.Trace}"
                }
            }).GroupBy(Function(x) x.Headers(0).Split.First) _
            .Select(Function(x) x.First)
        Return New FastaFile(fasta).Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Export.MotifSites",
               Info:="Motif iteration step 1",
               Usage:="/Export.MotifSites /in <meme.txt> [/out <outDIR> /batch]")>
    <Group(CLIGrouping.MotifSitesTools)>
    Public Function ExportTestMotifs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim batch As Boolean = args.GetBoolean("/batch")

        If batch Then
            Dim out As String = args.GetValue("/out", [in].TrimEnd("\"c, "/"c) & "." & NameOf(ExportTestMotifs))
            For Each text As String In ls - l - r - wildcards("*.txt") <= [in]
                Call __EXPORT(text, out)
            Next
        Else
            Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & NameOf(ExportTestMotifs))
            Call __EXPORT([in], out)
        End If

        Return 0
    End Function

    Private Sub __EXPORT(meme As String, out As String)
        Dim motifs As LDM.Motif() = MEME_TEXT.SafelyLoad(meme, True)

        For Each motif As LDM.Motif In motifs
            Dim sites As IEnumerable(Of FASTA.FastaSeq) =
                motif.Sites.Select(Function(x) x.ToFasta)
            Dim fasta As New FASTA.FastaFile(sites)
            Dim path As String = out & "/" & motif.uid & ".fasta"

            Call fasta.Save(path, Encodings.ASCII)
        Next
    End Sub

    ''' <summary>
    ''' 导出通过MAST程序锁分析出来的Motif之间相似度的结果
    ''' 文件夹的组织结构是Motif.uid -> Motifs
    ''' </summary>
    ''' <returns></returns>
    ''' 
    ''' 
    <ExportAPI("/Export.Similarity.Hits",
               Info:="Motif iteration step 2",
               Usage:="/Export.Similarity.Hits /in <inDIR> [/out <out.Csv>]")>
    <Group(CLIGrouping.MotifSitesTools)>
    Public Function LoadSimilarityHits(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimEnd("\"c, "/"c) & ".SimilarityHits.Csv")
        Dim DIRs As IEnumerable(Of String) = ls - l - lsDIR <= [in]
        Dim result As New List(Of SimilarityHit)

        For Each DIR As String In DIRs
            result +=
                DIR.__loadSimilarityHits
        Next

        Return result.SaveTo(out)
    End Function

    <Extension>
    Private Function __loadSimilarityHits(DIR As String) As SimilarityHit()
        Dim xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") <= DIR
        Dim masts = xmls.Select(AddressOf SafeLoadXml(Of XmlOutput.MAST.MAST))
        Dim MotifId As String = DIR.BaseName
        Dim result As New List(Of SimilarityHit)

        For Each mast As XmlOutput.MAST.MAST In masts
            If mast Is Nothing Then
                Continue For
            End If
            If mast.Sequences Is Nothing Then
                Continue For
            End If
            If mast.Sequences.SequenceList.IsNullOrEmpty Then
                Continue For
            End If

            Dim hitId As String = mast.Motifs.BriefName

            For Each seq As SequenceDescript In mast.Sequences.SequenceList
                If seq.Segments.IsNullOrEmpty Then
                    Continue For
                End If

                result += From seg As XmlOutput.MAST.Segment
                          In seq.Segments
                          Where Not seg.Hits.IsNullOrEmpty
                          Select From hit As XmlOutput.MAST.HitResult
                                 In seg.Hits
                                 Let id As String =
                                     hitId & "-" & hit.motif.Split("_"c).Last
                                 Select New SimilarityHit With {
                                     .MotifId = MotifId,
                                     .HitId = id,
                                     .Site = seq.name
                                 }
            Next
        Next

        Return result
    End Function

    Public Class SimilarityHit
        Public Property MotifId As String
        Public Property HitId As String
        Public Property Site As String

        <Column(Name:="Is.match?")>
        Public ReadOnly Property IsMatch As Boolean
            Get
                Return Not (InStr(HitId, MotifId, CompareMethod.Text) = 1)
            End Get
        End Property

        Public Function GetHitSites(memehash As Dictionary(Of String, LDM.Motif)) As IEnumerable(Of FASTA.FastaSeq)
            Return memehash(HitId).Sites.Select(Function(x) x.ToFasta)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 合并相似的Motif进入下一次迭代
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/Similarity.Union",
               Info:="Motif iteration step 3",
               Usage:="/Similarity.Union /in <preSource.fasta.DIR> /meme <meme.txt.DIR> /hits <similarity_hist.Csv> [/out <out.DIR>]")>
    <Group(CLIGrouping.MotifSitesTools)>
    Public Function UnionSimilarity(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim meme As String = args - "/meme"
        Dim hits As String = args - "/hits"
        Dim out As String = args.GetValue("/out", [in] & "/next.iteraction/")
        Dim hitsHash = (From x As SimilarityHit
                        In hits.LoadCsv(Of SimilarityHit)
                        Where x.IsMatch
                        Select x
                        Group x By x.MotifId Into Group) _
                             .ToDictionary(Function(x) x.MotifId,
                                           Function(x) x.Group.ToArray)
        Dim memehash = (From file As String
                        In ls - l - r - wildcards("*.txt") <= meme
                        Let motifs As LDM.Motif() =
                            MEME_TEXT.SafelyLoad(file, True)
                        Where Not motifs.IsNullOrEmpty
                        Select motifs).IteratesALL.ToDictionary(Function(x) x.uid)

        For Each fa As String In ls - l - wildcards("*.fasta") <= [in]
            Dim name As String = fa.BaseName
            If Not hitsHash.ContainsKey(name) Then
                Continue For
            End If

            Dim fasta As New FASTA.FastaFile(fa)

            For Each hit In hitsHash(name)
                If memehash.ContainsKey(hit.HitId) Then
                    fasta += hit.GetHitSites(memehash)
                End If
            Next

            Call fasta.Save(out & "/" & name & ".fasta")
        Next

        Return 0
    End Function

    <ExportAPI("/motif.sites.summary")>
    <Usage("/motif.sites.summary /in <data.directory> [/out <summary.csv>]")>
    Public Function MotifSiteSummary(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or [in].TrimDIR & ".summary.csv"
        Dim familyHits = [in] _
            .ListFiles("*.fasta") _
            .Select(Function(path)
                        Dim family = path.BaseName
                        Return StreamIterator.SeqSource(path).Select(Function(fa) (fa.locus_tag, family))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(a) a.locus_tag) _
            .Select(Function(geneId)
                        Dim list = geneId.Select(Function(hit) hit.family).Distinct.ToArray
                        Return New FootprintSite With {
                            .ID = geneId.Key,
                            .gene = geneId.Key,
                            .src = list
                        }
                    End Function) _
            .ToArray

        Return familyHits.SaveTo(out).CLICode
    End Function
End Module
