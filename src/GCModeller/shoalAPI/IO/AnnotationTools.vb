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
