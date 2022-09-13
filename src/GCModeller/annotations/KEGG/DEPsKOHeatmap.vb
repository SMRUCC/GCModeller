#Region "Microsoft.VisualBasic::e171cc2970e9191f8efb1b72a9edd6e5, GCModeller\annotations\KEGG\DEPsKOHeatmap.vb"

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

    '   Total Lines: 78
    '    Code Lines: 51
    ' Comment Lines: 20
    '   Blank Lines: 7
    '     File Size: 3.55 KB


    ' Module DEPsKOHeatmap
    ' 
    '     Function: ColorKO, KOColor, Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

''' <summary>
''' 绘制差异表达蛋白或者基因的``log2FC``对KO分类的热图作图
''' </summary>
Public Module DEPsKOHeatmap

    ''' <summary>
    ''' + 热图的矩阵顶部的颜色条为实验的分组信息
    ''' + 左边的层次聚类树为DEPs在试验间的变化模式的聚类
    ''' + 层次聚类树的颜色条表示KO的分类
    ''' + 热图矩阵的底部标记为实验的分组标签
    ''' </summary>
    ''' <param name="KOInfo">``<see cref="DataSet.ID"/> -> KO``</param>
    ''' <param name="KOcolor">KEGG大分类的颜色</param>
    ''' <param name="groupColors">实验分组的颜色</param>
    ''' <param name="groupInfo">``<see cref="DataSet.ID"/> -> group``</param>
    ''' <returns></returns>
    Public Function Plot(matrix As IEnumerable(Of DataSet),
                         groupInfo As Dictionary(Of String, String),
                         groupColors As Dictionary(Of String, Color),
                         KOInfo As Dictionary(Of String, String),
                         Optional KOcolor As Dictionary(Of String, Color) = Nothing,
                         Optional size$ = "3000,2700",
                         Optional padding$ = g.DefaultLargerPadding,
                         Optional bg$ = "white",
                         Optional schema$ = ColorBrewer.QualitativeSchemes.Paired6,
                         Optional title$ = "KEGG KO log2FC heatmap") As GraphicsData

        Dim groupClassColor = groupInfo.ToDictionary(
            Function(x) x.Key,
            Function(x) groupColors(x.Value).ToHtmlColor)

        Return Heatmap.Plot(
            matrix, size:=size, padding:=padding, mapName:=schema, mainTitle:=title, bg:=bg,
            drawClass:=(KOInfo.ColorKO(KOcolor), groupClassColor))
    End Function

    ''' <summary>
    ''' 使用内部的数据库对每一个蛋白质的自己的KO进行大分类的颜色赋值
    ''' </summary>
    ''' <param name="koInfo"></param>
    ''' <param name="KOcolor"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ColorKO(koInfo As Dictionary(Of String, String), KOcolor As Dictionary(Of String, Color)) As Dictionary(Of String, String)
        Dim ko00000 As Dictionary(Of String, BriteHText()) = KOCatalog.ko00000
        Dim colors = koInfo.ToDictionary(
            Function(x) x.Key,
            Function(KO)
                Return KO _
                    .KOColor(ko00000, KOcolor) _
                    .ToHtmlColor
            End Function)
        Return colors
    End Function

    <Extension>
    Private Function KOColor(KO As KeyValuePair(Of String, String),
                             ko00000 As Dictionary(Of String, BriteHText()),
                             colors As Dictionary(Of String, Color)) As Color

        Dim [class] = ko00000(KO.Value) _
            .Select(Function(h) h.GetRoot.ClassLabel) _
            .ToArray
        Dim allColors = [class].Select(Function(c) colors(c))
        Dim average As Color = allColors.Average
        Return average
    End Function
End Module
