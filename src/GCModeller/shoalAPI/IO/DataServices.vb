#Region "Microsoft.VisualBasic::5a6764bcd966eee1cb465566eee5cb02, ..\GCModeller\shoalAPI\IO\DataServices.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

