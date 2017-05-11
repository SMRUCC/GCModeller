Namespace Platform

    Public MustInherit Class TaskModel

        Protected current As Integer

        Protected MustOverride Function contents() As String()

        ''' <summary>
        ''' Public interface for invoke this task
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetTask() As Action

        Public Function GetProgress() As TaskProgress
            Return New TaskProgress With {
                .current = current,
                .progress = contents()
            }
        End Function

        Public Overrides Function ToString() As String
            Return $" --> [{contents(current)}]"
        End Function
    End Class
End Namespace