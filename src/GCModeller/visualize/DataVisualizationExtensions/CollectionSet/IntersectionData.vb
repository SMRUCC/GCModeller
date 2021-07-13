Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class IntersectionData

        Public Property groups As FactorGroup()

        ''' <summary>
        ''' get the labels of all collection set like ``a vs b``, etc
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAllCollectionTags() As String()
            Return groups _
                .Select(Function(d) d.data.Select(Function(t) t.name)) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

    End Class

    Public Class FactorGroup

        Public Property factors As String
        Public Property data As NamedCollection(Of String)()
        Public Property color As Color

        Public Iterator Function GetAllUniques() As IEnumerable(Of NamedCollection(Of String))
            Dim allIndex As NamedValue(Of Index(Of String))() = data _
                .Select(Function(t)
                            Return New NamedValue(Of Index(Of String)) With {
                                .Name = t.name,
                                .Value = t.value.Indexing
                            }
                        End Function) _
                .ToArray

            For Each [set] As NamedCollection(Of String) In data
                Dim others = allIndex.Where(Function(i) i.Name <> [set].name).ToArray
                Dim excepts As String() = [set] _
                    .Where(Function(id)
                               Return others.All(Function(i) Not id Like i.Value)
                           End Function) _
                    .ToArray

                Yield New NamedCollection(Of String) With {
                    .name = [set].name,
                    .value = excepts,
                    .description = $"{excepts.Length} unique id between all collection set"
                }
            Next
        End Function

        Public Iterator Function GetIntersection(collections As String()) As IEnumerable(Of String)
            Dim allIndex As NamedValue(Of Index(Of String))() = data _
                .Where(Function(t)
                           Return collections.IndexOf(t.name) > -1
                       End Function) _
                .Select(Function(t)
                            Return New NamedValue(Of Index(Of String)) With {
                                .Name = t.name,
                                .Value = t.value.Indexing
                            }
                        End Function) _
                .ToArray

            For Each id As String In allIndex _
                .Select(Function(t) t.Value.Objects) _
                .IteratesALL _
                .Distinct

                Dim count As Integer = allIndex.Where(Function(i) id Like i.Value).Count

                If count > 1 Then
                    Yield id
                End If
            Next
        End Function

    End Class
End Namespace