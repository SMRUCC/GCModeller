#Region "Microsoft.VisualBasic::42f1b731579d3b88ae6b37c742d53294, ..\GNUplot\Test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Module Program

    Sub Main()
        ' Call plotArray()
        ' Call plotXYScatter()
        ' Call plot3D()
        ' Call plotGridZ()
        ' Call plotPointCloud()
        ' Call plotSurface()
        Call plotHeatMap()

        Pause()
    End Sub

    Sub plotHeatMap()
        Dim Z#(,) = New Double(,) {
            {0, 0, 0, 1, 2, 2, 1, 0, 0, 0},
            {0, 0, 2, 3, 3, 3, 3, 2, 0, 0},
            {0, 2, 3, 4, 4, 4, 4, 3, 2, 0},
            {2, 3, 4, 5, 5, 5, 5, 4, 3, 2},
            {3, 4, 5, 6, 7, 7, 6, 5, 4, 3},
            {3, 4, 5, 6, 7, 7, 6, 5, 4, 3},
            {2, 3, 4, 5, 5, 5, 5, 4, 3, 2},
            {0, 2, 3, 4, 4, 4, 4, 3, 2, 0},
            {0, 0, 2, 3, 3, 3, 3, 2, 0, 0},
            {0, 0, 0, 1, 2, 2, 1, 0, 0, 0}
        }
        GNUplot.HeatMap(Z)
    End Sub

    Sub plotSurface()
        ' make 20 random data points
        Dim X#() = New Double(20) {}
        Dim Y#() = New Double(20) {}
        Dim Z#() = New Double(20) {}
        Dim r As New Random()

        For i% = 0 To 20
            X(i) = r.Next(30) - 15
            Y(i) = r.Next(50) - 25
            Z(i) = r.Next(20) - 10
        Next

        ' fit the points to a surface grid of 40x40 with smoothing level 2
        GNUplot.Set("dgrid3d 40,40,2")

        ' set the range for the x,y,z axis and plot (using pm3d to map height to color)
        GNUplot.Set("xrange[-30:30]", "yrange[-30:30]", "zrange[-30:30]")
        GNUplot.SPlot(X, Y, Z, "with pm3d")
    End Sub

    Sub plotArray()
        Dim Y#() = {-4, 6.5, -2, 3, -8, -5, 11, 4, -5, 10}
        GNUplot.Plot(Y)
    End Sub

    Sub plotXYScatter()
        Dim X#() = {-10, -8.5, -2, 1, 6, 9, 10, 14, 15, 19}
        Dim Y#() = {-4, 6.5, -2, 3, -8, -5, 11, 4, -5, 10}
        GNUplot.Plot(X, Y)
    End Sub

    Sub plot3D()
        GNUplot.SPlot("1 / (.05*x*x + .05*y*y + 1)")
    End Sub

    Sub plotGridZ()
        Dim Z#(,) = {{-4, -2.5, 1, 3}, {-3, -2, 3, 4}, {-1, 2, 6, 8}}
        GNUplot.Set("pm3d", "palette gray")         ' we'll make monochrome color based on height of the plane
        GNUplot.SPlot(Z, "with points pointtype 6") ' we'll try with points at vertexes instead of lines
    End Sub

    Sub plotPointCloud()
        ' make some random data points
        Dim X#() = New Double(100) {}
        Dim Y#() = New Double(100) {}
        Dim Z#() = New Double(100) {}
        Dim r As New Random()

        For i% = 0 To 100
            X(i) = r.Next(30) - 15
            Y(i) = r.Next(50) - 25
            Z(i) = r.Next(20) - 10
        Next

        ' set the range for the x,y,z axis and plot (using pointtype triangle and color blue)
        GNUplot.Set("xrange[-30:30]", "yrange[-30:30]", "zrange[-30:30]")
        GNUplot.SPlot(X, Y, Z, "with points pointtype 8 lc rgb 'blue'")
    End Sub
End Module

