Imports System.Text

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Equation
        Public Class SpeciesReference
            Public Property AccessionId As String
            Public Property Stoichiometric As Integer

            Public Overrides Function ToString() As String
                Return String.Format("{0} {1}", Stoichiometric, AccessionId)
            End Function
        End Class

        Public Property Left As SpeciesReference()
        Public Property Right As SpeciesReference()
        Public Property Reversible As Boolean

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each Item In Left
                Call sBuilder.Append(Item.ToString & " + ")
            Next
            Call sBuilder.Remove(sBuilder.Length - 3, 3)
            If Reversible Then
                Call sBuilder.Append(" <=> ")
            Else
                Call sBuilder.Append(" ==> ")
            End If

            For Each Item In Right
                Call sBuilder.Append(Item.ToString, " + ")
            Next
            Call sBuilder.Remove(sBuilder.Length - 3, 3)

            Return sBuilder.ToString
        End Function
    End Class
End Namespace