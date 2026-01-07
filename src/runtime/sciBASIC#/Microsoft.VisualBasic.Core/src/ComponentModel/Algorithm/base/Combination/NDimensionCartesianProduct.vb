Namespace ComponentModel.Algorithm.base

    Public Class NDimensionCartesianProduct

        Private Class InternalLambda(Of T)

            Public sequences As IEnumerable(Of T)()

            ' 递归辅助函数
            Public Function helper(depth As Integer, currentCombination As List(Of T)) As IEnumerable(Of T())
                If depth = sequences.Length Then
                    Return {currentCombination.ToArray()}
                Else
                    Dim result As New List(Of T())()
                    For Each item In sequences(depth)
                        Dim newCombination As New List(Of T)(currentCombination)

                        Call newCombination.Add(item)
                        Call result.AddRange(helper(depth + 1, newCombination))
                    Next

                    Return result
                End If
            End Function
        End Class

        ''' <summary>
        ''' 生成任意数量集合的笛卡尔积（递归实现）
        ''' </summary>
        Public Shared Iterator Function CreateMultiCartesianProduct(Of T)(ParamArray sequences As IEnumerable(Of T)()) As IEnumerable(Of T())
            If sequences Is Nothing OrElse sequences.Length = 0 Then
                Return
            End If

            Dim recursive As New InternalLambda(Of T) With {.sequences = sequences}

            For Each combo As T() In recursive.helper(0, New List(Of T)())
                Yield combo
            Next
        End Function
    End Class
End Namespace