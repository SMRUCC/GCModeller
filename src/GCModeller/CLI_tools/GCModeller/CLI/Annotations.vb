Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("--Interpro.Build", Usage:="--Interpro.Build /xml <interpro.xml>")>
    Public Function BuildFamilies(args As CommandLine.CommandLine) As Integer
        Dim DbPath As String = args("/xml")
        Dim DbXml = SMRUCC.genomics.DatabaseServices.ComparativeGenomics.AnnotationTools.Interpro.Xml.LoadDb(DbPath)
        Dim Families = SMRUCC.genomics.DatabaseServices.ComparativeGenomics.AnnotationTools.Interpro.Xml.BuildFamilies(DbXml)
        Call Families.SaveTo(DbXml.FilePath.TrimFileExt & ".Families.csv")

        Return 0
    End Function
End Module