''' <summary>
''' helper module for download geo dataset
''' </summary>
Public Class GEODataSet

    ReadOnly geo_url As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geo_acc">the GEO dataset accession id.</param>
    Sub New(geo_acc As String)
        geo_url = $"https://www.ncbi.nlm.nih.gov/geo/query/acc.cgi?acc={geo_acc}"
    End Sub

End Class
