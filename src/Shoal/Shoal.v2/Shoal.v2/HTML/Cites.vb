Imports System.Text
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace HTML

    Public Module Cites

        Public Function GetCites(typeDef As Type) As String
            Dim cites As Cite() = Cite.GetCiteList(typeDef)
            If cites.IsNullOrEmpty Then
                Dim ns = GetEntry(typeDef)
                If ns Is Nothing Then
                    Return ""
                Else
                    Return ns.Cites
                End If
            Else
                Return cites.ToArray(Function(c) c.HTML(120)).JoinBy("<br /><br />")
            End If
        End Function
    End Module
End Namespace