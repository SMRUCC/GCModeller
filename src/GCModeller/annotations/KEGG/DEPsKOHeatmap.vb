
Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver

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

    End Function
End Module
