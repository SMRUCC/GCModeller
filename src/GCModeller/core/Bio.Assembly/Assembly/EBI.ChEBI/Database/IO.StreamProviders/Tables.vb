Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

    Public Class BaseElements
        Public Property ID As String
        Public Property COMPOUND_ID As String
        Public Property TYPE As String
        Public Property SOURCE As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", TYPE, SOURCE, ID)
        End Function
    End Class

    Public Class Names : Inherits BaseElements

        Public Property NAME As String
        Public Property ADAPTED As String
        Public Property LANGUAGE As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) [{1}]{2}", ID, LANGUAGE, NAME)
        End Function
    End Class

    Public Class DatabaseAccession : Inherits BaseElements
        Public Property ACCESSION_NUMBER As String

        Public Overrides Function ToString() As String
            Return ACCESSION_NUMBER
        End Function
    End Class
End Namespace