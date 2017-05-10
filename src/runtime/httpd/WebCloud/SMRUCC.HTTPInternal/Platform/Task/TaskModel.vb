Namespace Platform

    Public MustInherit Class TaskModel

        Protected current As Integer

        Protected MustOverride Function contents() As String()

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