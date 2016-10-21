Imports Microsoft.VisualBasic.Language

Namespace Assembly.Expasy.Database.csv

    Public Class SwissProt

        Public Property [Class] As String
        Public Property Entry As String
        Public Property seq As String

        Public Overrides Function ToString() As String
            Return Entry
        End Function

        Public Shared Function CreateObjects(Enzyme As Database.Enzyme) As SwissProt()
            Dim LQuery = LinqAPI.Exec(Of SwissProt) <=
 _
                From Id As String
                In Enzyme.SwissProt
                Select New SwissProt With {
                    .Class = Enzyme.Identification,
                    .Entry = Id
                }

            Return LQuery
        End Function
    End Class
End Namespace