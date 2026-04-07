#Region "Microsoft.VisualBasic::a4a8ed7c4b848d5c81237f1c0f4eb48d, analysis\Metagenome\Metagenome\Kmers\KmerMemory.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 84
    '    Code Lines: 67 (79.76%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 17 (20.24%)
    '     File Size: 3.04 KB


    '     Class KmerMemory
    ' 
    '         Properties: Count
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetEnumerator, HashKmer, IEnumerable_GetEnumerator, ToString
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
