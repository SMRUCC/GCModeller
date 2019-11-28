Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA

<Package("gseakit.background")>
Public Module GSEABackground

    <ExportAPI("read.background")>
    Public Function ReadBackground(file As String) As Background
        Return file.LoadXml(Of Background)
    End Function

    <ExportAPI("geneSet.intersects")>
    Public Function ClusterIntersections(cluster As Cluster, geneSet$(), Optional isLocusTag As Boolean = False) As String()
        Return cluster.Intersect(geneSet, isLocusTag).ToArray
    End Function
End Module
