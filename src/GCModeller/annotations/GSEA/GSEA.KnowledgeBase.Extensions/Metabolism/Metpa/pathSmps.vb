Imports RDotNet.Extensions.VisualBasic.API

Public Class pathSmps

    Public Smps As String()()
    Public ids As String()

    Public Function write() As String
        Dim vec As String = base.c(ids, stringVector:=True)

        If vec = "NULL" Then
            Return vec
        Else
            names(vec) = Smps _
                .Select(Function(a) a.JoinBy("; ")) _
                .ToArray
        End If

        Return vec
    End Function
End Class
