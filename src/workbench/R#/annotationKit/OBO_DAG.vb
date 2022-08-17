
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' The Open Biological And Biomedical Ontology (OBO) Foundry
''' </summary>
<Package("OBO")>
Module OBO_DAG

    ''' <summary>
    ''' parse the obo file 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.obo")>
    Public Function readOboDAG(path As String) As GO_OBO
        Return GO_OBO.LoadDocument(path)
    End Function
End Module
