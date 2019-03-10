﻿#Region "Microsoft.VisualBasic::37fa8a9e031008609f964d9b0b5d7a85, Data_science\Mathematica\Plot\Plots-statistics\PCA\PC2.vb"

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

    '     Module PCAPlot
    ' 
    '         Function: PC2
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports PCA_analysis = Microsoft.VisualBasic.Math.LinearAlgebra.PCA

Namespace PCA

    Public Module PCAPlot

        ''' <summary>
        ''' 将目标数据集通过PCA降维到二维数据，然后绘制散点图
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="sampleGroup%"></param>
        ''' <param name="labels$"></param>
        ''' <param name="size$"></param>
        ''' <param name="colorSchema$"></param>
        ''' <returns></returns>
        <Extension> Public Function PC2(input As GeneralMatrix,
                                        sampleGroup%,
                                        Optional labels$() = Nothing,
                                        Optional size$ = "2000,1800",
                                        Optional colorSchema$ = "Set1:c8") As GraphicsData

            Dim result = New PCA_analysis(input)  ' x, y
            Dim x As Vector
            Dim y As Vector

            With result.Project(input.RowVectors.ToArray, nPC:=2)
                x = .ByRef(0)
                y = .ByRef(1)
            End With

            Dim getlabel As Func(Of Integer, String)

            If labels.IsNullOrEmpty Then
                getlabel = Function(i) "#" & (i + 1).FormatZero()
            Else
                getlabel = Function(i) labels(i)
            End If

            Dim pts As Entity() = Points(x, y) _
            .SeqIterator _
            .Select(Function(pt)
                        Dim point As PointF = pt.value

                        Return New Entity With {
                            .uid = getlabel(pt.i),
                            .Properties = {
                                point.X,
                                point.Y
                            }
                        }
                    End Function) _
            .ToArray

            ' 进行聚类获取得到分组
            Dim kmeans As ClusterCollection(Of Entity) = pts.ClusterDataSet(sampleGroup)
            ' 赋值颜色到分组上
            Dim colors() = Designer.GetColors(colorSchema)
            ' 点为黑色的，border则才是所上的颜色
            Dim serials As New List(Of SerialData)

            For Each group In kmeans.SeqIterator
                Dim color As Color = colors(group)
                Dim stroke$ = New Stroke With {
                    .dash = DashStyle.Solid,
                    .fill = color.RGBExpression,
                    .width = 20
                }.ToString
                Dim points As PointData() = group _
                    .value _
                    .Select(Function(o)
                                Return New PointData With {
                                    .pt = New PointF(o(0), o(1)),
                                    .stroke = stroke
                                }
                            End Function) _
                    .ToArray
                Dim s As New SerialData With {
                    .color = Color.Black,
                    .PointSize = 5,
                    .title = "Cluster #" & (group.i + 1),
                    .pts = points
                }

                serials += s
            Next

            Dim dx = x.Max - x.Min
            Dim xaxis = $"({x.Min - dx / 5},{x.Max + dx / 5}),n=10"

            Return Bubble.Plot(serials, size.SizeParser, xAxis:=xaxis, strokeColorAsMainColor:=True)
        End Function
    End Module
End Namespace
