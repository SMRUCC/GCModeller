Namespace Kmers

    Public Class KmerMemory(Of T) : Implements IEnumerable(Of KeyValuePair(Of String, T))

        ReadOnly buckets As New Dictionary(Of String, Dictionary(Of String, T))
        ReadOnly prefixLen As Integer

        Default Public ReadOnly Property [Get](kmer As String) As T
            Get
                Dim key As String = kmer.Substring(0, prefixLen)

                If buckets.ContainsKey(key) Then
                    If buckets(key).ContainsKey(kmer) Then
                        Return buckets(key)(kmer)
                    Else
                        Return Nothing
                    End If
                End If

                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return Aggregate bucket In buckets.Values Into Sum(bucket.Count)
            End Get
        End Property

        Sub New(Optional prefixLen As Integer = 3)
            Me.prefixLen = prefixLen
        End Sub

        Sub New(cache As Dictionary(Of String, T), Optional prefixLen As Integer = 3)
            For Each item As KeyValuePair(Of String, T) In cache
                Dim kmer As String = item.Key
                Dim data As T = item.Value
                Dim key As String = kmer.Substring(0, prefixLen)

                If Not buckets.ContainsKey(key) Then
                    Call buckets.Add(key, New Dictionary(Of String, T) From {{kmer, data}})
                Else
                    Call buckets(key).Add(kmer, data)
                End If
            Next
        End Sub

        Public Function HashKmer(kmer As String) As Boolean
            Dim key As String = kmer.Substring(0, prefixLen)

            If buckets.ContainsKey(key) Then
                Return buckets(key).ContainsKey(kmer)
            End If

            Return False
        End Function

        Public Sub Add(kmer As String, data As T)
            Dim key As String = kmer.Substring(0, prefixLen)

            If Not buckets.ContainsKey(key) Then
                Call buckets.Add(key, New Dictionary(Of String, T) From {{kmer, data}})
            Else
                Call buckets(key).Add(kmer, data)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{buckets.Count} k-mer prefix buckets of total {Count} elements"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, T)) Implements IEnumerable(Of KeyValuePair(Of String, T)).GetEnumerator
            For Each bucket As Dictionary(Of String, T) In buckets.Values
                For Each item In bucket
                    Yield item
                Next
            Next
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace