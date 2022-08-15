
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<Package("OBO")>
Module OBO_DAG

    <ExportAPI("read.obo")>
    Public Function readOboDAG(path As String) As GO_OBO
        Return GO_OBO.LoadDocument(path)
    End Function
End Module
