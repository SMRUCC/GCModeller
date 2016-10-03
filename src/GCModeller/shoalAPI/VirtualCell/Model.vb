#Region "Microsoft.VisualBasic::ca395144a2021a4599e3667a819846a6, ..\GCModeller\shoalAPI\VirtualCell\Model.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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
