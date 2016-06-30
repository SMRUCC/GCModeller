Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace PlasmidMap

    <[PackageNamespace]("Data.Visualization.Plasmid_Map", Description:="Data visualization module for the bacteria plasmid object.", Category:=APICategories.UtilityTools)>
    Public Module ShellScriptAPI

#Const DEBUG = 1

#If DEBUG Then
        <ExportAPI("test_debug()", Info:="Just for debugging...")>
        Public Function TestDEBUG() As Boolean
            Dim model As New PlasmidMap.PlasmidMapDrawingModel With {
                .GeneObjects = {New DrawingModels.SegmentObject With {
                .LocusTag = "TEST_1",
                .Direction = 0,
                .CommonName = "TEST_Annotations_TEXT",
                .Left = 100,
                .Right = 200,
                .GenomeLength = 600,
                .Color = Color.Black}}}
            Call New DrawingDevice().InvokeDrawing(model).Save(My.Computer.FileSystem.SpecialDirectories.Temp & "/Test.bmp")
            Return True
        End Function
#End If
    End Module
End Namespace