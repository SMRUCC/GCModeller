Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Expasy", Category:=APICategories.ResearchTools)>
Public Module Expasy

    <ExportAPI("Read.Csv.EnzymeClass")>
    Public Function ReadEnzymeClass(path As String) As LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)(False).ToArray
    End Function

    <ExportAPI("EC.Classify.Process")>
    Public Function EnzymeClassification(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)) _
        As LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()
        Return LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClassification(data)
    End Function

    <ExportAPI("Write.Csv.EnzymeClass")>
    Public Function SaveResult(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT),
                               saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function
End Module
