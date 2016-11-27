Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module OTU

    ''' <summary>
    ''' Only works on the 16S/18S data
    ''' </summary>
    ''' <param name="contigs"></param>
    ''' <param name="similarity">
    ''' Two sequence at least have 97% percentage similarity that can be cluster into one OTU
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildOTUClusters(contigs As IEnumerable(Of FastaToken), output As StreamWriter, Optional similarity# = 97%) As NamedValue(Of String())()
        Dim ref As FastaToken = contigs.First
        Dim OTUs As New List(Of (ref As FastaToken, fullEquals#, cluster As NamedValue(Of List(Of String))))
        Dim n As int = 1

        OTUs += (ref,
            fullEquals:=RunNeedlemanWunsch.RunAlign(
                ref, ref,
                False,
                output, echo:=False),
            cluster:=New NamedValue(Of List(Of String)) With {
                .Name = "OTU_" & ++n,
                .Value = New List(Of String) From {
                    ref.Title
                }
        })

        For Each seq As FastaToken In contigs.Skip(1)
            Dim matched = LinqAPI.DefaultFirst(Of (ref As FastaToken, fullEquals#, cluster As NamedValue(Of List(Of String)))) <=
                From OTU As (ref As FastaToken, fullEquals#, cluster As NamedValue(Of List(Of String)))
                In OTUs.AsParallel
                Let score As Double = RunNeedlemanWunsch.RunAlign(
                    seq, OTU.ref,
                    False,
                    output, echo:=False)
                Let is_matched As Double = 100 * score / OTU.fullEquals
                Where is_matched >= similarity
                Select OTU

            If matched.ref Is Nothing Then
                ' 没有找到匹配的，则是新的cluster
                OTUs += (seq,
                    fullEquals:=RunNeedlemanWunsch.RunAlign(
                        seq, seq,
                        False,
                        output, echo:=False),
                    cluster:=New NamedValue(Of List(Of String)) With {
                        .Name = "OTU_" & ++n,
                        .Value = New List(Of String) From {
                            seq.Title
                        }
                    })
            Else
                matched.cluster.Value.Add(seq.Title) ' 是当前的找到的这个OTU的
            End If
        Next

        Return LinqAPI.Exec(Of NamedValue(Of String())) <=
 _
            From OTU As (ref As FastaToken, fullEquals#, cluster As NamedValue(Of List(Of String)))
            In OTUs
            Let refSeq As FastaToken = New FastaToken With {
                .SequenceData = OTU.ref.SequenceData,
                .Attributes = {
                    OTU.cluster.Name & " " & OTU.ref.Title
                }
            }
            Let OTUseq As String = refSeq.GenerateDocument(-1)
            Let cluster As NamedValue(Of List(Of String)) = OTU.cluster
            Select New NamedValue(Of String()) With {
                .Name = cluster.Name,
                .Value = cluster.Value,
                .Description = OTUseq
            }
    End Function
End Module
