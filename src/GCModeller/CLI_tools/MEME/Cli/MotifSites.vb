Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.Text
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports System.Data.Linq.Mapping

Partial Module CLI

    <ExportAPI("/Export.MotifSites",
               Info:="Motif iteration step 1",
               Usage:="/Export.MotifSites /in <meme.txt> [/out <outDIR> /batch]")>
    Public Function ExportTestMotifs(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim batch As Boolean = args.GetBoolean("/batch")

        If batch Then
            Dim out As String = args.GetValue("/out", [in].TrimEnd("\"c, "/"c) & "." & NameOf(ExportTestMotifs))
            For Each text As String In ls - l - r - wildcards("*.txt") <= [in]
                Call __EXPORT(text, out)
            Next
        Else
            Dim out As String = args.GetValue("/out", [in].TrimFileExt & "." & NameOf(ExportTestMotifs))
            Call __EXPORT([in], out)
        End If

        Return 0
    End Function

    Private Sub __EXPORT(meme As String, out As String)
        Dim motifs As LDM.Motif() = MEME_TEXT.SafelyLoad(meme, True)

        For Each motif As LDM.Motif In motifs
            Dim sites As IEnumerable(Of FASTA.FastaToken) =
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
    Public Function LoadSimilarityHits(args As CommandLine.CommandLine) As Integer
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

            For Each seq In mast.Sequences.SequenceList
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

        Public Function GetHitSites(memehash As Dictionary(Of String, LDM.Motif)) As IEnumerable(Of FASTA.FastaToken)
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
    Public Function UnionSimilarity(args As CommandLine.CommandLine) As Integer
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
                        Select motifs).MatrixAsIterator.ToDictionary(Function(x) x.uid)

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
End Module