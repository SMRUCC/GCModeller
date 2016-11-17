Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace TrackDatas.Highlights

    Public Class MotifSites : Inherits GeneMark

        ''' <summary>
        ''' 这个构造函数会使用位点的类型的生成颜色谱
        ''' </summary>
        ''' <param name="sites"></param>
        Sub New(sites As IEnumerable(Of IMotifSite), Optional chr$ = "chr1")
            Dim array As IMotifSite() = sites.ToArray
            Dim types$() = array _
                .Select(Function(x) x.Type) _
                .Distinct _
                .ToArray

            MyBase.COGColors = CircosColor.ColorProfiles(types)

            __source = LinqAPI.MakeList(Of ValueTrackData) <=
 _
              From gene As IMotifSite
              In array
              Let COG As String = If(String.IsNullOrEmpty(gene.Type), "-", gene.Type)
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

        ''' <summary>
        ''' 使用分数来进行颜色的生成，而非使用位点的类型
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="levels%"></param>
        ''' <param name="mapName$"></param>
        Sub New(sites As IEnumerable(Of IMotifScoredSite),
                Optional levels% = 100,
                Optional mapName$ = ColorMap.PatternJet,
                Optional chr$ = "chr1")

            Dim array As IMotifScoredSite() = sites.ToArray
            Dim colors As Mappings() = array _
                .Select(Function(x) x.Score) _
                .GradientMappings(mapName, replaceBase:=False)

            MyBase.COGColors = colors _
                .GroupBy(Function(x) CStr(x.value)) _
                .ToDictionary(Function(x) x.Key,
                              Function(x) x.First.CircosColor)

            __source = LinqAPI.MakeList(Of ValueTrackData) <=
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