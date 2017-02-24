Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports SMRUCC.genomics.Analysis.SequenceTools.ClusterMatrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/locis.clustering",
               Usage:="/locis.clustering /in <locis.fasta> [/first.ID /method <NeedlemanWunsch> /clusters <20> /out <out.DIR>]")>
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

        Call clusters.SaveTo(out & "/clusters.csv")

        Return 0
    End Function
End Module