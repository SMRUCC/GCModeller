Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.PathVisio
Imports SMRUCC.genomics.Model.PathVisio.GPML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
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

    <ExportAPI("nodes.table")>
    Public Function NodesTable(pathway As Pathway) As dataframe
        Dim uuid As String() = pathway.DataNode.Select(Function(n) n.GraphId).ToArray
        Dim metaboliteName As String() = pathway.DataNode.Select(Function(n) n.TextLabel.TrimNewLine).ToArray
        Dim type As String() = pathway.DataNode.Select(Function(n) n.Type.ToString).ToArray
        Dim dbrefName As String() = pathway.DataNode.Select(Function(n) n.Xref.Database).ToArray
        Dim dbref As String() = pathway.DataNode.Select(Function(n) n.Xref.ID).ToArray
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"name", metaboliteName},
                {"type", type},
                {"database", dbrefName},
                {"dbref", dbref}
            },
            .rownames = uuid
        }

        Return table
    End Function

    <ExportAPI("as.graph")>
    Public Function createGraph(pathway As Pathway) As NetworkGraph
        Return pathway.CreateGraph
    End Function
End Module
