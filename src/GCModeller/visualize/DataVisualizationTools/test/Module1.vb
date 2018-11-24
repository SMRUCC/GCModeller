Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Module Module1

    Sub Main()

        Dim profiles = {
            New OrthologyProfile With {.Category = "category1", .HomologyDegrees = {
                New NamedValue(Of Color)(1, Color.Red, 180),
                New NamedValue(Of Color)(10, Color.LightBlue, 200),
                New NamedValue(Of Color)(20, Color.SkyBlue, 100),
                New NamedValue(Of Color)(30, Color.Blue, 260),
                New NamedValue(Of Color)(40, Color.DarkBlue, 50),
                New NamedValue(Of Color)(100, Color.Gray, 500)
                }
            },
            New OrthologyProfile With {.Category = "category1", .HomologyDegrees = {
                New NamedValue(Of Color)(1, Color.Red, 300),
                New NamedValue(Of Color)(10, Color.LightBlue, 500),
                New NamedValue(Of Color)(20, Color.SkyBlue, 190),
                New NamedValue(Of Color)(30, Color.Blue, 90),
                New NamedValue(Of Color)(40, Color.DarkBlue, 10),
                New NamedValue(Of Color)(100, Color.Gray, 300)
        }}
        }

        Call profiles.Plot.AsGDIImage.SaveAs("./test.png")

    End Sub
End Module
