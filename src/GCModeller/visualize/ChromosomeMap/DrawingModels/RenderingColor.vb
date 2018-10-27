#Region "Microsoft.VisualBasic::b2d572dcaa620fd562a2ff61f19d0753, ChromosomeMap\DrawingModels\RenderingColor.vb"

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
Imports SMRUCC.genomics.ComponentModel

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
        Public Function ApplyingCOGNumberColors(Of MyvaCOG As IFeatureDigest)(genes As MyvaCOG(), Chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim ColorProfiles = RenderingColor.InitCOGColors((From cogAlign In genes
                                                              Select cogAlign.Feature
                                                              Distinct).ToArray).ToDictionary(Function(obj) obj.Key,
                                                                                              Function(obj) DirectCast(New SolidBrush(obj.Value), Brush))
            Dim DefaultCogColor As New SolidBrush(Chromesome.Configuration.NoneCogColor)
            Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

            For Each gene As SegmentObject In Chromesome.GeneObjects
                Dim Cog As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)

                If Not Cog Is Nothing Then
                    If Not String.IsNullOrEmpty(Cog.Feature) Then
                        gene.Color = ColorProfiles(Cog.Feature)
                    Else
                        gene.Color = DefaultCogColor
                    End If
                Else
                    gene.Color = DefaultCogColor
                End If
            Next

            Call Chromesome.COGs.SetValue(ColorProfiles)

            Return Chromesome
        End Function
    End Module
End Namespace
