Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace Builder

    Public Class UniqueIdTrimer : Inherits IBuilder

        Sub New(MetaCyc As DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim MetaCycRxns = MetaCyc.GetReactions
            For i As Integer = 0 To MetaCycRxns.NumOfTokens - 1
                Dim Rxn = MetaCycRxns(i)

                Call Remove(Rxn.Left)
                Call Remove(Rxn.Right)
            Next

            Return MyBase.Model
        End Function

        Private Shared Sub Remove(TargetList As List(Of String))
            Dim Collection As String() = TargetList.ToArray
            For Each Str As String In Collection
                If Str.First() = "|"c AndAlso Str.Last() = "|"c Then
                    Call TargetList.Add(Mid(Str, 2, Len(Str) - 2))
                    Call TargetList.Remove(Str)
                End If
            Next
        End Sub
    End Class
End Namespace