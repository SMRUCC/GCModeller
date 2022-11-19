#Region "Microsoft.VisualBasic::e479d62d6f20b1e98b52f1b2f1cd8dfc, GCModeller\annotations\Proteomics\CloudPlot.vb"

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

    '   Total Lines: 115
    '    Code Lines: 90
    ' Comment Lines: 15
    '   Blank Lines: 10
    '     File Size: 4.59 KB


    ' Module CloudPlot
    ' 
    '     Function: Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Data.GeneOntology

''' <summary>
''' X -> iBAQ表达量值
''' Y -> GO向量距离值
''' size -> logFC
''' color -> pvalue
''' </summary>
Public Module CloudPlot

    ''' <summary>
    ''' 绘制云图
    ''' </summary>
    ''' <param name="expression">原始数据</param>
    ''' <param name="annotations">蛋白注释结果</param>
    ''' <param name="DEPs">DEP计算结果文件</param>
    ''' <param name="schema$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(expression As EntityObject(), annotations As UniprotAnnotations(), DEPs As EntityObject(), tag$,
                         Optional schema$ = "Paired:c8",
                         Optional levels% = 125,
                         Optional sizeRange$ = "5,125",
                         Optional size$ = "2000,1600",
                         Optional margin$ = g.DefaultLargerPadding,
                         Optional bg$ = "white") As GraphicsData

        Dim colors As Color() = Designer.GetColors(schema, levels)
        Dim foldChanges = DEPs _
            .Select(Function(protein)
                        Dim P# = -Math.Log10(Val(protein("p.value")))
                        Return New NamedValue(Of (logFC#, P#)) With {
                            .Name = protein.ID,
                            .Value = (Val(protein("logFC")), P#)
                        }
                    End Function) _
            .ToArray

        Dim Plevels%() = foldChanges _
            .Select(Function(protein)
                        Dim P# = protein.Value.P
                        If P.IsNaNImaginary Then
                            Return 0
                        Else
                            Return P
                        End If
                    End Function) _
            .RangeTransform({0, levels - 1}) _
            .Select(Function(x) CInt(x)) _
            .ToArray
        Dim radius#() = foldChanges _
            .Select(Function(protein)
                        Return protein.Value.logFC
                    End Function) _
            .RangeTransform(DoubleRange.TryParse(sizeRange))
        Dim proteinID As Index(Of String) = foldChanges.Keys.Indexing
        Dim expressions =
            expression _
            .Select(Function(protein)
                        Return New NamedValue(Of Double) With {
                            .Name = protein.ID,
                            .Value = Val(protein(tag))
                        }
                    End Function) _
            .Sort(by:=proteinID)
        Dim PseAA As NamedValue(Of Vector)() =
            annotations _
            .Select(Function(protein)
                        Return New NamedValue(Of String()) With {
                            .Name = protein.ID,
                            .Value = protein.GO
                        }
                    End Function) _
            .Construct _
            .Sort(by:=proteinID)

        ' 表达数据和GO分类向量都是已经经过排序操作了的，可以直接使用

        Dim plotInternal =
            Sub(ByRef g As IGraphics, rect As GraphicsRegion)

                Dim pointsX = expressions _
                    .Values _
                    .RangeTransform(DoubleRange.TryParse(rect.XRange))
                Dim pointsY = PseAA _
                    .Values _
                    .Select(Function(v) v.Mod) _
                    .RangeTransform(DoubleRange.TryParse(rect.YRange))

                For i As Integer = 0 To foldChanges.Length - 1
                    Dim X = pointsX(i), Y = pointsY(i)
                    Dim r = radius(i)
                    Dim color As Color = colors(Plevels(i))
                    Dim circle As New Rectangle(X - r, Y - r, r * 2, r * 2)

                    Call g.FillPie(New SolidBrush(color), circle, 0, 360)
                Next
            End Sub

        Return GraphicsPlots(size.SizeParser, margin, bg, plotInternal)
    End Function
End Module
