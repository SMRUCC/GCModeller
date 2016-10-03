#Region "Microsoft.VisualBasic::1d30bbf0d446bd3a051bbe05701219d4, ..\GCModeller\shoalAPI\SBML.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

<[Namespace]("Doc.SBML")>
Module SBML

    <Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.CARMEN.Reaction)))>
    <ExportAPI("Write.Csv.CarmenImports")>
    Public Function WriteCARMEN(data As Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.CARMEN.Reaction), path As String) As Boolean
        Return data.SaveTo(path, False)
    End Function

    <ExportAPI("Read.Xml.SBML2")>
    Public Function LoadSBML2(path As String) As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile
        Return LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile.Load(path)
    End Function
End Module
