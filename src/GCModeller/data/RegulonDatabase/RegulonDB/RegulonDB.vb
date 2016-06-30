Imports Oracle.LinuxCompatibility.MySQL
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.SequenceModel.FASTA

Namespace RegulonDB

    Public Class RegulonDB

        Dim DbReflector As MySQL

        Sub New(MySQL As ConnectionUri)
            DbReflector = New MySQL(MySQL)
        End Sub

        Public Function ExportSites() As FastaFile
            Dim Table = DbReflector.Query(Of Tables.site)("select * from site")
            Dim File As FastaFile = __export(Table.ToArray)
            Return File
        End Function

        Private Shared Function __export(table As Generic.IEnumerable(Of Tables.site)) As FastaFile
            Dim fasta = table.ToArray(Function(site) __export(site))
            Return CType(fasta, FastaFile)
        End Function

        Private Shared Function __export(site As Tables.site) As SequenceModel.FASTA.FastaToken
            Dim attrs As String() = New String() {
                site.key_id_org,
                site.site_id,
                site.site_internal_comment,
                site.site_length,
                site.site_note,
                site.site_posleft,
                site.site_posright
            }
            Return New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                .SequenceData = site.site_sequence,
                .Attributes = attrs
            }
        End Function
    End Class
End Namespace