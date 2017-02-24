Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.ClusterMatrix
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Imaging

Partial Module CLI

    <ExportAPI("/locis.clustering",
               Usage:="/locis.clustering /in <locis.fasta> [/first.ID /method <NeedlemanWunsch> /colors <clusters> /clusters <20> /out <out.DIR>]")>
    Public Function LociClustering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim method$ = args.GetValue("/method", "NeedlemanWunsch")
        Dim expected% = args.GetValue("/clusters", 20)
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{method}_expected={expected}.clusters/")
        Dim fasta As FastaFile = FastaFile.LoadNucleotideData([in])
        Dim firstID As Boolean = args.GetBoolean("/first.ID")

        If firstID Then
            Call fasta.FirstTokenID
        End If

        Dim matrix As DataSet() = fasta.SimilarityMatrix
        Dim clusters As EntityLDM() = matrix.KMeans(expected)
        Dim colors As Color() = Designer.GetColors(args <= "/colors", expected)
        Dim clusterColors As Dictionary(Of String, Color) = clusters _
            .Select(Function(x) x.Cluster) _
            .Distinct _
            .SeqIterator _
            .ToDictionary(Function(cluster) +cluster,
                          Function(color) colors(color))

        Call clusters.SaveTo(out & "/clusters.csv")
        Call clusters.ToNetwork(clusterColors).Save(out)

        Return 0
    End Function
End Module