Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Module FileName

    ''' <summary>
    ''' bootstrapping sampling
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <param name="n"></param>
    ''' <param name="range"></param>
    ''' <returns></returns>
    Public Iterator Function RandomSeed(seq As String, n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        For i As Integer = 0 To n
            Dim klen As Integer = randf.GetNextBetween(range)
            Dim left As Integer = randf.NextInteger(seq.Length - klen)
            Dim part As String = seq.Substring(left, klen)

            Yield New GraphSeed With {
                .part = part,
                .start = left,
                .graph = Builder.SequenceGraph(part, SequenceModel.NT).GetVector(SequenceModel.NT)
            }
        Next
    End Function

    <Extension>
    Public Function RandomSeed(seqs As IEnumerable(Of FastaSeq), n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        Return seqs.Select(Function(fa) RandomSeed(fa.SequenceData, n, range)).IteratesALL
    End Function

    Public Iterator Function Cluster(seeds As IEnumerable(Of GraphSeed), member As Double) As IEnumerable(Of NamedCollection(Of GraphSeed))
        Dim tree As New AVLClusterTree(Of GraphSeed)(GraphSeed.GetCompares(cluster:=member), views:=Function(a) a.part)

        For Each seed As GraphSeed In seeds
            Call tree.Add(seed)
        Next

        For Each node As ClusterKey(Of GraphSeed) In tree
            If node.NumberOfKey > 2 Then
                Yield New NamedCollection(Of GraphSeed)("", node.ToArray)
            End If
        Next
    End Function

    <Extension>
    Public Function CreateMotifs(node As NamedCollection(Of GraphSeed), param As PopulatorParameter) As SequenceMotif
        Dim pwm = node.Select(Function(a) a.part).BuildMotifPWM(param)

        If Not pwm Is Nothing Then
            Return pwm.Cleanup
        Else
            Return Nothing
        End If
    End Function

End Module

Public Class GraphSeed

    ''' <summary>
    ''' the part of the sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property part As String
    Public Property start As Integer
    Public Property graph As Double()

    Public Shared Function GetCompares(cluster As Double) As Comparison(Of GraphSeed)
        Return Function(a, b)
                   Dim score As Double = SSM(a.graph, b.graph)

                   If score >= cluster Then
                       Return 0
                   ElseIf score > 0 Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function
End Class
