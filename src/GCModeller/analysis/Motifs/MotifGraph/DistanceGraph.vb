Public Module DistanceGraph

    ''' <summary>
    ''' 计算出出现在f1后面的f2的距离平均值
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 具有先后顺序关系的片段距离
    ''' </remarks>
    Public Function MeasureAverageDistance(seq As String, f1 As String, f2 As String) As Double
        If seq.IndexOf(f1) = -1 OrElse seq.IndexOf(f2) = -1 Then
            ' no such relationship
            Return 0
        End If

        Dim pos1 As Integer = 0
        Dim pos2 As Integer = 0
        Dim pos As New List(Of Integer)

        Do While True
            pos1 = InStr(pos1 + 1, seq, f1)
            pos2 = InStr(pos2 + 1, seq, f2)

            If pos1 > 0 AndAlso pos2 > 0 Then
                If pos1 < pos2 Then
                    pos.Add(pos2 - pos1)
                Else
                    Exit Do
                End If
            Else
                Exit Do
            End If
        Loop

        If pos.Count = 0 Then
            Return 0
        Else
            Return pos.Average
        End If
    End Function

    Public Function TupleDistanceGraph(seq As String, components As IReadOnlyCollection(Of Char)) As Dictionary(Of String, Double)
        Dim graph As New Dictionary(Of String, Double)
        Dim tuples As String() = GetTuples(components).ToArray

        For Each t1 As String In tuples
            For Each t2 As String In tuples
                If t1 <> t2 Then
                    Call graph.Add($"{t1}|{t2}", MeasureAverageDistance(seq, t1, t2))
                End If
            Next
        Next

        Return graph
    End Function

    Friend Iterator Function GetTuples(components As IReadOnlyCollection(Of Char)) As IEnumerable(Of String)
        For Each i As Char In components
            For Each j As Char In components
                Yield New String({i, j})
            Next
        Next
    End Function
End Module
