
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("DNA_Comparative")>
Public Module DNA_Comparative

    <ExportAPI("seq.dist")>
    Public Function dist(<RRawVectorArgument> seqs As Object) As DistanceMatrix

    End Function

    <ExportAPI("sigma")>
    Public Function Sigma(f As FastaSeq, g As FastaSeq) As vector
        Dim dist As Double = DifferenceMeasurement.Sigma(f, g)
        Dim desc As String = DifferenceMeasurement.SimilarDescription(dist)

        Return vector.asVector({dist}, New unit(desc.ToString))
    End Function
End Module

