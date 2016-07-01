Imports System
Imports System.Drawing
Imports SMRUCC.genomics.GCModeller.DataVisualization.SyntenyVisual
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization

Module Program

    Sub New()
        VBDebugger.Mute = True
    End Sub

    Public Function Main() As Integer

        Dim list As Double() = "G:\5.14.circos\03.ZIKV_45_2015_updated_mafft_named.GCSkew.txt".ReadVector
        Dim img As New Bitmap(3000, 1000)
        Dim res = SMRUCC.genomics.GCModeller.DataVisualization.GCSkew.InvokeDrawingCurve(img, list, New Point(200, 850), New Size(2500, 800), SMRUCC.genomics.GCModeller.DataVisualization.GraphicTypes.Histogram)

        Call res.SaveAs("x:\dddd.png", ImageFormats.Png)

        Return GetType(Program).RunCLI(App.CommandLine, executeFile:=AddressOf ExecuteFile)
    End Function

    Private Function ExecuteFile(file As String, args As CommandLine) As Integer
        Dim model As DrawingModel = ModelAPI.GetDrawsModel(file)
        Dim res As Drawing.Image = model.Visualize
        Dim png As String = file.TrimFileExt & ".png"
        Return res.SaveAs(png, ImageFormats.Png).CLICode
    End Function
End Module
