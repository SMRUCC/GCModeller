Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API

Namespace Metabolism.Metpa

    Public Class mset

        Public Property metaboliteNames As String()
        Public Property kegg_id As String()

        Public Function write() As String
            Dim mset As String = base.c(kegg_id, stringVector:=True)

            If mset = "NULL" Then
                Return mset
            Else
                names(mset) = metaboliteNames
            End If

            Return mset
        End Function
    End Class

    Public Class msetList

        Public list As NamedValue(Of mset)()

        Public Function write() As String
            Dim list As New var(base.list)

            For Each map In Me.list
                list(map.Name) = map.Value.write
            Next

            Return list
        End Function

    End Class
End Namespace