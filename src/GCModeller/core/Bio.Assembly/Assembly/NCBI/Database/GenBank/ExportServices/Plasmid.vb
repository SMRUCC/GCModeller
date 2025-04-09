
Namespace Assembly.NCBI.GenBank.CsvExports

    ''' <summary>
    ''' tabular information about a plasmid replicate
    ''' </summary>
    Public Class Plasmid : Inherits gbEntryBrief

        Public Property PlasmidID As String
        Public Property PlasmidType As String
        Public Property isolation_source As String
        Public Property Country As String
        Public Property Host As String
        Public ReadOnly Property IsShortGun As Boolean
            Get
                Return InStr(Definition, "shotgun", CompareMethod.Text) > 0
            End Get
        End Property

        Public Overloads Shared Function Build(gbk As NCBI.GenBank.GBFF.File) As Plasmid
            Dim Plasmid As Plasmid = ConvertObject(Of Plasmid)(gbk)
            Plasmid.PlasmidID = gbk.Features.source.Query("plasmid")
            Plasmid.Host = gbk.Features.source.Query("host")
            Plasmid.Country = gbk.Features.source.Query("country")
            Plasmid.isolation_source = gbk.Features.source.Query("isolation_source")

            Return Plasmid
        End Function
    End Class
End Namespace