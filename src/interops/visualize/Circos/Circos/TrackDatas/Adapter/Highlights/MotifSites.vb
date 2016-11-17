Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Namespace TrackDatas.Highlights

    Public Class MotifSites : Inherits GeneMark

        Sub New(sites As IEnumerable(Of IMotifSite))
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
                  .chr = "chr1",
                  .formatting = New Formatting With {
                      .fill_color = fill
                  }
              }
        End Sub
    End Class
End Namespace