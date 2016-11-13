Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Binary.KMeans.SW",
               Usage:="/Binary.KMeans.SW /in <dataset.fasta> [/cut 0.65 /minw 6 /out <out.DIR>]")>
    Public Function BinaryKmeansSW(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim cut# = args.GetValue("/cut", 0.65)
        Dim minw% = args.GetValue("/minw", 6)
        Dim out As String =
            args.GetValue("/out", [in].TrimSuffix & $"-cut={cut},minw={minw}/")
        Dim fa As New FastaFile([in])
        Dim clusters = BinaryKmeans(fa, cut, minw)
        Dim net = clusters.bTreeNET

        Call clusters.SaveTo(out & $"/{[in].BaseName}-kmeans.csv")
        Call net.Save(out & "/binary-net/")

        Return 0
    End Function

    Public Function BinaryKmeans(seq As FastaFile, Optional cutoff# = 0.65, Optional minW% = 6) As EntityLDM()
        Dim LQuery = From a As FastaToken
                     In seq.AsParallel
                     Let Name As String = a.Title
                     Select New EntityLDM With {
                         .Name = Name,
                         .Properties = a.Cluster(seq, cutoff, minW)
                     }
        Dim tree As EntityLDM() = LQuery.TreeCluster(True)
        Return tree
    End Function

    <Extension>
    Private Function Cluster(query As FastaToken, source As FastaFile, cutoff#, minW%) As Dictionary(Of String, Double)
        Dim LQuery = From b As FastaToken
                     In source
                     Let sw As SmithWaterman = SmithWaterman.Align(query, b)
                     Let out As HSP = sw.GetOutput(cutoff, minW).Best
                     Select b.Title,
                         score = If(out Is Nothing, -10.0R, out.Score)
        Dim output As Dictionary(Of String, Double) =
            LQuery.ToDictionary(Function(x) x.Title,
                                Function(x) x.score)
        Return output
    End Function
End Module
