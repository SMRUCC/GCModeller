#Region "Microsoft.VisualBasic::17d9200c68c99fb0d6873c1859173e72, models\GPML\PathVisio\Xml\Network.vb"

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

    '     Enum DataNodeTypes
    ' 
    '         GeneProduct, Metabolite, Pathway, Protein
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class DataNode
    ' 
    '         Properties: Comment, Graphics, GraphId, GroupRef, TextLabel
    '                     Type, Xref
    ' 
    '         Function: ToString
    ' 
    '     Class Interaction
    ' 
    '         Properties: BiopaxRef, Graphics, GraphId, Xref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace GPML

    Public Enum DataNodeTypes
        GeneProduct
        Metabolite
        Protein
        Pathway
    End Enum

    Public Class DataNode

        <XmlAttribute> Public Property TextLabel As String
        <XmlAttribute> Public Property GraphId As String
        <XmlAttribute> Public Property Type As DataNodeTypes
        <XmlAttribute> Public Property GroupRef As String

        Public Property Comment As Comment
        Public Property Graphics As Graphics
        Public Property Xref As Xref

        Public Overrides Function ToString() As String
            Return $"[{GraphId}] {TextLabel} As {Type}"
        End Function

    End Class

    Public Class Interaction

        <XmlAttribute>
        Public Property GraphId As String
        Public Property BiopaxRef As String
        Public Property Graphics As Graphics
        Public Property Xref As Xref

        Public Overrides Function ToString() As String
            Return $"[{Graphics.Points(0).GraphRef} - {Graphics.Points(1).GraphRef}]"
        End Function

    End Class
End Namespace
