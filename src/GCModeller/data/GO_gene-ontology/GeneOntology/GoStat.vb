Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' Statics of the GO function catalog
''' </summary>
Public Module GoStat

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, String), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Return genes.CountStat(Function(g) (getGO(g), 1), GO_terms)
    End Function

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, (goID$, number%)), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Dim out As Dictionary(Of String, Dictionary(Of NamedValue(Of int))) =
            Enums(Of Ontologies) _
            .ToDictionary(Function(o) o.Description,
                          Function(null) New Dictionary(Of NamedValue(Of int)))

        For Each g As gene In genes
            Dim value As (goID$, Number As Integer) = getGO(g)
            Dim goID As String = value.goID
            Dim term As Term = GO_terms(goID)
            Dim count = out(term.namespace)

            If Not count.ContainsKey(goID) Then
                Call count.Add(
                    New NamedValue(Of int) With {
                        .Name = goID,
                        .Description = term.name,
                        .Value = 0
                    })
            End If

            count(goID).Value.value += value.Number
        Next

        Return out.ToDictionary(
            Function(x) x.Key,
            Function(value) As NamedValue(Of Integer)()
                Dim array As NamedValue(Of Integer)() =
                    New NamedValue(Of Integer)(value.Value.Count) {}

                For Each i As SeqValue(Of NamedValue(Of int)) In value.Value.Values.SeqIterator
                    Dim x As NamedValue(Of int) = +i

                    array(i.i) = New NamedValue(Of Integer) With {
                        .Name = x.Name,
                        .Value = x.Value,
                        .Description = x.Description
                    }
                Next

                Return array
            End Function)
    End Function
End Module
