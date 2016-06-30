Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Phylip.MatrixFile
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

<PackageNamespace("Phylip.IO")>
Module Phylip

    <IO_DeviceHandle(GetType(MatrixFile))>
    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As MatrixFile, saveCsv As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveCsv)
    End Function

    <IO_DeviceHandle(GetType(Gendist))>
    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As Gendist, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <IO_DeviceHandle(GetType(NeighborMatrix))>
    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As NeighborMatrix, saveCsv As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveCsv)
    End Function
End Module
