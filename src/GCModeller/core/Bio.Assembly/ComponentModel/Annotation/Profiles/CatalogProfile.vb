Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfile : Implements Enumeration(Of NamedValue(Of Double))

        Public Property profile As New Dictionary(Of String, Double)

        Sub New()
        End Sub

        Sub New(data As IEnumerable(Of NamedValue(Of Double)))
            For Each item In data
                profile(item.Name) = item.Value
            Next
        End Sub

        Sub New(copy As CatalogProfile)
            profile = New Dictionary(Of String, Double)(copy.profile)
        End Sub

        Sub New(data As IEnumerable(Of NamedValue(Of Integer)))
            For Each item In data
                profile(item.Name) = item.Value
            Next
        End Sub

        Public Function Add(value As NamedValue(Of Double)) As CatalogProfile
            Call profile.Add(value.Name, value.Value)
            Return Me
        End Function

        Public Function Add(name As String, value As Double)
            Call profile.Add(name, value)
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return profile.Keys.GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements Enumeration(Of NamedValue(Of Double)).GenericEnumerator
            For Each item In profile
                Yield New NamedValue(Of Double) With {
                    .Name = item.Key,
                    .Value = item.Value
                }
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of NamedValue(Of Double)).GetEnumerator
            Yield GenericEnumerator()
        End Function

        Public Overloads Shared Widening Operator CType(profile As NamedValue(Of Double)()) As CatalogProfile
            Return New CatalogProfile With {
                .profile = profile.ToDictionary.FlatTable
            }
        End Operator

        Public Shared Narrowing Operator CType(profile As CatalogProfile) As NamedValue(Of Double)()
            If profile Is Nothing Then
                Return {}
            Else
                Return profile.AsEnumerable.ToArray
            End If
        End Operator
    End Class

End Namespace