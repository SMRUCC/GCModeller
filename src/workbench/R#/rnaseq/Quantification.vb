Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.GeneQuantification
Imports SMRUCC.genomics.SequenceModel.SAM.featureCount
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("gene_quantification")>
Module Quantification

    <ExportAPI("read_featureCounts")>
    <RApiReturn(GetType(featureCounts))>
    Public Function read_featureCounts(file As String) As Object
        Return featureCounts.ReadTable(file).ToArray
    End Function

    <ExportAPI("gene_indexstats")>
    <RApiReturn(GetType(GeneData))>
    Public Function sample_indexstats(file As String) As Object
        Return GeneQuantification.ConvertCountsToTPM(IndexStats.Parse(file.OpenReadonly)).ToArray
    End Function

    <ExportAPI("read_genedata")>
    <RApiReturn(GetType(GeneData))>
    Public Function read_genedata(file As String) As GeneData()
        Return file.LoadCsv(Of GeneData)(mute:=True)
    End Function
End Module
