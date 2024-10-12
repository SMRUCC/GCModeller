﻿#Region "Microsoft.VisualBasic::b6f0adb617ec773303f8bc33cd4774c3, visualize\ChromosomeMap\DrawingModels\ChromesomeDrawingModel.vb"

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

    '   Total Lines: 71
    '    Code Lines: 26 (36.62%)
    ' Comment Lines: 38 (53.52%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (9.86%)
    '     File Size: 2.64 KB


    '     Class ChromesomeDrawingModel
    ' 
    '         Properties: COGs, Configuration, GeneObjects, Loci, MotifSiteColors
    '                     MotifSites, MutationDatas, TSSs
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

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

Namespace DrawingModels

    ''' <summary>
    ''' Data model for described a chromosome drawing action invoked.(用于描述如何绘制一个基因组的图形数据的数据模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ChromesomeDrawingModel : Inherits TabularFormat.Rpt

        ''' <summary>
        ''' 所需要进行绘制的基因组之中的基因对象，整个基因组之中的基本框架
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneObjects As SegmentObject()
        ''' <summary>
        ''' 基因的突变点的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property MutationDatas As MultationPointData()
        ''' <summary>
        ''' 绘图设备的配置数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Configuration As Configuration.DataReader
        ''' <summary>
        ''' 转录调控位点
        ''' </summary>
        ''' <returns></returns>
        Public Property MotifSites As MotifSite()
        ''' <summary>
        ''' 基因的转录起始位点
        ''' </summary>
        ''' <returns></returns>
        Public Property TSSs As TSSs()
        Public Property Loci As Loci()

        ''' <summary>
        ''' COG分类的颜色配置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property COGs As Dictionary(Of String, Brush)
        ''' <summary>
        ''' Motif位点的颜色配置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSiteColors As Dictionary(Of String, Color)

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("Drawing Model Brief Information:" & vbCrLf)
            Call sb.AppendLine($"GeneObject Segments:  {GeneObjects.TryCount}")
            Call sb.AppendLine($"Mutation Sites Data:  {MutationDatas.TryCount}")
            Call sb.AppendLine($"Motif Sites:          {MotifSites.TryCount}")
            Call sb.AppendLine($"Loci Sites:           {Loci.TryCount}")
            Call sb.AppendLine($"TSSs Sites:           {TSSs.TryCount}")

            Return sb.ToString
        End Function
    End Class
End Namespace
