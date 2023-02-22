Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API

Namespace Metabolism.Metpa

    Public Class dgr

        Public kegg_id As String()
        Public dgr As Double()

        Public Function write() As String
            Dim vec As String = base.c(data:=dgr)

            If vec = "NULL" Then
                Return vec
            Else
                names(vec) = kegg_id
            End If

            Return vec
        End Function

    End Class

    Public Class dgrList

        Public pathways As NamedValue(Of dgr)()

        Public Function write() As String
            Dim list As New var(base.list)

            For Each map In pathways
                list(map.Name) = map.Value.write
            Next

            Return list
        End Function

    End Class
End Namespace