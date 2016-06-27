Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    Public Class [Object]
        Public Property UniqueId As String
        Public Property Name As String
        Public Property ReactionEquation As ReactionEquation
        Public Property Pathways As String()
        Public Property Cofactors As String()
        Public Property Activators As String()
        Public Property Inhibitors As String()
        Public Property SubunitComposition As String

        Public Overrides Function ToString() As String
            Dim sbr As StringBuilder = New StringBuilder(512)

            sbr.Append(UniqueId & ", ")
            sbr.Append(Name & ", ")
            sbr.Append(ReactionEquation.ToString & ", ")
            For Each e As String In Pathways
                Append(e, sbr)
            Next
            For Each e As String In Cofactors
                Append(e, sbr)
            Next
            For Each e As String In Activators
                Append(e, sbr)
            Next
            For Each e As String In Inhibitors
                Append(e, sbr)
            Next

            sbr.Append(SubunitComposition)

            Return sbr.ToString
        End Function

        Friend Sub Append(e As String, ByRef sbr As StringBuilder)
            If String.IsNullOrEmpty(e) Then
                sbr.Append("NULL, ")
            Else
                sbr.AppendFormat("{0}, ", e)
            End If
        End Sub

        Public Shared Function GetData(e As RecordLine) As [Object]
            Dim NewObj As New [Object]

            With NewObj
                .UniqueId = e.Data(0)
                .Name = e.Data(1)
                .ReactionEquation = e.Data(ReactionEquation.INDEX)
                ReDim .Pathways(3)
                Call Array.ConstrainedCopy(e.Data, 3, .Pathways, 0, 4)
                ReDim .Cofactors(3)
                Call Array.ConstrainedCopy(e.Data, 7, .Cofactors, 0, 4)
                ReDim .Activators(3)
                Call Array.ConstrainedCopy(e.Data, 11, .Activators, 0, 4)
                ReDim .Inhibitors(3)
                Call Array.ConstrainedCopy(e.Data, 15, .Inhibitors, 0, 4)
                .SubunitComposition = e.Data(19)
            End With

            Return NewObj
        End Function
    End Class
End Namespace