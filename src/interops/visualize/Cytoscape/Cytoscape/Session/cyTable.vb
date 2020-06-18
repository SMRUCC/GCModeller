Imports Microsoft.VisualBasic.Data.csv.IO

Namespace Session

    Public Class cyTable

        Public Property CyCSVVersion As String
        Public Property fields As cyField()

        Public Shared Function LoadTable(path As String) As cyTable
            Dim table = File.Load(path)
            Dim version As String = table.Rows(Scan0)(1)
            Dim getFields As cyField() =
                Iterator Function() As IEnumerable(Of cyField)
                    For Each col In table.Columns()
                        Dim name As String = col(1)
                        Dim type As String = col(2)
                        Dim mutable As String = col(3)
                        Dim note As String = col(4)
                        Dim data As String() = col.Skip(4).ToArray

                        Yield New cyField With {
                            .name = name,
                            .type = type,
                            .mutable = mutable = NameOf(mutable),
                            .note = note,
                            .data = data
                        }
                    Next
                End Function().ToArray

            Return New cyTable With {
                .CyCSVVersion = version,
                .fields = getFields
            }
        End Function
    End Class

    Public Class cyField
        Public Property name As String
        Public Property type As String
        Public Property mutable As Boolean
        Public Property note As String
        Public Property data As String()
    End Class
End Namespace


