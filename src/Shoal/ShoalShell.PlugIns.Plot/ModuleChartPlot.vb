Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Terminal.Utility

<[PackageNamespace]("Plot.Device.Chart", Url:="http://SourceForge.net/projects/shoal")>
Module ModuleChartPlot

    <ExportAPI("plot.serials")>
    Public Function PlotChart(data As DocumentStream.File) As Image
        'Using device As FormChartPlotDevice = New FormChartPlotDevice
        '    Call device.Draw(DocumentStream.DataFrame.CreateObject(data))
        '    Call device.ShowDialog()
        '    Return device.CopyChartImage
        'End Using
    End Function

    <ExportAPI("plot.pi_chart")>
    Public Function PlotPiChart(data As DocumentStream.File) As Image
        Throw New NotImplementedException
    End Function
End Module

''' <summary>
''' Data source operation ShoalShell API for the csv document IO. 
''' </summary>
''' <remarks></remarks>
<[PackageNamespace]("IO_Device.Csv", Description:="Data source operation ShoalShell API for the csv document IO.", Publisher:="xie.guigang@live.com",
                    Url:="http://SourceForge.net/projects/shoal")>
Public Module DataSource

    <InputDeviceHandle("excel")>
    <ExportAPI("Read.Excel")>
    Public Function LoadExcel(path As String) As Dictionary(Of String, DocumentStream.DataFrame)
        Dim Excel = New ExcelReader(path, True, True)
        Dim Tables = Excel.GetWorksheetList
        Dim LQuery = (From Table As String In Tables Select Table, DF = Excel.GetWorksheet(Table).CreateDataReader.DataFrame).ToDictionary(Function(item) item.Table, Function(item) item.DF)
        Return LQuery
    End Function

    <ExportAPI("get.table")>
    Public Function GetTable(Excel As Dictionary(Of String, DocumentStream.DataFrame), Tab As String) As DocumentStream.DataFrame
        Return Excel(Tab)
    End Function

    <ExportAPI("csv.trim")>
    Public Function TrimData(path As String, Optional replaceAs As String = "") As DocumentStream.File
        Return DocumentStream.File.Normalization(path, replaceAs)
    End Function

    <InputDeviceHandle("dataframe")>
    <ExportAPI("read.csv.data.frame")>
    Public Function LoadDataFrameFromCsv(path As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame
        Return Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.Load(path, System.Text.Encoding.Default)
    End Function

    <InputDeviceHandle("csv")>
    <ExportAPI("read.csv")>
    Public Function ReadCsv(path As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Return Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)
    End Function

    <InputDeviceHandle("imports.csv")>
    <ExportAPI("imports.csv")>
    Public Function ReadCSV_FASTLOAD(path As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Using pdt As New CBusyIndicator(_start:=True)
            Call Console.WriteLine("[DEBUG] Start loading data from " & path.ToFileURL)
            Dim MAT = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.FastLoad(path)
            Return MAT
        End Using
    End Function

    <ExportAPI("imports.csv")>
    Public Function ImportsCsv(path As String, Optional delimiter As String = ",") As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        If String.Equals(delimiter, "VB.Tab", StringComparison.OrdinalIgnoreCase) Then
            delimiter = vbTab
        End If
        If String.Equals(delimiter, "VB.CrLF", StringComparison.OrdinalIgnoreCase) Then
            delimiter = vbCrLf
        End If
        If String.Equals(delimiter, "VB.LF", StringComparison.OrdinalIgnoreCase) Then
            delimiter = vbLf
        End If

        Return Microsoft.VisualBasic.DocumentFormat.Csv.DataImports.Imports(path, delimiter)
    End Function

    <IO_DeviceHandle(GetType(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File))>
    <ExportAPI("write.csv")>
    Public Function WriteCsv(data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, path As String) As Boolean
        Return data.Save(path, False)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of Object)))>
    <ExportAPI("write.csv")>
    Public Function WriteCsv(data As Generic.IEnumerable(Of Object), path As String) As Boolean
        Return data.SaveTo(path)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.ComponentModels.DynamicObjectLoader)))>
    <ExportAPI("write.csv")>
    Public Function WriteCsv(data As Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.ComponentModels.DynamicObjectLoader), path As String) As Boolean
        Return data.SaveTo(path)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject)))>
    Public Function WriteCsv(data As Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject), path As String) As Boolean
        Return CType(data, Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File).Save(path, False)
    End Function

    <OutputDeviceHandle(GetType(Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject)))>
    Private Function ShowCSVData(csv As Generic.IEnumerable(Of Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject)) As Boolean
        Dim TempPath As String = My.Computer.FileSystem.SpecialDirectories.Temp & "/csv_output___temp.csv"
        Call CType(csv, Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File).Save(TempPath, False)
        Call Process.Start(TempPath).WaitForExit()
        Return True
    End Function

    <ExportAPI("Create.Csv")>
    Public Function CreateDocument(data As Generic.IEnumerable(Of Object)) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Return Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Save(data, False)
    End Function

    <IO_DeviceHandle(GetType(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame))>
    <ExportAPI("Write.Csv")>
    Public Function WriteCsv(data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame, saveto As String) As Boolean
        Return data.Save(saveto, False)
    End Function
End Module