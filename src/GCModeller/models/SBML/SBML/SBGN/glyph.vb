#Region "Microsoft.VisualBasic::5c38b8f4b80c167f9b3b24ec271cbd5d, G:/GCModeller/src/GCModeller/models/SBML/SBML//SBGN/glyph.vb"

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

    '   Total Lines: 66
    '    Code Lines: 47
    ' Comment Lines: 0
    '   Blank Lines: 19
    '     File Size: 1.80 KB


    '     Class glyph
    ' 
    '         Properties: [class], bbox, compartmentOrder, id, label
    '                     port
    ' 
    '         Function: ToString
    ' 
    '     Class port
    ' 
    '         Properties: id, x, y
    ' 
    '     Class label
    ' 
    '         Properties: bbox, text
    ' 
    '         Function: ToString
    ' 
    '     Class bbox
    ' 
    '         Properties: h, w, x, y
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace SBGN

    Public Class glyph

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property compartmentOrder As String

        Public Property label As label
        Public Property bbox As bbox

        <XmlElement>
        Public Property port As port()

        Public Overrides Function ToString() As String
            Return $"({[class]}) {id}: {label}"
        End Function

    End Class

    Public Class port

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

    End Class

    Public Class label

        <XmlAttribute>
        Public Property text As String
        Public Property bbox As bbox

        Public Overrides Function ToString() As String
            Return text
        End Function

    End Class

    Public Class bbox

        <XmlAttribute> Public Property w As Double
        <XmlAttribute> Public Property h As Double
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"({x},{y}) {{w:{w}, h:{h}}}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(bbox As bbox) As RectangleF
            If bbox Is Nothing Then
                Return Nothing
            Else
                Return New RectangleF(bbox.x, bbox.y, bbox.w, bbox.h)
            End If
        End Operator

    End Class
End Namespace
