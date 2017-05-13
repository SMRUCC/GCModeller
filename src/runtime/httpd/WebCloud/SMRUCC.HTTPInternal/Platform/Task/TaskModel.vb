Namespace Platform

    Public MustInherit Class TaskModel

        Protected current As Integer

        Protected MustOverride Function contents() As String()

        ''' <summary>
        ''' Public interface for invoke this task
        ''' </summary>
        Public MustOverride Sub RunTask()

        Public Function GetProgress() As TaskProgress
            Dim o As New TaskProgress With {
                .current = current,
                .progress = contents()
            }
            Return o
        End Function

        Public Overrides Function ToString() As String
            Return $" -> [{contents(current)}]"
        End Function
    End Class
End Namespace