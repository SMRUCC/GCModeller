#Region "Microsoft.VisualBasic::2c8d17e3ea0bff1587e6fb355fe7b071, CLI_tools\Spiderman\CreateNetwork\Network.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::fe618c2ea05cf2787dbf0036d278b049, CLI_tools\Spiderman\CreateNetwork\Network.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Class Network
'    ' 
'    '     Function: ToString
'    '     Class Edge
'    ' 
'    '         Properties: Description, Direction, FromNode, ToNode
'    '         Enum Directions
'    ' 
'    '             Bidirectional, DirectlyTo
'    ' 
'    ' 
'    ' 
'    ' 
'    ' 
'    '  
'    ' 
'    '     Properties: Edges
'    ' 
'    '     Function: LoadCSVTabularModel, (+2 Overloads) LoadModel, SearchPath
'    ' 
'    '     Sub: Save
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.Xml.Serialization
'Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
'Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.DataModel

'Public Class Network

'    Public Class Edge
'        <XmlAttribute> Public Property FromNode As String
'        <XmlAttribute> Public Property ToNode As String
'        <XmlElement> Public Property Description As String
'        ''' <summary>
'        ''' Path Search direction between <see cref="Edge.FromNode"></see> and <see cref="Edge.ToNode"></see> 
'        ''' </summary>
'        ''' <value></value>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        <XmlAttribute> Public Property Direction As Directions

'        Public Enum Directions
'            DirectlyTo
'            Bidirectional
'        End Enum

'        Public Overrides Function ToString() As String
'            Dim strDirection As String = If(Direction = Directions.DirectlyTo, "-->", "<==>")
'            Return String.Format("{0} {1} {2}; ({3})", FromNode.ToString, strDirection, ToNode.ToString, Description)
'        End Function
'    End Class

'    Public Property Edges As Edge()

'    Public Function SearchPath(FromNode As String, ToNode As String) As Edge()
'        Dim DirectToLQuery = (From edge In Edges Where String.Equals(edge.FromNode, FromNode) Select edge Order By edge.Direction Ascending).ToArray
'        Dim BidirectionalLQuery = (From edge In Edges Where String.Equals(edge.ToNode, FromNode) AndAlso edge.Direction = Edge.Directions.Bidirectional Select edge).ToArray

'        If DirectToLQuery.IsNullOrEmpty AndAlso BidirectionalLQuery.IsNullOrEmpty Then
'            Return New Edge() {}
'        Else
'            Dim Pathway As List(Of Edge) = New List(Of Edge)

'            For Each Edge In DirectToLQuery
'                If String.Equals(Edge.ToNode, ToNode) Then
'                    Return New Edge() {Edge}
'                Else
'                    Call Pathway.AddRange(SearchPath(Edge.ToNode, ToNode))
'                End If
'            Next
'            For Each Edge In BidirectionalLQuery
'                If String.Equals(Edge.FromNode, ToNode) Then
'                    Return New Edge() {Edge}
'                Else
'                    Call Pathway.AddRange(SearchPath(Edge.FromNode, ToNode))
'                End If
'            Next

'            Return Pathway.ToArray
'        End If
'    End Function

'    Public Sub Save(SavedFile As String)
'        Call Me.GetXml.SaveTo(SavedFile)
'    End Sub

'    Public Shared Function LoadModel(GCML As String) As Network
'        Dim Model = GCML.LoadXml(Of BacterialModel)()
'        Return LoadModel(Model)
'    End Function

'    Public Shared Function LoadModel(GCML As BacterialModel) As Network
'        Dim EdgeList As List(Of Network.Edge) = New List(Of Edge)
'        Call EdgeList.AddRange(MetabolismNetwork.CreateObject(GCML.Metabolism.MetabolismNetwork.ToArray))

'        Return New Network With {.Edges = EdgeList.ToArray}
'    End Function

'    Public Shared Function LoadCSVTabularModel(ModelFile As String) As Network
'        Dim Model As New CellSystem(ModelFile, New LogFile(Settings.LogDIR & "/Spiderman.log"))
'        Return LoadModel(Model.LoadAction)
'    End Function
'End Class

