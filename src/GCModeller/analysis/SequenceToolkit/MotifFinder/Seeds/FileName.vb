Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataMining.UMAP
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
    Public Iterator Function RandomSeed(seq As FastaSeq, n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        Dim seqtitle As String = seq.Title
        Dim seqdata As String = seq.SequenceData

        Call VBDebugger.EchoLine(seq.Title)

        For i As Integer = 0 To n
            Dim klen As Integer = randf.GetNextBetween(range)
            Dim left As Integer = randf.NextInteger(seq.Length - klen)
            Dim part As String = seqdata.Substring(left, klen)

            Yield New GraphSeed With {
                .part = part,
                .start = left,
                .graph = Builder _
                    .SequenceGraph(part, SequenceModel.NT) _
                    .GetVector(SequenceModel.NT),
                .source = seqtitle
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RandomSeed(seqs As IEnumerable(Of FastaSeq), n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        Return seqs.Select(Function(fa) RandomSeed(fa, n, range)).IteratesALL
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
    ''' <summary>
    ''' the raw graph data
    ''' </summary>
    ''' <returns></returns>
    Public Property graph As Double()
    ''' <summary>
    ''' the raw <see cref="graph"/> vector umap embedding data result
    ''' </summary>
    ''' <returns></returns>
    Public Property embedding As Double()
    Public Property source As String

    Public Overrides Function ToString() As String
        Return part
    End Function

    Public Shared Function GetCompares(cluster As Double) As Comparison(Of GraphSeed)
        Return Function(a, b)
                   Dim score As Double = SSM(a.embedding, b.embedding)

                   If score >= cluster Then
                       Return 0
                   ElseIf score > 0 Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function

    Public Shared Iterator Function UMAP(seeds As GraphSeed(), ndims As Integer) As IEnumerable(Of GraphSeed)
        Dim manifold As New Umap(AddressOf DistanceFunctions.CosineForNormalizedVectors, dimensions:=ndims)
        Dim x As Double()() = seeds.Select(Function(a) a.graph).ToArray
        Dim nloops As Integer

        nloops = manifold.InitializeFit(x)
        manifold = manifold.Step(nloops)
        x = manifold.GetEmbedding

        For i As Integer = 0 To seeds.Length - 1
            seeds(i).embedding = x(i)
            Yield seeds(i)
        Next
    End Function
End Class
