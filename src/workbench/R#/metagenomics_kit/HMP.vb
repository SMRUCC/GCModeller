Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Repository.NIH.HMP
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' An internal ``HMP`` client for download data files from ``https://portal.hmpdacc.org/`` website
''' </summary>
''' 
<Package("HMP_portal")>
Module HMP

    ''' <summary>
    ''' run file downloads
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="outputdir"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("fetch")>
    Public Function fetch(<RRawVectorArgument> files As Object, outputdir As String, Optional env As Environment = Nothing) As Object
        Dim filesManifest As pipeline = pipeline.TryCreatePipeline(Of manifest)(files, env)

        If filesManifest.isError Then
            Return filesManifest.getError
        End If

        Return filesManifest _
            .populates(Of manifest) _
            .HandleFileDownloads(save:=outputdir) _
            .ToArray
    End Function

    <ExportAPI("read.manifest")>
    <RApiReturn(GetType(manifest))>
    Public Function readFileManifest(file As String, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("the required file path can not be nothing!", env)
        ElseIf file.FileExists Then
            Return manifest.LoadTable(file).ToArray
        Else
            Return Internal.debug.stop({"the given file is not exists!", "path: " & file}, env)
        End If
    End Function
End Module
