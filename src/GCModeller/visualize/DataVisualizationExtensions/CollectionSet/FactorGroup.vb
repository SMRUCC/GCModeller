Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class FactorGroup

        Public Property factor As String
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

        Public Function GetUniqueId(name As String) As String()
            Dim target As NamedCollection(Of String) = data.Where(Function(t) t.name = name).FirstOrDefault

            If target.IsEmpty Then
                Return {}
            End If

            Dim others = data.Where(Function(t) t.name <> name).Select(Function(t) t.value).IteratesALL.Distinct.Indexing
            Dim unique = target.Where(Function(id) Not id Like others).ToArray

            Return unique
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

                Dim countN As Integer = Aggregate i As NamedValue(Of Index(Of String))
                                        In allIndex
                                        Where id Like i.Value
                                        Into Count

                If countN = collections.Length Then
                    Yield id
                End If
            Next
        End Function

    End Class
End Namespace