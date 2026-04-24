Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("gene_quantification")>
Module Quantification

    <ExportAPI("read_featureCounts")>
    <RApiReturn(GetType(featureCounts))>
    Public Function read_featureCounts(file As String) As Object
        Return featureCounts.ReadTable(file).ToArray
    End Function

End Module
