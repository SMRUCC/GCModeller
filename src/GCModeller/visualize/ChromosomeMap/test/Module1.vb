Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels
Imports SMRUCC.genomics.Visualize.ChromosomeMap.PlasmidMap

Module Module1

    Sub Main()

    End Sub

    <ExportAPI("test_debug()", Info:="Just for debugging...")>
    Public Function TestDEBUG() As Boolean
        Dim model As New PlasmidMapDrawingModel With {
            .GeneObjects = {
            New SegmentObject With {
            .LocusTag = "TEST_1",
            .Direction = 0,
            .CommonName = "TEST_Annotations_TEXT",
            .Left = 100,
            .Right = 200,
            .Color = Brushes.Black}},
            .genomeSize = 600
        }
        Call DrawingDevice.DrawMap(model).Save("./Test.png")
        Return True
    End Function
End Module
