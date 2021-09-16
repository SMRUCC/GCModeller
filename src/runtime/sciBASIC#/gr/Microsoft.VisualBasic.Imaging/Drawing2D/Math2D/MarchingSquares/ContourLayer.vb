#Region "Microsoft.VisualBasic::c14976ff71966109b87da9052cfe370d, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Math2D\MarchingSquares\ContourLayer.vb"

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

'     Class ContourLayer
' 
'         Properties: dimension, shapes, threshold
' 
'         Function: GetContours
' 
'     Class Polygon2D
' 
'         Properties: x, y
' 
'         Function: ToArray
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq

Namespace Drawing2D.Math2D.MarchingSquares

    Public Class ContourLayer

        Public Property threshold As Double
        Public Property shapes As Polygon2D()
        Public Property dimension As Integer()

        Public Shared Iterator Function GetContours(sample As IEnumerable(Of MeasureData), Optional epsilon As Double = 0.00001) As IEnumerable(Of GeneralPath)
            Dim matrix As New MapMatrix(sample)
            Dim level_cutoff As Double() = matrix.GetPercentages
            Dim data As Double()() = matrix _
                .GetMatrixInterpolation _
                .MatrixTranspose _
                .ToArray

            For Each polygon As GeneralPath In New MarchingSquares(matrix.dimension, epsilon).mkIsos(data, levels:=level_cutoff)
                Yield polygon
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="threshold">[0, 100]</param>
        ''' <param name="epsilon"></param>
        ''' <returns></returns>
        Public Shared Function GetOutline(x As Double(), y As Double(),
                                          Optional threshold As Double = 85,
                                          Optional epsilon As Double = 0.00001) As GeneralPath

            Dim sample As MeasureData() = x.Select(Function(xi, i) New MeasureData(xi, y(i), 100)).ToArray
            Dim topleft As New MeasureData(0, 0, 0)
            Dim topright As New MeasureData(x.Max, y.Max, 0)
            Dim bottomleft As New MeasureData(0, y.Max, 0)
            Dim bottomright As New MeasureData(x.Max, 0, 0)
            Dim allRegions = sample _
                .JoinIterates({topleft, topright, bottomleft, bottomright}) _
                .DoCall(Function(poly) GetContours(poly, epsilon)) _
                .Where(Function(d) d.level >= threshold) _
                .OrderByDescending(Function(poly) poly.level) _
                .ToArray
            'Dim polygonRegion As New GeneralPath(1) With {
            '    .dimension = New Size(x.Max, y.Max)
            '}

            'For Each layer As GeneralPath In allRegions
            '    For Each polygon In layer.polygons
            '        Call polygonRegion.AddPolygon(polygon)
            '    Next
            'Next

            'Return polygonRegion

            Return allRegions.First
        End Function
    End Class

    Public Class Polygon2D

        Public Property x As Integer()
        Public Property y As Integer()

        Public Function ToArray() As PointF()
            Return x _
                .Select(Function(xi, i) New PointF(xi, y(i))) _
                .ToArray
        End Function
    End Class
End Namespace
