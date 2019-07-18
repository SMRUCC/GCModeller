Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    Public Class EnzymeWebQuery : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False
                   )

            Call MyBase.New(url:=AddressOf GetECNumber,
                            contextGuid:=Function(id) id,
                            parser:=AddressOf ParsePage,
                            prefix:=Nothing,
                            cache:=cache,
                            interval:=interval,
                            offline:=offline
                   )
        End Sub

        Public Shared Function GetECNumber(ec As String) As String
            Return $"https://www.genome.jp/dbget-bin/www_bget?ec:{ec}"
        End Function

        Public Shared Function ParsePage(html$, schema As Type) As Object

        End Function
    End Class
End Namespace