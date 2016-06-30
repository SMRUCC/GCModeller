Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports LANS.SystemsBiology.InteractionModel.DataServicesExtension

<PackageNamespace("GCModeller.DataServices")>
Module DataServices

    <InputDeviceHandle("serials.data")>
    <ExportAPI("read.csv.serials")>
    Public Function LoadData(path As String) As SerialsData()
        Dim Csv = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.FastLoad(path)
        Return LoadCsv(Csv)
    End Function

    <InputDeviceHandle("serials.data")>
    <ExportAPI("load.csv.serials")>
    Public Function LoadCsv(Csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As SerialsData()
        Return LANS.SystemsBiology.InteractionModel.DataServicesExtension.LoadCsv(CsvDatas:=Csv.ToArray)
    End Function

    <ExportAPI("Write.Csv")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of SerialsData)))>
    Public Function SaveCsv(data As Generic.IEnumerable(Of SerialsData), saveto As String) As Boolean
        Return LANS.SystemsBiology.InteractionModel.DataServicesExtension.SaveCsv(data).Save(saveto, False)
    End Function
End Module
