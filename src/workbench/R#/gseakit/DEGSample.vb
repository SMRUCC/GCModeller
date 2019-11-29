
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

<Package("gseakit.DEG_sample")>
Module DEGSample

    <ExportAPI("read.sampleinfo")>
    Public Function ReadSampleInfo(file As String) As SampleInfo()
        Return file.LoadCsv(Of SampleInfo)
    End Function

    <ExportAPI("write.sampleinfo")>
    Public Function WriteSampleInfo(sampleinfo As SampleInfo(), file$) As Boolean
        Return sampleinfo.SaveTo(file)
    End Function

    ''' <summary>
    ''' Create sampleInfo table from text files
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    <ExportAPI("sampleinfo.text.groups")>
    Public Function ScanForSampleInfo(dir As String) As SampleInfo()
        Dim sampleInfo As New List(Of SampleInfo)
        Dim samplelist As String()
        Dim groupName$
        Dim index As i32 = 1

        For Each file As String In ls - l - r - "*.txt" <= dir
            groupName = file.BaseName
            samplelist = file.ReadAllLines
            sampleInfo += samplelist _
                .Select(Function(id)
                            Return New SampleInfo With {
                                .ID = id,
                                .sample_name = id,
                                .sample_group = groupName,
                                .Order = ++index
                            }
                        End Function)
        Next

        Return sampleInfo
    End Function
End Module
