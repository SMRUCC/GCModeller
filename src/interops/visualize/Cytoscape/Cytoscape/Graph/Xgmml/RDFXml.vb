﻿#Region "Microsoft.VisualBasic::293181812f6aafc346c87c3e15af9040, visualize\Cytoscape\Cytoscape\Graph\Xgmml\RDFXml.vb"

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


    ' Code Statistics:

    '   Total Lines: 34
    '    Code Lines: 22
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 1.39 KB


    '     Class RDFXml
    ' 
    '         Function: Load, WriteXml
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView.XGMML

    Public NotInheritable Class RDFXml

        Public Const XGMML As String = "http://www.cs.rpi.edu/XGMML"
        Public Const dc As String = "http://purl.org/dc/elements/1.1/"
        Public Const xlink As String = "http://www.w3.org/1999/xlink"
        Public Const rdf As String = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Public Const cy As String = "http://www.cytoscape.org"

        Public Shared Function WriteXml(graph As XGMMLgraph, encoding As Encoding, path As String) As Boolean
            Return XmlDocumentText(graph).SaveTo(path, encoding)
        End Function

        Public Shared Function XmlDocumentText(graph As XGMMLgraph) As String
            If graph.networkMetadata Is Nothing Then
                graph.attributes.Add(NetworkMetadata.createAttribute)
            Else
                graph.networkMetadata.about = "http://www.cytoscape.org/"
            End If

            Return graph.GetXml
        End Function

        ''' <summary>
        ''' 使用这个方法才能够正确的加载一个cytoscape的网络模型文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(path As String) As XGMMLgraph
            Return path.LoadXml(Of XGMMLgraph)()
        End Function
    End Class
End Namespace
