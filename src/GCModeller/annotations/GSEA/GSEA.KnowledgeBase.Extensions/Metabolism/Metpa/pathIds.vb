Imports RDotNET.Extensions.VisualBasic.API

Namespace Metabolism.Metpa

    Public Class pathIds

        Public pathwayNames As String()
        Public ids As String()

        Public Function write() As String
            Dim vec As String = base.c(ids, stringVector:=True)

            If vec = "NULL" Then
                Return vec
            Else
                names(vec) = pathwayNames
            End If

            Return vec
        End Function

    End Class
End Namespace