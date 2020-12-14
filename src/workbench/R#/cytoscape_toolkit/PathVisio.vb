Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.PathVisio.GPML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("PathVisio")>
Module PathVisio

    <ExportAPI("read.gpml")>
    <RApiReturn(GetType(Pathway))>
    Public Function readXmlModel(file As String, Optional env As Environment = Nothing) As Object
        If file.FileExists Then
            Return file.LoadXml(Of Pathway)
        Else
            Return Internal.debug.stop({$"the given file for read is not exists on your file system!", $"file: {file.GetFullPath}"}, env)
        End If
    End Function
End Module
