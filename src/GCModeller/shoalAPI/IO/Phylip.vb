#Region "Microsoft.VisualBasic::470dbbd9ae5ac20538ca775b99ed30d7, ..\GCModeller\shoalAPI\IO\Phylip.vb"

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

