#Region "Microsoft.VisualBasic::282ab832cea3d854efcc9b535206b09f, visualize\Cytoscape\Cytoscape\Graph\Xgmml\RDFXml.vb"

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

'     Module RDFXml
' 
'         Function: TrimRDF, (+2 Overloads) WriteXml
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView.XGMML

    Public NotInheritable Class RDFXml

        Const XGMML As String = "http://www.cs.rpi.edu/XGMML"
        Const dc As String = "http://purl.org/dc/elements/1.1/"
        Const xlink As String = "http://www.w3.org/1999/xlink"
        Const rdf As String = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Const cy As String = "http://www.cytoscape.org"

        Public Shared Function WriteXml(graph As Graph, encoding As Encoding, path As String) As Boolean
            If graph.networkMetadata Is Nothing Then
                graph.attributes.Add(NetworkMetadata.createAttribute)
            Else
                graph.networkMetadata.about = "http://www.cytoscape.org/"
            End If

            Return graph.GetXml.SaveTo(path, encoding)
        End Function

        ''' <summary>
        ''' 使用这个方法才能够正确的加载一个cytoscape的网络模型文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(path As String) As Graph
            Return path.LoadXml(Of Graph)()
        End Function
    End Class
End Namespace
