Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    Public Class ReactionEquation
        Public Property Substrates As String()
        Public Property Products As String()

        Public Overrides Function ToString() As String
            Dim sbr As StringBuilder = New StringBuilder(256)

            For Each e As String In Substrates
                sbr.Append("[" & e & "] + ")
            Next
            sbr.Remove(sbr.Length - 2, 2)
            sbr.Append(" <----> ")
            For Each e As String In Products
                sbr.Append("[" & e & "] + ")
            Next
            sbr.Remove(sbr.Length - 2, 2)

            Return sbr.ToString
        End Function

        Public Shared Widening Operator CType(e As String) As ReactionEquation
            Dim Eqstr As String() = Strings.Split(e, "  <-->  ")
            Dim newobj As New ReactionEquation

            With newobj
                .Substrates = Strings.Split(Eqstr.First, " + ")
                .Products = Strings.Split(Eqstr.Last, " + ")
            End With

            Return newobj
        End Operator

        Public Const INDEX As Integer = 2
    End Class
End Namespace