Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace NCBIBlastResult

    Public Class OrthologyProfile

        ''' <summary>
        ''' 功能分组名称标签信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As String

        ''' <summary>
        ''' + name: degree
        ''' + value: color
        ''' + description: n genes
        ''' </summary>
        ''' <returns></returns>
        Public Property HomologyDegrees As NamedValue(Of Color)()

        Public ReadOnly Property Total As Integer
            Get
                Return Aggregate level As NamedValue(Of Color)
                       In HomologyDegrees
                       Let n = Val(level.Description)
                       Into Sum(n)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Category} ({Total})"
        End Function

    End Class

    Public Module OrthologyProfiles

        <Extension>
        Public Function OrthologyProfiles(result As IEnumerable(Of BBHIndex), colors As RangeList(Of Double, NamedValue(Of Color))) As OrthologyProfile()

        End Function

        <Extension>
        Public Function Plot(profileData As IEnumerable(Of OrthologyProfile),
                             Optional size$ = "3300,2700",
                             Optional margin$ = g.DefaultPadding,
                             Optional bg$ = "white",
                             Optional labelFontCSS$ = CSSFont.Win7LargeBold,
                             Optional boxBorderStrokeCSS$ = Stroke.AxisStroke) As GraphicsData

            Dim labelFont As Font = CSSFont.TryParse(labelFontCSS)
            Dim profiles As OrthologyProfile() = profileData.ToArray
            Dim maxCount% = profiles.Max(Function(category) category.Total)
            Dim maxLabel$ = profiles _
                .Select(Function(orth) orth.Category) _
                .MaxLengthString
            Dim boxStroke As Pen = Stroke.TryParse(boxBorderStrokeCSS)
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)

                    ' 所有的category标签加起来的总高度，这个总高度也是绘图区域的总高度
                    Dim labelSize As SizeF = g.MeasureString("A", labelFont)
                    Dim totalHeight# = labelSize.Height * profiles.Length + (labelSize.Height / 2) * (profiles.Length - 1)
                    Dim maxLabelWidth% = g.MeasureString(maxLabel, labelFont).Width

                    ' 绘图区域垂直居中显示
                    Dim top% = (region.Height - totalHeight) / 2
                    Dim left% = region.Padding.Left

                    ' 绘制盒子
                    ' 得到盒子的宽和高
                    Dim boxWidth = region.PlotRegion.Width - maxLabelWidth - 5
                    Dim boxHeight = totalHeight
                    Dim pos As New Point(left + maxLabelWidth, top)

                    ' 绘制盒子边框
                    Call g.DrawRectangle(boxStroke, New Rectangle(pos, New Size(boxWidth, boxHeight)))

                End Sub

            Return g.GraphicsPlots(
                size.SizeParser, margin, bg,
                plotAPI:=plotInternal
            )
        End Function
    End Module
End Namespace