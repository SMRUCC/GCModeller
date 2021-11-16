#Region "Microsoft.VisualBasic::2b7219bf211ea6ff4419b4ff7e46e55f, models\GPML\PathVisio\Xml\Pathway.vb"

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

    '     Class Pathway
    ' 
    '         Properties: BiopaxRef, Comment, DataNode, Graphics, Group
    '                     InfoBox, Interaction, Label, Name, Organism
    '                     Version
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace GPML

    <XmlRoot("Pathway", [Namespace]:="http://pathvisio.org/GPML/2013a")>
    <XmlType("Pathway", [Namespace]:="http://pathvisio.org/GPML/2013a")>
    Public Class Pathway

        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property Version As String
        <XmlAttribute> Public Property Organism As String

        Public Property Comment As Comment

        <XmlElement>
        Public Property BiopaxRef As String()

        Public Property Graphics As Graphics

        <XmlElement> Public Property DataNode As DataNode()
        <XmlElement> Public Property Interaction As Interaction()
        <XmlElement> Public Property Label As Label()
        <XmlElement> Public Property Group As Group()
        Public Property InfoBox As InfoBox()
        ' Public Property Biopax As Biopax.Level3.File

    End Class

End Namespace
