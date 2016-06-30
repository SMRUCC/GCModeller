Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.Reports
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Annotation.Reports", Publisher:="xie.guigang@gmail.com")>
Public Module ShellScriptAPI

    <ExportAPI("Write.Rtf.Anno_Reports")>
    Public Function SaveReportAsRtf(rpt As GenomeAnnotations, saveRTF As String) As Boolean
        Return rpt.SaveRTF(saveRTF)
    End Function
End Module
