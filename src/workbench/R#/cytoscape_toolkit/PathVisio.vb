#Region "Microsoft.VisualBasic::b404397c9af137635051b34156e2f2e1, R#\cytoscape_toolkit\PathVisio.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module PathVisio
    ' 
    '     Function: createGraph, NodesTable, readXmlModel
    ' 
    ' /********************************************************************************/

#End Region

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
