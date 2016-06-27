Imports LANS.SystemsBiology.Assembly.SBML.Components

Namespace Specifics.MetaCyc

    Public MustInherit Class ReaderBase(Of TMap)
        Implements IEnumerable(Of [Property])

        Protected ReadOnly __source As Dictionary(Of String, [Property])
        Protected ReadOnly __mapsKey As IReadOnlyDictionary(Of TMap, String)

        Sub New(source As IEnumerable(Of [Property]), maps As IReadOnlyDictionary(Of TMap, String))
            __source = source.ToDictionary(Function(x) x.Name)
            __mapsKey = maps
        End Sub

        Protected Function __getValue(key As TMap) As String
            Dim name As String = __mapsKey(key)
            If __source.ContainsKey(name) Then
                Return __source(name).value
            Else
                Return ""
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of [Property]) Implements IEnumerable(Of [Property]).GetEnumerator
            For Each x In __source
                Yield x.Value
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace