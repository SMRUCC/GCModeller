﻿#Region "Microsoft.VisualBasic::af127daa6160d88da0fd13598c9b4526, visualize\ChromosomeMap\DrawingModels\RenderingColor.vb"

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

    '   Total Lines: 121
    '    Code Lines: 95 (78.51%)
    ' Comment Lines: 9 (7.44%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 17 (14.05%)
    '     File Size: 5.46 KB


    '     Module ModelColorExtensions
    ' 
    '         Function: ApplyingCOGCategoryColor, ApplyingCOGNumberColors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation

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

    Public Module ModelColorExtensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="MyvaCOG">需要实现有 <see cref="ICOGCatalog"/> 接口</typeparam>
        ''' <param name="genes"></param>
        ''' <param name="chromesome"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ApplyingCOGCategoryColor(Of MyvaCOG As ICOGCatalog)(
                        genes As MyvaCOG(),
                        chromesome As ChromesomeDrawingModel,
                        Optional alpha% = 230) As ChromesomeDrawingModel

            Dim colorProfiles As Dictionary(Of String, Brush) = RenderingColor _
                .InitCOGColors(Nothing, alpha) _
                .ToDictionary(Function(obj) obj.Key,
                              Function(obj) CType(New SolidBrush(obj.Value), Brush))
            With chromesome

                Dim defaultCOG_color As New SolidBrush(.Configuration.NoneCogColor)
                Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

                For Each gene As SegmentObject In .GeneObjects
                    Dim COG_gene As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)

                    If Not COG_gene Is Nothing AndAlso Not COG_gene.Catalog.StringEmpty Then

                        If COG_gene.Catalog.Length > 1 Then
                            Dim neuColor As Color = COG_gene.Catalog _
                                .Select(Function(c) DirectCast(
                                    colorProfiles.TryGetValue(
                                        CStr(c),
                                        [default]:=defaultCOG_color), SolidBrush).Color) _
                                .Average
                            gene.Color = New SolidBrush(neuColor)
                        Else
                            gene.Color = colorProfiles.TryGetValue(
                                COG_gene.Catalog,
                                [default]:=defaultCOG_color)
                        End If

                    Else
                        gene.Color = defaultCOG_color
                    End If
                Next

                ' 在这里需要填充一个空字符串表示cog not assign的颜色
                ' 否则在绘制图例的时候任然是默认色，而非这个配置文件指定的颜色
                colorProfiles("") = defaultCOG_color
                .COGs = colorProfiles
            End With

            Return chromesome
        End Function

        <Extension>
        Public Function ApplyingCOGNumberColors(Of MyvaCOG As IFeatureDigest)(genes As MyvaCOG(), chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim ColorProfiles = RenderingColor.InitCOGColors((From cogAlign In genes
                                                              Select cogAlign.Feature
                                                              Distinct).ToArray).ToDictionary(Function(obj) obj.Key,
                                                                                              Function(obj) DirectCast(New SolidBrush(obj.Value), Brush))
            Dim defaultCogColor As New SolidBrush(chromesome.Configuration.NoneCogColor)
            Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

            For Each gene As SegmentObject In chromesome.GeneObjects
                Dim COG As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)

                If Not COG Is Nothing Then
                    If Not String.IsNullOrEmpty(COG.Feature) Then
                        gene.Color = ColorProfiles(COG.Feature)
                    Else
                        gene.Color = defaultCogColor
                    End If
                Else
                    gene.Color = defaultCogColor
                End If
            Next

            ColorProfiles = chromesome.COGs

            Return chromesome
        End Function
    End Module
End Namespace
