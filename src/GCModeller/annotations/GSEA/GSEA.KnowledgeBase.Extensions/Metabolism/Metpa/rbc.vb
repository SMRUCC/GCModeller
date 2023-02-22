Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic

Namespace Metabolism.Metpa

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()

        Public Function write() As String
            Dim rbc As String = base.c(data:=data)

            If rbc = "NULL" Then
                Return rbc
            Else
                names(rbc) = kegg_id
            End If

            Return rbc
        End Function
    End Class

    Public Class rbcList

        Public Property list As Dictionary(Of String, rbc)

        Public Function write() As String
            Dim list As New var(base.list)

            For Each map In Me.list
                list(map.Name) = map.Value.write
            Next

            Return list
        End Function
    End Class
End Namespace