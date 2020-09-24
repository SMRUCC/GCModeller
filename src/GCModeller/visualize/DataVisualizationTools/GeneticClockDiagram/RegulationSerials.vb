#Region "Microsoft.VisualBasic::731bfdb2c6f9d822f0c4289dcefc087c, visualize\DataVisualizationTools\GeneticClockDiagram\RegulationSerials.vb"

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

    ' Module RegulationSerials
    ' 
    '     Function: __directDrawing, CalculateOffset, CompareDrawing, Distributions, (+2 Overloads) InvokeDrawing
    '               MeasureMaxSize
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models

<[Namespace]("plot.regulation.serials", Description:="")> Module RegulationSerials

    Dim PointWidth As Integer = 10
    Dim PointHeight As Integer = 1
    Dim Margin As Integer = 5

    <ExportAPI("plot.serials.doppler.offset")>
    Public Function InvokeDrawing(data As Generic.IEnumerable(Of NumericVector)) As Image
        data = (From row In data.AsParallel Select sd = CalculateOffset(row) Order By sd.name Ascending).ToArray '数据视图转换
        Return __directDrawing(data)
    End Function

    <ExportAPI("plot.serials.doppler.offset")>
    Public Function InvokeDrawing(data As IEnumerable(Of NumericVector), IDList As IEnumerable(Of String)) As Image
        Dim IDc = IDList.ToArray
        data = (From row In data.AsParallel Where Array.IndexOf(IDc, row.name) > -1 Select row).ToArray '数据筛选
        Return InvokeDrawing(data)
    End Function

    ''' <summary>
    ''' 同一个基因在不同的突变株之间的相互比较
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="IDList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("plot.expression.compares")>
    Public Function CompareDrawing(source As String, IDList As IEnumerable(Of String)) As Image
        Dim IDc = IDList.ToArray
        Dim Data = (From path In source.LoadSourceEntryList({"*.csv"}).AsParallel
                    Select ID = path.Key, dat = (From row
                                                 In path.Value.LoadXml(Of NumericVector())
                                                 Where Array.IndexOf(IDc, row.name) > -1
                                                 Select row).ToArray).ToArray
        Data = (From row In Data.AsParallel Select ID = row.ID, dat = (From obj In row.dat Select sd = CalculateOffset(obj) Order By sd.name Ascending).ToArray).ToArray '数据视图转换

        Dim Y As Integer = Margin
        Dim X As Integer = Margin

        Dim GroupData = (From item In (From row In Data Select (From obj In row.dat Select row.ID, obj.vector, obj.name).ToArray).ToArray.Unlist Select item Group By item.name Into Group).ToArray
        Dim ReGen = (From gr In GroupData Select GeneID = gr.name, Expr = (From item In gr.Group.ToArray Select sd = New NumericVector With {.name = item.ID, .vector = item.vector} Order By sd.name Ascending).ToArray).ToArray
        Dim InvokeDrawingLQuery = (From item In ReGen Select GeneID = item.GeneID, res = __directDrawing(item.Expr)).ToArray
        Dim IDFont = New Font(FontFace.MicrosoftYaHei, 12)
        Dim FontSize = MeasureMaxSize((From item In ReGen Select item.GeneID).ToArray, IDFont)

        Using g As Graphics2D = New Size(Margin * 4 + InvokeDrawingLQuery.First.res.Width, Margin * 5 + (InvokeDrawingLQuery.First.res.Height + FontSize.Height + 5) * ReGen.Count).CreateGDIDevice
            For Each item In InvokeDrawingLQuery
                Call g.Graphics.DrawString(item.GeneID, IDFont, Brushes.Black, New Point((g.Width - item.GeneID.MeasureSize(g, IDFont).Width) / 2, Y))
                Y += FontSize.Height + 5
                Call g.Graphics.DrawImage(item.res, Margin, Y)
                Y += item.res.Height + 5
            Next

            Return g.ImageResource
        End Using
    End Function

    <ExportAPI("level.distribution")>
    Public Function Distributions(source As String, IDList As IEnumerable(Of String)) As IO.File
        Dim IDc = IDList.ToArray
        Dim Data = (From path In source.LoadSourceEntryList({"*.csv"}).AsParallel Select ID = path.Key, dat = (From row In path.Value.LoadXml(Of NumericVector()) Where Array.IndexOf(IDc, row.name) > -1 Select row).ToArray).ToArray
        Data = (From row In Data.AsParallel Select ID = row.ID, dat = (From obj In row.dat Select sd = CalculateOffset(obj) Order By sd.name Ascending).ToArray).ToArray '数据视图转换
        Dim GroupData = (From item In (From row In Data Select (From obj In row.dat Select row.ID, obj.vector, obj.name).ToArray).ToArray.Unlist Select item Group By item.name Into Group).ToArray
        Dim ReGen = (From gr In GroupData Select GeneID = gr.name, Expr = (From item In gr.Group.ToArray Select sd = New NumericVector With {.name = item.ID, .vector = item.vector} Order By sd.name Ascending).ToArray).ToArray
        Dim CSV As New IO.File

        For Each Gene In ReGen
            Dim Chunk = (From Experiment In Gene.Expr Select Experiment.vector).Unlist
            Dim Mapping = GenerateMapping(Chunk) '映射水平
            Dim i As Integer = 0
            Dim Levels = (From n In Mapping Select n Distinct Order By n Ascending).ToArray

            Call CSV.Add({Gene.GeneID})
            Call CSV.Last.AddRange((From lv In Levels Select s = lv.ToString).ToArray)

            For Each Expr In Gene.Expr
                Dim ChunkBuffer As Integer() = New Integer(Expr.vector.Length - 1) {}
                Call Array.ConstrainedCopy(Mapping, i, ChunkBuffer, 0, ChunkBuffer.Length)
                i += ChunkBuffer.Length
                Dim Row = New List(Of String) From {Expr.name}
                Call CSV.Add(Row)
                Call Row.AddRange((From lv In Levels Select s = (From n In ChunkBuffer Where n = lv Select 1).ToArray.Count.ToString).ToArray)
            Next

            Call CSV.AppendLine()
            Call CSV.AppendLine()
        Next

        Return CSV
    End Function

    Private Function MeasureMaxSize(StringCol As String(), Font As Font) As Size
        With New Size(1, 1).CreateGDIDevice
            Dim LQuery = (From s As String In StringCol Select s.MeasureSize(.ByRef, Font)).ToArray
            Dim sz As New Size((From item In LQuery Select item.Width).Max, (From item In LQuery Select item.Height).Max)
            Return sz
        End With
    End Function

    Private Function __directDrawing(data As NumericVector()) As Image
        Dim Y As Integer = Margin
        Dim X As Integer = Margin
        '创建颜色映射
        Dim ColorMappings = New GeneticClock.ColorRender(data, [Global]:=False)
        Dim TagDrawingFont As Font = New Font(FontFace.MicrosoftYaHei, 10)
        Dim StringSize As SizeF()

        With New Size(1, 1).CreateGDIDevice
            StringSize = (From s In data Select s.name.MeasureSize(.ByRef, TagDrawingFont)).ToArray
        End With

        Dim sX = (From sz In StringSize Select n = sz.Height).Max

        If sX > PointHeight Then
            PointHeight = sX + 1
        End If

        sX = (From sz In StringSize Select n = sz.Width).Max

        Dim width As Integer = data.First.Length * PointWidth + 20 * Margin + sX
        Dim Height As Integer = data.Count * PointHeight + 2 * Margin
        Dim g = (New Size(width, Height)).CreateGDIDevice '创建绘图设备

        Dim Mappings = ColorMappings.GetColorRenderingProfiles
        Dim PointSize = New Size(PointWidth, PointHeight)

        '绘图
        For i As Integer = 0 To data.Count - 1
            Dim Line = data(i)

            Call g.Graphics.DrawString(Line.name, New Font(FontFace.MicrosoftYaHei, 12), Brushes.Black, New Point(Margin, Y))

            X = Margin + sX + 20

            Dim Map = Mappings(i)

            For Each p As Double In Line.vector
                Call g.Graphics.FillRectangle(New SolidBrush(Map.GetValue(p)), New Rectangle(New Point(X, Y), PointSize))
                X += RegulationSerials.PointWidth
            Next

            Dim Avg As Double = (Map.Max - Map.Average) * 0.25 + Map.Average

            Call g.Graphics.DrawString((From item In Map.Profiles Where item.Key > Avg Select item).ToArray.Count, New Font(FontFace.MicrosoftYaHei, 12), Brushes.Black, New Point(X + 5, Y))

            Y += PointHeight
        Next

        Return g.ImageResource
    End Function

    Private Function CalculateOffset(data As NumericVector) As NumericVector
        Dim ChunkBuffer As Double() = New Double(data.vector.Count - 2) {}
        Dim pre As Double = data.vector.First

        For i As Integer = 1 To ChunkBuffer.Count - 1
            Dim n As Double = data.vector(i)
            ChunkBuffer(i - 1) = Math.Abs(n - pre)
            pre = n
        Next

        Return New NumericVector With {
            .vector = ChunkBuffer,
            .name = data.name
        }
    End Function
End Module
