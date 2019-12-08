
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<Package("gokit.file")>
Public Module file

    <ExportAPI("read.go_obo")>
    Public Function ReadGoObo(goDb As String) As GO_OBO
        Return GO_OBO.LoadDocument(path:=goDb)
    End Function
End Module
