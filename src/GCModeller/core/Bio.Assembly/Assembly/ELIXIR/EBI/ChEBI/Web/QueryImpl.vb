Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.ELIXIR.EBI.ChEBI.WebServices

    Public Class QueryImpl : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional sleep% = -1)
            MyBase.New(
                AddressOf WebServices.CreateRequest,
                AddressOf fileId,
                AddressOf parseREST,
                AddressOf prefix,
 _
                cache:=cache,
                interval:=sleep
            )
        End Sub

        Private Shared Function prefix(chebiId As String) As String
            Return Strings.Mid(chebiId, 1, 2)
        End Function

        Private Shared Function fileId(chebiId As String) As String
            Return Strings.Trim(chebiId).Split(":"c).Last
        End Function

        Private Shared Function parseREST(response$, schema As Type) As Object
            Try
                Return REST.ParsingRESTData(response)
            Catch ex As Exception
                Call App.LogException(ex)

                Return Nothing
            End Try
        End Function
    End Class
End Namespace