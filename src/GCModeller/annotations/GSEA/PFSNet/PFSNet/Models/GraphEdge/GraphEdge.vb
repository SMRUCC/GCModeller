#Region "Microsoft.VisualBasic::e4ba5024584b2cc0a22ed5ba1f60d4c7, annotations\GSEA\PFSNet\PFSNet\Models\GraphEdge\GraphEdge.vb"

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

    '     Class GraphEdge
    ' 
    '         Properties: g1, g2, isSelfLoop, pathwayID
    ' 
    '         Function: LoadData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace DataStructure

    ''' <summary>
    ''' Gene to Gene Interaction.(基因与基因之间的互作关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GraphEdge

        <XmlAttribute>
        Public Property pathwayID As String

        ''' <summary>
        ''' The geneID of a gene node in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene1")> Public Property g1 As String
        ''' <summary>
        ''' Another partner gene node its id in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene2")> Public Property g2 As String

        ''' <summary>
        ''' Does <see cref="g1"/> is equals to <see cref="g2"/>?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isSelfLoop As Boolean
            Get
                Return String.Equals(g1, g2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, pathwayID, g1, g2)
        End Function

        Public Shared Function LoadData(path As String) As GraphEdge()
            Dim LQuery As GraphEdge() =
                LinqAPI.Exec(Of GraphEdge) <= From line As String
                                              In IO.File.ReadAllLines(path)
                                              Let tokens As String() = Strings.Split(line, vbTab)
                                              Select New GraphEdge With {
                                                  .pathwayID = tokens(0),
                                                  .g1 = tokens(1),
                                                  .g2 = tokens(2)
                                              }
            Return LQuery
        End Function
    End Class
End Namespace
