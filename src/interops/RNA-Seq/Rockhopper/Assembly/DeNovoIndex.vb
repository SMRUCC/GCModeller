' /********************************************************************************/
'
'  Rockhopper —— de novo 候选转录本的 BWT 索引与二次比对
'
'  复刻论文（13059_2014_Article_572.pdf）的 de novo 组装第二步：
'    把 de Bruijn 图遍历得到的候选转录本集合存为一个 Burrows-Wheeler 索引，
'    再把原始读段比对回候选转录本，通过比对结果淘汰覆盖率不足的候选
'    （阈值由 `Min reads mapping to a transcript` / CLI `-b` 控制，默认 20）。
'
'  实现上直接复用 Alignment 模块的 FM-index 比对器（Core.Replicon + Alignment.Aligner），
'  避免重复实现索引与种子延伸。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core

Namespace Assembly

    ''' <summary>
    ''' 候选转录本的 BWT 索引与读段二次比对。
    ''' </summary>
    Public Class DeNovoIndex

        Private ReadOnly candidates As DeNovoTranscript()
        Private ReadOnly replicons As Replicon()

        Public ReadOnly Property Count As Integer
            Get
                Return candidates.Length
            End Get
        End Property

        Public Sub New(candidates As IEnumerable(Of DeNovoTranscript))
            Me.candidates = candidates.Where(Function(t) t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t.Sequence)).ToArray()
            Me.replicons = Me.candidates.Select(Function(t, i) New Replicon($"denovo_{i}", t.Sequence)).ToArray()
        End Sub

        ''' <summary>
        ''' 把原始读段比对回候选转录本，统计支持度并淘汰低覆盖候选。
        ''' </summary>
        ''' <param name="reads">原始读段（应已按 MinReadLength 过滤）。</param>
        ''' <param name="minReadsMapping">保留候选所需的最少映射读段数（CLI -b）。</param>
        ''' <param name="numThreads">并行线程数。</param>
        Public Function MapReads(reads As IEnumerable(Of FileIO.Read), minReadsMapping As Integer, Optional numThreads As Integer = 1) As DeNovoTranscripts
            If replicons.Length = 0 Then Return New DeNovoTranscripts()

            Dim aligner As New Alignment.Aligner(replicons,
                                                 percentMismatches:=0.1,
                                                 percentSeedLength:=0.5,
                                                 stopAfterOneHit:=True,
                                                 numThreads:=numThreads)
            aligner.BuildIndex()

            Dim counts As Integer() = New Integer(replicons.Length - 1) {}
            Dim readArray As FileIO.Read() = reads.ToArray()

            For Each hit As Alignment.AlignmentHit In aligner.AlignReads(readArray)
                If hit.RepliconIndex >= 0 AndAlso hit.RepliconIndex < counts.Length Then
                    counts(hit.RepliconIndex) += 1
                End If
            Next

            Call Logging.Output($"[MAP] candidates={replicons.Length}, reads={readArray.Length}, totalHits={counts.Sum()}{vbLf}")
            If replicons.Length > 0 Then
                Dim reference As String = Me.replicons(0).SequenceData
                Call Logging.Output($"[MAP] replicon0 len={reference.Length} seq={reference}{vbLf}")
                If readArray.Length > 0 Then Call Logging.Output($"[MAP] read0 len={readArray(0).Length} seq={readArray(0).Sequence}{vbLf}")
                If readArray.Length > 1 Then Call Logging.Output($"[MAP] read1 len={readArray(1).Length} seq={readArray(1).Sequence}{vbLf}")
            End If

            Dim totalMapped As Long = counts.Sum(Function(c) CLng(c))
            Dim results As New List(Of DeNovoTranscript)()

            For i As Integer = 0 To candidates.Length - 1
                If counts(i) < minReadsMapping Then Continue For

                Dim length As Integer = System.Math.Max(1, candidates(i).Length)
                Dim expression As Double = If(totalMapped = 0, 0.0,
                    counts(i) / (CDbl(length) * totalMapped) * 1000000000.0) ' 改良 RPKM（以映射读段总数为归一化因子）

                results.Add(New DeNovoTranscript With {
                    .Sequence = candidates(i).Sequence,
                    .Expression = expression,
                    .QValue = 1.0,
                    .Reads = counts(i)
                })
            Next

            Return New DeNovoTranscripts(results)
        End Function

    End Class

End Namespace
