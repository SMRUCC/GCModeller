#Region "Microsoft.VisualBasic::8cf0fa5fb0bfc82dfe71c5e64c748d16, visualize\Circos\Circos\TrackDatas\Adapter\Highlights\MotifSites.vb"

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

    '     Class MotifSites
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports ColorPatterns = Microsoft.VisualBasic.Imaging.ColorMap

Namespace TrackDatas.Highlights

    Public Class MotifSites : Inherits GeneMark

        ''' <summary>
        ''' 这个构造函数会使用位点的类型的生成颜色谱
        ''' </summary>
        ''' <param name="sites"></param>
        Sub New(sites As IEnumerable(Of IMotifSite), Optional chr$ = "chr1")
            Dim array As IMotifSite() = sites.ToArray
            Dim types$() = array _
                .Select(Function(x) x.family) _
                .Distinct _
                .ToArray

            MyBase.COGColors = CircosColor.ColorProfiles(types)

            source = LinqAPI.MakeList(Of ValueTrackData) <=
 _
              From gene As IMotifSite
              In array
              Let COG As String = If(String.IsNullOrEmpty(gene.family), "-", gene.family)
              Let fill As String = If(
                  COGColors.ContainsKey(COG),
                  COGColors(COG),
                  CircosColor.DefaultCOGColor)
              Select New ValueTrackData With {
                  .start = CInt(gene.site.Left),
                  .end = CInt(gene.site.Right),
                  .value = 1,
                  .chr = chr,
                  .formatting = New Formatting With {
                      .fill_color = fill
                  }
              }
        End Sub

        ''' <summary>
        ''' 使用分数来进行颜色的生成，而非使用位点的类型
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="levels%"></param>
        ''' <param name="mapName$"></param>
        Sub New(sites As IEnumerable(Of IMotifScoredSite),
                Optional levels% = 100,
                Optional mapName$ = ColorPatterns.PatternJet,
                Optional chr$ = "chr1")

            Dim array As IMotifScoredSite() = sites.ToArray
            Dim colors As Mappings() = array _
                .Select(Function(x) x.Score) _
                .GradientMappings(mapName, replaceBase:=False)

            MyBase.COGColors = colors _
                .GroupBy(Function(x) CStr(x.value)) _
                .ToDictionary(Function(x) x.Key,
                              Function(x) x.First.CircosColor)

            source = LinqAPI.MakeList(Of ValueTrackData) <=
 _
                From gene As IMotifScoredSite
                In array
                Let COG As String = gene.Score.ToString
                Let fill As String = If(
                    COGColors.ContainsKey(COG),
                    COGColors(COG),
                    CircosColor.DefaultCOGColor)
                Select New ValueTrackData With {
                    .start = CInt(gene.Site.Left),
                    .end = CInt(gene.Site.Right),
                    .value = 1,
                    .chr = chr,
                    .formatting = New Formatting With {
                        .fill_color = fill
                    }
                }
        End Sub
    End Class
End Namespace
