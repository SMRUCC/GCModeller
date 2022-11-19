#Region "Microsoft.VisualBasic::6b6124adf9699f226883d3478369ed48, GCModeller\visualize\DataVisualizationExtensions\GCSkew.vb"

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

    '   Total Lines: 118
    '    Code Lines: 74
    ' Comment Lines: 32
    '   Blank Lines: 12
    '     File Size: 4.87 KB


    ' Module GCSkew
    ' 
    '     Properties: PlotsHeight, ShowAverageLine, Steps, WindowSize, YValueOffset
    ' 
    '     Function: __getLoci, DrawingCurve, InvokeDrawing, InvokeDrawingGCContent, SetPlotBrush
    ' 
    '     Sub: DrawAixs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' plotting the GC% and GC skew curve.
''' </summary>
<Package("NT.GC.Curve", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module GCSkew

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

    Dim PlotBrush As SolidBrush = DirectCast(Brushes.DarkGreen, SolidBrush)

    <ExportAPI("Plot.Set.Color")>
    Public Function SetPlotBrush(r As Integer, g As Integer, b As Integer, Optional alpha As Integer = 220) As Color
        Dim cl = Color.FromArgb(alpha, r, g, b)
        PlotBrush = New SolidBrush(cl)
        Return cl
    End Function

    ''' <summary>
    ''' 将GC含量曲线绘制到目标比对图形<paramref name="source"></paramref>之上
    ''' </summary>
    ''' <param name="source"></param> 
    ''' <param name="nt">Attributes的 1 和 2 分别为nt的开始和结束的位置</param>
    ''' <param name="Location">坐标轴原点的位置</param>
    ''' <param name="Width">坐标轴纵轴的宽度</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("GC.Content.Drawing")>
    Public Function InvokeDrawingGCContent(source As Image, nt As FASTA.FastaSeq, Location As Point, Width As Integer) As Image
        Dim GCContent As Double() = NucleotideModels.GCContent(nt, WindowSize, Steps, True)
        Return DrawingCurve(source, GCContent, Location, New Size(Width, PlotsHeight)) ', loci:=__getLoci(nt))
    End Function

    Private Function __getLoci(nt As FASTA.FastaSeq) As Loci.Location
        If nt.Headers.Length < 3 Then
            Dim List As New List(Of String)
            Call List.AddRange(nt.Headers)
            Call List.Add("")
            Call List.Add("")
            nt.Headers = List.ToArray
        End If

        Dim Loci = New Loci.Location(Val(nt.Headers(1)), Val(nt.Headers(2)))
        If Loci.Right = 0 Then
            Loci.Right = nt.Length
        End If

        Return Loci
    End Function

    ''' <summary>
    ''' 绘制基本的坐标轴
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="location"></param>
    ''' <param name="size"></param>
    ''' <param name="tagFont"></param>
    Public Sub DrawAixs(g As IGraphics, location As Point, size As Size, tagFont As Font, min As Double, max As Double)
        Dim aixsSize As SizeF = g.MeasureString("0", tagFont)
        Dim vertex As New Point(location.X, location.Y - size.Height)
        Dim tmpLoci As New Point With {
            .X = location.X - YValueOffset,
            .Y = location.Y - aixsSize.Height / 2
        }
        Dim minLab$ = min.ToString("F2")
        Dim maxLab$ = max.ToString("F2")

        Call g.DrawString(minLab, font:=tagFont, brush:=Brushes.Black, point:=tmpLoci)
        Call g.DrawString(maxLab, tagFont, Brushes.Black, New Point(vertex.X - YValueOffset, vertex.Y - aixsSize.Height / 2))
    End Sub

    <ExportAPI("Curve.Drawing")>
    Public Function DrawingCurve(source As Image,
                                 buf As Double(),
                                 location As Point,
                                 size As Size,
                                 Optional type As GraphicTypes = GraphicTypes.Histogram) As Image
        Return CurvesModel.GraphicsDevice(type).Draw(source, buf, location, size)
    End Function

    ''' <summary>
    ''' 将GC偏移曲线绘制到目标比对图形<paramref name="source"></paramref>之上
    ''' </summary>
    ''' <param name="source"></param> 
    ''' <param name="nt"></param>
    ''' <param name="Location">坐标轴原点的位置</param>
    ''' <param name="Width">坐标轴纵轴的宽度</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("GCSkew.Drawing")>
    Public Function InvokeDrawing(source As Image, nt As FASTA.FastaSeq, Location As Point, Width As Integer) As Image
        ' 绘制gc偏移曲线
        Dim Skew As Double() = NucleotideModels.GCSkew(nt, WindowSize, Steps, True)
        Return DrawingCurve(source,
                            buf:=Skew,
                            location:=Location,
                            size:=New Size(Width, PlotsHeight))
        'loci:=__getLoci(nt))
    End Function
End Module
