#Region "Microsoft.VisualBasic::eac71c5a9a04ac9d88f526e05db7718c, models\SBML\SBML\SBGN\arc.vb"

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

    '   Total Lines: 61
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 1.66 KB


    '     Class arc
    ' 
    '         Properties: [class], [end], [next], glyph, id
    '                     source, start, target
    ' 
    '         Function: GetPoints, GetPolygon, ToString
    ' 
    '     Class point
    ' 
    '         Properties: x, y
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Namespace SBGN

    Public Class arc

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property source As String
        <XmlAttribute> Public Property target As String

        Public Property glyph As glyph

        Public Property start As point

        <XmlElement>
        Public Property [next] As point()

        Public Property [end] As point

        Public Overrides Function ToString() As String
            Return $"{source} -> {target}"
        End Function

        Public Function GetPolygon() As Polygon2D
            Dim points = GetPoints.ToArray
            Dim x = points.Select(Function(pt) pt.x).ToArray
            Dim y = points.Select(Function(pt) pt.y).ToArray

            Return New Polygon2D(x, y)
        End Function

        Private Iterator Function GetPoints() As IEnumerable(Of point)
            If start IsNot Nothing Then
                Yield start
            End If

            For Each point As point In [next].SafeQuery
                Yield point
            Next

            If [end] IsNot Nothing Then
                Yield [end]
            End If
        End Function

    End Class

    Public Class point

        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"[{x},{y}]"
        End Function

    End Class
End Namespace
