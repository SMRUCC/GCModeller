Imports LANS.SystemsBiology.AnalysisTools.CellularNetwork.PFSNet.DataStructure
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

Namespace VirtualCell

    <PackageNamespace("GCModeller.VirtualCell.Model.IO")>
    Module Model

        ''' <summary>
        ''' 生成用于启动pfsnet批量分析所使用的批处理脚本
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="saveCsv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <IO_DeviceHandle(GetType(PFSNetResultOut))>
        <ExportAPI("Write.PFSNet")>
        Public Function SavePfsNET(data As RTools.PfsNET.PFSNetResultOut, saveCsv As String) As Boolean
            Call data.GetXml.SaveTo(saveCsv)
            Call System.IO.File.WriteAllLines(saveCsv & ".txt", data.STD_OUTPUT)

            Return True
        End Function

        <IO_DeviceHandle(GetType(GCMarkupLanguage.BacterialModel))>
        Public Function SaveGCML(model As GCMarkupLanguage.BacterialModel, path As String) As Boolean
            Return model.Save(path)
        End Function

        <IO_DeviceHandle(GetType(FileStream.IO.XmlresxLoader))>
        <ExportAPI("Write.CellSystem_Loader")>
        Public Function WriteModel(XmlLoader As FileStream.IO.XmlresxLoader, Optional path As String = "") As Boolean
            Return XmlLoader.SaveTo(path)
        End Function

        <Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(FileStream.XmlFormat.CellSystemXmlModel))>
        <ExportAPI("Write.CellSystem_Model")>
        Public Function SaveModel(Model As FileStream.XmlFormat.CellSystemXmlModel, filepath As String) As Boolean
            Return Model.Save(filepath)
        End Function
    End Module
End Namespace