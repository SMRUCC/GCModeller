Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel

    Public Class HashList(Of T As IAddressOf) : Implements Enumeration(Of T)

        Dim list As New Dictionary(Of Integer, T)

        Default Public Property Item(i As Integer) As T
            Get
                If list.ContainsKey(i) Then
                    Return list(key:=i)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As T)
                value.Assign(i)
                list(key:=i) = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub Clear()
            Call list.Clear()
        End Sub

        Public Sub Replace(item As T)
            list(key:=item.Address) = item
        End Sub

        Public Sub ReplaceAt(i As Integer, item As T)
            item.Assign(i)
            list(key:=i) = item
        End Sub

        Public Sub AddRange(items As IEnumerable(Of T))
            If Not items Is Nothing Then
                For Each item As T In items
                    Call Replace(item)
                Next
            End If
        End Sub

        Public Function IndexOf(item As T) As Integer
            Return item.Address
        End Function

        Public Sub Remove(item As T)
            Call list.Remove(item.Address)
        End Sub

        Public Sub RemoveAt(i As Integer)
            Call list.Remove(i)
        End Sub

        Public Sub Sort()
            Dim sorted = list.Values.OrderBy(Function(a) a).ToArray

            Call Clear()

            For i As Integer = 0 To sorted.Length - 1
                Dim item As T = sorted(i)

                Call item.Assign(i)
                Call list.Add(i, item)
            Next
        End Sub

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            For Each item As T In list.Values
                Yield item
            Next
        End Function
    End Class
End Namespace