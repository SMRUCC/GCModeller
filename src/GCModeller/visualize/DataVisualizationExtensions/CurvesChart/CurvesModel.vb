#Region "Microsoft.VisualBasic::0b9a48cdad941d8f981df4e8861aa875, visualize\DataVisualizationExtensions\CurvesChart\CurvesModel.vb"

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

    '   Total Lines: 47
    '    Code Lines: 39 (82.98%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (17.02%)
    '     File Size: 1.67 KB


    ' Class CurvesModel
    ' 
    '     Properties: PlotsHeight, ShowAverageLine, Steps, WindowSize, YValueOffset
    ' 
    '     Function: Draw, GraphicsDevice
    ' 
    ' Enum GraphicTypes
    ' 
    '     Curves, Histogram
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver


#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Public MustInherit Class CurvesModel

    <DataFrameColumn("slide_Window.size")>
    Public Property WindowSize As Integer = 500
    <DataFrameColumn("slide_Window.steps")>
    Public Property Steps As Integer = 20
    <DataFrameColumn("plot.height")>
    Public Property PlotsHeight As Integer = 100
    <DataFrameColumn("offset.aix_y")>
    Public Property YValueOffset As Integer = 40
    <DataFrameColumn("average.shows")>
    Public Property ShowAverageLine As Boolean = True

    Protected PlotBrush As SolidBrush = Brushes.DarkGreen

    Public Function Draw(source As Image, buf As Double(), location As Point, size As Size) As Image
        Dim sample As DataSample(Of Double) = buf.DoubleSample

        Using g As IGraphics = DriverLoad.CreateGraphicsDevice(source)
            Call Draw(g, sample, location, size)
            Return DirectCast(g, GdiRasterGraphics).ImageResource
        End Using
    End Function

    Protected MustOverride Sub Draw(ByRef source As IGraphics, data As DataSample(Of Double), location As Point, size As Size)

    Public Shared Function GraphicsDevice(type As GraphicTypes) As CurvesModel
        Select Case type
            Case GraphicTypes.Curves
                Return New Line
            Case GraphicTypes.Histogram
                Return New Histogram
            Case Else
                Return New Line
        End Select
    End Function
End Class

Public Enum GraphicTypes
    Curves
    Histogram
End Enum
