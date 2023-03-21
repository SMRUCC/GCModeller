Namespace Turtle

    Public Class Triple

        Public Property subject As String
        Public Property relations As Relation()

        Public Overrides Function ToString() As String
            Return $"<{subject}> {relations.JoinBy(" ; ")}."
        End Function

    End Class

    Public Class Relation

        Public Property predicate As String
        Public Property objs As String()

        Public Overrides Function ToString() As String
            Return $"<{predicate}> <{objs.JoinBy(" , ")}>"
        End Function
    End Class
End Namespace