
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

<Package("gseakit.DEG_sample")>
Module DEGSample

    <ExportAPI("read.sampleinfo")>
    Public Function ReadSampleInfo(file As String) As SampleInfo()
        Return file.LoadCsv(Of SampleInfo)
    End Function
End Module
