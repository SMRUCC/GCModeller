Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' Statics of the GO function catalog
''' </summary>
Public Module GoStat

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, String), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Dim out As Dictionary(Of String, List(Of NamedValue(Of Integer))) =
            Enums(Of Ontologies) _
            .ToDictionary(Function(o) o.Description,
                          Function(null) New List(Of NamedValue(Of Integer)))

        For Each g As gene In genes
            Dim goID As String = getGO(g)
            Dim term As Term = GO_terms(goID)
        Next

        Return out.ToDictionary(
            Function(x) x.Key,
            Function(value) value.Value.ToArray)
    End Function
End Module
