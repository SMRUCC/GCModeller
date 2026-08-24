#Region "Microsoft.VisualBasic::9c790711ecd4fad63c4c88d23ad94b5d, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Elements\graphics.vb"

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

    '   Total Lines: 20
    '    Code Lines: 16 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (20.00%)
    '     File Size: 685 B


    '     Class graphics
    ' 
    '         Properties: bgcolor, fgcolor, height, name, type
    '                     width, x, y
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.KEGG.WebServices.KGML

    Public Class graphics

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property fgcolor As String
        <XmlAttribute> Public Property bgcolor As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double
        <XmlAttribute> Public Property width As Double
        <XmlAttribute> Public Property height As Double

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class
End Namespace
