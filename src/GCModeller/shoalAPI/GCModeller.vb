#Region "Microsoft.VisualBasic::f9290afbc1324e79d88623ef8edc8c94, ..\GCModeller\shoalAPI\GCModeller.vb"

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

Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

<[PackageNamespace]("GCModeller.Assembly.File.IO",
                    Publisher:="xie.guigang@gmail.com")>
Module GCModellerTools

    <InputDeviceHandle("GCML.Csvx")>
    Public Function LoadGCMLCsvx(Path As String) As XmlresxLoader
        Return New XmlresxLoader(CellSystemPath:=Path)
    End Function
End Module
