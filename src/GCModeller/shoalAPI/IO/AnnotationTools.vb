#Region "Microsoft.VisualBasic::cfb8e2af31f6dfaadd82a511c07d5126, ..\GCModeller\shoalAPI\IO\AnnotationTools.vb"

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

Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.CEG.Tools
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.Reports
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

Module AnnotationToolsIOAPI

    <InputDeviceHandle("CEG")>
    <ExportAPI("Read.Xml.CEG")>
    Public Function LoadDb(Path As String) As AnnotationTools.CEG.CEGAssembly
        Return Path.LoadXml(Of AnnotationTools.CEG.CEGAssembly)()
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of EssentialGeneCluster)))>
    Public Function ClusterDataSaved(Data As Generic.IEnumerable(Of EssentialGeneCluster), SaveTo As String) As Boolean
        Dim LQuery = (From Cluster In Data.AsParallel Select New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {.SequenceData = Cluster.Nt, .Attributes = {Cluster.ClusterID}}).ToArray
        Call CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(SaveTo & ".fasta")
        Return Data.SaveTo(SaveTo, False)
    End Function

    <InputDeviceHandle("CEG.Annotation")>
    <ExportAPI("Read.Csv.CEG_Annotation")>
    Public Function LoadAnnotation(Path As String) As CEG.Annotation()
        Return LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.CEG.Tools.LoadAnnotation(Path)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.GCModeller.Workbench.DatabaseServices.Model_Repository.Regprecise)))>
    <ExportAPI("Write.Csv.RegPreciseTable")>
    Public Function WriteTable(data As Generic.IEnumerable(Of LANS.SystemsBiology.GCModeller.Workbench.DatabaseServices.Model_Repository.Regprecise),
                               <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    <InputDeviceHandle("bbh")>
    <ExportAPI("Read.Csv.bbh")>
    Public Function ReadBBH(path As String) As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit()
        Return path.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)(False).ToArray
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)))>
    <ExportAPI("Write.Csv.bbh")>
    Public Function WriteBBH(data As Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit),
                             <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    <IO_DeviceHandle(GetType(GenomeAnnotations))>
    <ExportAPI("Write.Xml.Anno_Reports")>
    Public Function SaveReportAsXml(rpt As GenomeAnnotations, saveto As String) As Boolean
        Return rpt.GetXml.SaveTo(saveto)
    End Function

    <InputDeviceHandle("Annotations")>
    <ExportAPI("Read.Xml.Anno_Reports")>
    Public Function LoadReportFromXml(path As String) As GenomeAnnotations
        Return path.LoadXml(Of GenomeAnnotations)()
    End Function
End Module

