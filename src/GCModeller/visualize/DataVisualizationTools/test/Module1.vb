Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Module Module1

    Sub Main()

        Dim profiles = {
            New OrthologyProfile With {.Category = "category1", .HomologyDegrees = {
                New NamedValue(Of Color)("a", Color.Red, 180),
                New NamedValue(Of Color)("b", Color.LightBlue, 200),
                New NamedValue(Of Color)("c", Color.SkyBlue, 100),
                New NamedValue(Of Color)("d", Color.Blue, 260),
                New NamedValue(Of Color)("e", Color.DarkBlue, 50),
                New NamedValue(Of Color)("f", Color.Gray, 500)
                }
            },
            New OrthologyProfile With {.Category = "category1 XXXXXXXXX", .HomologyDegrees = {
                New NamedValue(Of Color)("a", Color.Red, 300),
                New NamedValue(Of Color)("b", Color.LightBlue, 500),
                New NamedValue(Of Color)("c", Color.SkyBlue, 190),
                New NamedValue(Of Color)("d", Color.Blue, 90),
                New NamedValue(Of Color)("e", Color.DarkBlue, 10),
                New NamedValue(Of Color)("f", Color.Gray, 300)
        }},
         New OrthologyProfile With {.Category = "category1 XXXXXXXXX skjfhsdkjfshdjkf",
            .HomologyDegrees = {
                New NamedValue(Of Color)("a", Color.Red, 30),
                New NamedValue(Of Color)("b", Color.LightBlue, 400),
                New NamedValue(Of Color)("c", Color.SkyBlue, 390),
                New NamedValue(Of Color)("d", Color.Blue, 290),
                New NamedValue(Of Color)("e", Color.DarkBlue, 10),
                New NamedValue(Of Color)("f", Color.Gray, 300)
        }}
        }

        Call profiles.Plot.AsGDIImage.SaveAs("./test.png")

    End Sub
End Module
