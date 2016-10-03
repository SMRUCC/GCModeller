#Region "Microsoft.VisualBasic::43529873e3573cfac322dfbed8b3d5ee, ..\GCModeller\shoalAPI\IO\Genbank.vb"

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
'Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Extensions

Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions

Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports Microsoft.VisualBasic.Scripting.MetaData

Partial Module IO

    <InputDeviceHandle("Gff")>
    <ExportAPI("Read.Gff")>
    Public Function ReadGff(Path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF.LoadDocument(Path)
    End Function

    <IO_DeviceHandle(GetType(LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF))>
    <ExportAPI("Write.Gff")>
    Public Function WriteGff(data As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF, SaveTo As String) As Boolean
        Return data.Save(SaveTo, System.Text.Encoding.ASCII)
    End Function

End Module
