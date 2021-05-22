#Region "Microsoft.VisualBasic::913bc5eacd5d36b1710e20c12f0d8ea5, visualize\Cytoscape\Cytoscape\Session\CysSessionFile.vb"

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

'     Class CysSessionFile
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetCollectionKey, GetLayoutedGraph, GetNetworks, GetSessionInfo, GetViewKey
'                   nodeLabels, Open
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Session

    '<XmlRoot("graph", [Namespace]:="http://www.cs.rpi.edu/XGMML")>
    'Public Class NetworkCollection

    '    <XmlAttribute> Public Property id As String
    '    <XmlAttribute> Public Property label As String

    '    Public Property att As att

    '    <XmlNamespaceDeclarations()>
    '    Public xmlns As XmlSerializerNamespaces

    '    Sub New()
    '        xmlns = New XmlSerializerNamespaces

    '        xmlns.Add("cy", XGMMLgraph.xmlnsCytoscape)
    '        xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
    '        xmlns.Add("xlink", "http://www.w3.org/1999/xlink")
    '        xmlns.Add("dc", XGMMLgraph.xmlns_dc)
    '    End Sub
    'End Class

    'Public Class att
    '    <XmlElement("graph")>
    '    Public Property graphs As XGMMLgraph()
    'End Class

    ''' <summary>
    ''' ``*.cys`` cytoscape session file reader model
    ''' </summary>
    Public Class CysSessionFile

        ''' <summary>
        ''' the original *.cys session file location.
        ''' </summary>
        Public ReadOnly source As String

        ReadOnly tempDir As String

        Private Sub New(tempDir As String, cys As String)
            Me.tempDir = tempDir
            Me.source = cys
        End Sub

        Public Function GetSessionInfo() As virtualColumn()
            Return ($"{tempDir}/tables/cytables.xml") _
                .LoadXml(Of cyTables) _
                .AsEnumerable _
                .ToArray
        End Function

        Public Iterator Function GetNetworks() As IEnumerable(Of NamedCollection(Of String))
            Dim xml As XmlElement
            Dim collection As String
            Dim networkNames As New List(Of String)

            For Each file As String In $"{tempDir}/networks".ListFiles("*.xgmml")
                xml = file.ReadAllText.DoCall(AddressOf XmlElement.ParseXmlText)
                collection = xml.attributes("label")
                xml = xml.getElementsByTagName("att").First

                For Each graph In xml.getElementsByTagName("graph")
                    networkNames.Add(graph.attributes("label"))
                Next

                Yield New NamedCollection(Of String) With {
                    .name = collection,
                    .value = networkNames.PopAll
                }
            Next
        End Function

        Public Function GetCollectionKey(collection As String) As NamedValue(Of String)
            Dim find As String = $"*{collection}.xgmml"
            Dim xml As XmlElement

            find = $"{tempDir}/networks".ListFiles(find).FirstOrDefault

            If Not find Is Nothing Then
                xml = XmlElement.ParseXmlText(find.ReadAllText)

                If xml.attributes("label") = collection Then
                    Return New NamedValue(Of String) With {
                        .Name = collection,
                        .Value = xml.id,
                        .Description = find
                    }
                End If
            End If

            For Each file As String In $"{tempDir}/networks".ListFiles("*.xgmml")
                xml = file.ReadAllText.DoCall(AddressOf XmlElement.ParseXmlText)

                If xml.attributes("label") = collection Then
                    Return New NamedValue(Of String) With {
                        .Name = collection,
                        .Value = xml.id,
                        .Description = find
                    }
                End If
            Next

            Return Nothing
        End Function

        Public Function GetViewKey(name As String, id As String) As NamedValue(Of String)
            Dim find As String = $"{id}*{name}.xgmml"
            Dim xml As XmlElement

            find = $"{tempDir}/views".ListFiles(find).FirstOrDefault

            If Not find Is Nothing Then
                xml = XmlElement.ParseXmlText(find.ReadAllText)

                If xml.attributes("{http://www.cytoscape.org}networkId") = id Then
                    Return New NamedValue(Of String) With {
                        .Name = name,
                        .Value = xml.id,
                        .Description = find
                    }
                End If
            End If

            For Each file As String In $"{tempDir}/views".ListFiles("*.xgmml")
                xml = file.ReadAllText.DoCall(AddressOf XmlElement.ParseXmlText)

                If xml.attributes("{http://www.cytoscape.org}networkId") = id Then
                    Return New NamedValue(Of String) With {
                        .Name = name,
                        .Value = xml.id,
                        .Description = find
                    }
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 加载一个已经具有网络布局信息的网络模型
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLayoutedGraph(Optional collection$ = Nothing, Optional name$ = Nothing) As NetworkGraph
            Dim network = GetCollectionKey(collection).Description.ReadAllText.DoCall(AddressOf XmlElement.ParseXmlText)
            Dim graph = network.getElementsByTagName("att").First.getElementsByTagName("graph").FirstOrDefault(Function(a) a.attributes("label") = name)

            If graph Is Nothing Then
                Return Nothing
            End If

            Dim graphId As String = graph.id
            Dim view = GetViewKey(name, graphId).Description.ReadAllText.DoCall(AddressOf XmlElement.ParseXmlText)
            Dim g As New NetworkGraph
            Dim info = GetSessionInfo().GroupBy(Function(a) a.name).ToDictionary(Function(a) a.Key, Function(a) a.ToArray)
            Dim sharedName = info("shared name").FirstOrDefault(Function(a) a.sourceTable.BaseName.StartsWith("SHARED_ATTRS-org.cytoscape.model.CyNode"))
            Dim nodeNames = cyTable.LoadTable($"{tempDir}/tables/{sharedName.sourceTable}").DoCall(AddressOf nodeLabels)
            Dim nodeIndex As New Dictionary(Of String, Node)

            For Each node As XmlElement In graph.getElementsByTagName("node")
                Call nodeIndex.Add(node.id, g.CreateNode(nodeNames(node.id)))
            Next

            For Each edge As XmlElement In graph.getElementsByTagName("edge")
                Call g.AddEdge(nodeIndex(edge.attributes("source")), nodeIndex(edge.attributes("target")))
            Next

            Dim graphics As XmlElement

            For Each node As XmlElement In view.getElementsByTagName("node")
                graphId = node.attributes("{http://www.cytoscape.org}nodeId")
                graphics = node.getElementsByTagName("graphics").First

                g.GetElementByID(nodeNames(graphId)).data.initialPostion = New FDGVector3(
                    x:=Single.Parse(graphics.attributes("x")),
                    y:=Single.Parse(graphics.attributes("y")),
                    z:=Single.Parse(graphics.attributes("z"))
                )
            Next

            Return g
        End Function

        Private Shared Function nodeLabels(nodeNames As cyTable) As Dictionary(Of String, String)
            Dim labels As New Dictionary(Of String, String)
            Dim SUID As cyField = nodeNames!SUID
            Dim common As cyField = nodeNames!common
            Dim size As Size = nodeNames.Dim

            For i As Integer = 0 To size.Height - 1
                labels.Add(SUID(i), common(i))
            Next

            Return labels
        End Function

        Public Shared Function Open(cys As String) As CysSessionFile
            Dim temp As String = TempFileSystem.GetAppSysTempFile(".zip", App.PID, "cytoscape_")

            Call UnZip.ImprovedExtractToDirectory(cys, temp, Overwrite.Always, extractToFlat:=False)

            Return New CysSessionFile(temp.ListDirectory.First, cys)
        End Function
    End Class
End Namespace
