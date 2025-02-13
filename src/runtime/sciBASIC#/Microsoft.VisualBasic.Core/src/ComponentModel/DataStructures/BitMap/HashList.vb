Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel

    Public Class HashList(Of T) : Implements Enumeration(Of T)

        ' 可能会出现的问题：
        ' 默认情况下，.NET中的Object.GetHashCode方法返回的是一个基于对象内存地址的确定的哈希值，并且每次调用都不会重新进行复杂的哈希计算。
        ' 在多线程环境中，如果对象的内存地址在垃圾回收过程中发生了变化（例如，发生了内存压缩），那么默认的GetHashCode方法可能会返回不同的值。但这是一种特殊情况，并不常见。

        Dim keys As New List(Of Integer)
        Dim offset As New Dictionary(Of Integer, Integer)
        Dim list As New Dictionary(Of Integer, T)

        Public Sub New()
        End Sub

        Public Sub Clear()
            Call keys.Clear()
            Call offset.Clear()
            Call list.Clear()
        End Sub

        Public Sub Add(item As T)
            Dim hashkey As Integer = item.GetHashCode

            list(key:=hashkey) = item
            keys.Add(hashkey)
            offset.Add(hashkey, keys.Count - 1)
        End Sub

        Public Sub AddRange(items As IEnumerable(Of T))
            If Not items Is Nothing Then
                For Each item As T In items
                    Call Add(item)
                Next
            End If
        End Sub

        Public Function IndexOf(item As T) As Integer
            Dim hashkey As Integer = item.GetHashCode

            If offset.ContainsKey(hashkey) Then
                Return offset(hashkey)
            Else
                Return -1
            End If
        End Function

        Public Sub Remove(item As T)
            Dim i As Integer = IndexOf(item)

            If i > -1 Then
                Call RemoveAt(i)
            End If
        End Sub

        Public Sub RemoveAt(i As Integer)
            Dim hashkey As Integer = keys(i)

            keys.RemoveAt(i)
            list.Remove(hashkey)
            offset.Remove(hashkey)

            For idx As Integer = i To keys.Count - 1
                offset(key:=idx) = offset(key:=idx) - 1
            Next
        End Sub

        Public Sub Sort()
            Dim sorted = list.Values.OrderBy(Function(a) a).ToArray

            Call Clear()

            For i As Integer = 0 To sorted.Length - 1
                Dim hashkey As Integer = sorted(i).GetHashCode

                Call keys.Add(hashkey)
                Call offset.Add(hashkey, i)
                Call list.Add(hashkey, sorted(i))
            Next
        End Sub

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            For Each key As Integer In keys
                Yield list(key:=key)
            Next
        End Function
    End Class
End Namespace