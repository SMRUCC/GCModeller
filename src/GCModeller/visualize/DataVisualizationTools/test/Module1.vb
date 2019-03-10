Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Module Module1

    Sub Main()
        Dim rand As New Random
        Dim max# = 1000
        Dim getCategory = Function(i As Integer) As OrthologyProfile
                              Return New OrthologyProfile With {
                                  .Category = "category " & (i + 1),
                                  .HomologyDegrees = {
                                      New NamedValue(Of Color)("a", Color.Red, rand.NextDouble * max * 0.65),
                                      New NamedValue(Of Color)("b", Color.LightBlue, rand.NextDouble * max),
                                      New NamedValue(Of Color)("c", Color.SkyBlue, rand.NextDouble * max * 0.65),
                                      New NamedValue(Of Color)("d", Color.Blue, rand.NextDouble * max),
                                      New NamedValue(Of Color)("e", Color.DarkBlue, rand.NextDouble * max * 0.3),
                                      New NamedValue(Of Color)("f", Color.Gray, rand.NextDouble * max * 0.5)
                                  }
                              }
                          End Function

        Dim profiles = 20.SeqRandom.Select(Function(null) getCategory(null)).RenderColors.ToArray

        Call profiles.Plot.AsGDIImage.SaveAs("./test.png")

    End Sub
End Module
