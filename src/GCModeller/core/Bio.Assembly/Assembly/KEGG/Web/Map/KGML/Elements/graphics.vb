#Region "Microsoft.VisualBasic::fcb55429da4b7272bd20a4cba2513236, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Components.vb"

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

'   Total Lines: 185
'    Code Lines: 73 (39.46%)
' Comment Lines: 85 (45.95%)
'    - Xml Docs: 96.47%
' 
'   Blank Lines: 27 (14.59%)
'     File Size: 9.44 KB


'     Class Link
' 
'         Properties: type
' 
'         Function: ToString
' 
'     Class relation
' 
'         Properties: entry1, entry2, subtype
' 
'     Class subtype
' 
'         Properties: name, value
' 
'     Class compound
' 
'         Properties: id, name
' 
'         Function: ToString
' 
'     Class reaction
' 
'         Properties: id, name, products, substrates
' 
'         Function: GetModel, ToString
' 
'     Class entry
' 
'         Properties: graphics, id, link, name, reaction
'                     type
' 
'         Function: ToString
' 
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
