#Region "Microsoft.VisualBasic::71463244aa44624763ce8de0eb49fb4d, ..\visualize\visualizeTools\GeneticClockDiagram\RegulationSerials.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.InteractionModel

<[Namespace]("plot.regulation.serials", Description:="")> Module RegulationSerials

    Dim PointWidth As Integer = 10
    Dim PointHeight As Integer = 1
    Dim Margin As Integer = 5

    <ExportAPI("plot.serials.doppler.offset")>
    Public Function InvokeDrawing(data As Generic.IEnumerable(Of SerialsData)) As System.Drawing.Image
        data = (From row In data.AsParallel Select sd = CalculateOffset(row) Order By sd.Tag Ascending).ToArray '数据视图转换
        Return __directDrawing(data)
    End Function

    <ExportAPI("plot.serials.doppler.offset")>
    Public Function InvokeDrawing(data As Generic.IEnumerable(Of SerialsData), IDList As Generic.IEnumerable(Of String)) As System.Drawing.Image
        Dim IDc = IDList.ToArray
        data = (From row In data.AsParallel Where Array.IndexOf(IDc, row.Tag) > -1 Select row).ToArray '数据筛选
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
    Public Function CompareDrawing(source As String, IDList As Generic.IEnumerable(Of String)) As Image
        Dim IDc = IDList.ToArray
        Dim Data = (From path In source.LoadSourceEntryList({"*.csv"}).AsParallel
                    Select ID = path.Key, dat = (From row In DataServicesExtension.LoadData(path.Value)
                                                 Where Array.IndexOf(IDc, row.Tag) > -1
                                                 Select row).ToArray).ToArray
        Data = (From row In Data.AsParallel Select ID = row.ID, dat = (From obj In row.dat Select sd = CalculateOffset(obj) Order By sd.Tag Ascending).ToArray).ToArray '数据视图转换

        Dim Y As Integer = Margin
        Dim X As Integer = Margin

        Dim GroupData = (From item In (From row In Data Select (From obj In row.dat Select row.ID, obj.ChunkBuffer, obj.Tag).ToArray).ToArray.Unlist Select item Group By item.Tag Into Group).ToArray
        Dim ReGen = (From gr In GroupData Select GeneID = gr.Tag, Expr = (From item In gr.Group.ToArray Select sd = New SerialsData With {.Tag = item.ID, .ChunkBuffer = item.ChunkBuffer} Order By sd.Tag Ascending).ToArray).ToArray
        Dim InvokeDrawingLQuery = (From item In ReGen Select GeneID = item.GeneID, res = __directDrawing(item.Expr)).ToArray
        Dim IDFont = New Font(FontFace.MicrosoftYaHei, 12)
        Dim FontSize = MeasureMaxSize((From item In ReGen Select item.GeneID).ToArray, IDFont)
        Dim GRdwh = New Size(Margin * 4 + InvokeDrawingLQuery.First.res.Width, Margin * 5 + (InvokeDrawingLQuery.First.res.Height + FontSize.Height + 5) * ReGen.Count).CreateGDIDevice

        For Each item In InvokeDrawingLQuery
            Call GRdwh.Graphics.DrawString(item.GeneID, IDFont, Brushes.Black, New Point((GRdwh.Width - item.GeneID.MeasureString(IDFont).Width) / 2, Y))
            Y += FontSize.Height + 5
            Call GRdwh.Graphics.DrawImage(item.res, Margin, Y)
            Y += item.res.Height + 5
        Next

        Return GRdwh.ImageResource
    End Function

    <ExportAPI("level.distribution")>
    Public Function Distributions(source As String, IDList As IEnumerable(Of String)) As IO.File
        Dim IDc = IDList.ToArray
        Dim Data = (From path In source.LoadSourceEntryList({"*.csv"}).AsParallel Select ID = path.Key, dat = (From row In DataServicesExtension.LoadData(path.Value) Where Array.IndexOf(IDc, row.Tag) > -1 Select row).ToArray).ToArray
        Data = (From row In Data.AsParallel Select ID = row.ID, dat = (From obj In row.dat Select sd = CalculateOffset(obj) Order By sd.Tag Ascending).ToArray).ToArray '数据视图转换
        Dim GroupData = (From item In (From row In Data Select (From obj In row.dat Select row.ID, obj.ChunkBuffer, obj.Tag).ToArray).ToArray.Unlist Select item Group By item.Tag Into Group).ToArray
        Dim ReGen = (From gr In GroupData Select GeneID = gr.Tag, Expr = (From item In gr.Group.ToArray Select sd = New SerialsData With {.Tag = item.ID, .ChunkBuffer = item.ChunkBuffer} Order By sd.Tag Ascending).ToArray).ToArray
        Dim CSV As New IO.File

        For Each Gene In ReGen
            Dim Chunk = (From Experiment In Gene.Expr Select Experiment.ChunkBuffer).Unlist
            Dim Mapping = GenerateMapping(Chunk) '映射水平
            Dim i As Integer = 0
            Dim Levels = (From n In Mapping Select n Distinct Order By n Ascending).ToArray

            Call CSV.Add({Gene.GeneID})
            Call CSV.Last.AddRange((From lv In Levels Select s = lv.ToString).ToArray)

            For Each Expr In Gene.Expr
                Dim ChunkBuffer As Integer() = New Integer(Expr.ChunkBuffer.Length - 1) {}
                Call Array.ConstrainedCopy(Mapping, i, ChunkBuffer, 0, ChunkBuffer.Length)
                i += ChunkBuffer.Length
                Dim Row = New List(Of String) From {Expr.Tag}
                Call CSV.Add(Row)
                Call Row.AddRange((From lv In Levels Select s = (From n In ChunkBuffer Where n = lv Select 1).ToArray.Count.ToString).ToArray)
            Next

            Call CSV.AppendLine()
            Call CSV.AppendLine()
        Next

        Return CSV
    End Function

    Private Function MeasureMaxSize(StringCol As String(), Font As Font) As Size
        Dim LQuery = (From s As String In StringCol Select s.MeasureString(Font)).ToArray
        Dim sz = New Size((From item In LQuery Select item.Width).ToArray.Max, (From item In LQuery Select item.Height).ToArray.Max)
    End Function

    Private Function __directDrawing(data As SerialsData()) As Image
        Dim Y As Integer = Margin
        Dim X As Integer = Margin
        '创建颜色映射
        Dim ColorMappings = New GeneticClock.ColorRender(data, [Global]:=False)
        Dim TagDrawingFont As Font = New Font(FontFace.MicrosoftYaHei, 10)
        Dim StringSize = (From s In data Select s.Tag.MeasureString(TagDrawingFont)).ToArray

        Dim sX = (From sz In StringSize Select n = sz.Height).ToArray.Max

        If sX > PointHeight Then
            PointHeight = sX + 1
        End If

        sX = (From sz In StringSize Select n = sz.Width).ToArray.Max

        Dim width As Integer = data.First.Length * PointWidth + 20 * Margin + sX
        Dim Height As Integer = data.Count * PointHeight + 2 * Margin
        Dim Gr = (New Size(width, Height)).CreateGDIDevice '创建绘图设备

        Dim Mappings = ColorMappings.GetColorRenderingProfiles
        Dim PointSize = New Size(PointWidth, PointHeight)

        '绘图
        For i As Integer = 0 To data.Count - 1
            Dim Line = data(i)

            Call Gr.Graphics.DrawString(Line.Tag, New Font(FontFace.MicrosoftYaHei, 12), Brushes.Black, New Point(Margin, Y))

            X = Margin + sX + 20

            Dim Map = Mappings(i)

            For Each p In Line
                Call Gr.Graphics.FillRectangle(New SolidBrush(Map.GetValue(p)), New Rectangle(New Point(X, Y), PointSize))
                X += RegulationSerials.PointWidth
            Next

            Dim Avg As Double = (Map.Max - Map.Average) * 0.25 + Map.Average

            Call Gr.Graphics.DrawString((From item In Map.Profiles Where item.Key > Avg Select item).ToArray.Count, New Font(FontFace.MicrosoftYaHei, 12), Brushes.Black, New Point(X + 5, Y))

            Y += PointHeight
        Next

        Return Gr.ImageResource
    End Function

    Private Function CalculateOffset(data As SerialsData) As SerialsData
        Dim ChunkBuffer As Double() = New Double(data.ChunkBuffer.Count - 2) {}
        Dim pre As Double = data.ChunkBuffer.First

        For i As Integer = 1 To ChunkBuffer.Count - 1
            Dim n As Double = data.ChunkBuffer(i)
            ChunkBuffer(i - 1) = Math.Abs(n - pre)
            pre = n
        Next

        Return New SerialsData With {.ChunkBuffer = ChunkBuffer, .Tag = data.Tag}
    End Function
End Module
