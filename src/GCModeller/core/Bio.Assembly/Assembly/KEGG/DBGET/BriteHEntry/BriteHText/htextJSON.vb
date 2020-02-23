
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class htextJSON

        Public Property name As String
        Public Property children As htextJSONnode()

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Shared Function parseJSON(json As String) As htextJSON
            Return json.SolveStream.LoadJSON(Of htextJSON)
        End Function

        Public Iterator Function DeflateTerms() As IEnumerable(Of BriteTerm)

        End Function

    End Class

    Public Class htextJSONnode

        Public Property name As String
        Public Property children As htextJSONnode()

        Public ReadOnly Property entryId As String
            Get
                If children.IsNullOrEmpty Then
                    Return name.Split.First
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property commonName As String
            Get
                If children.IsNullOrEmpty Then
                    Return name.GetTagValue(" ", trim:=True).Value
                Else
                    Return name
                End If
            End Get
        End Property

    End Class
End Namespace