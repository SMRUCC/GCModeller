Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace VFDB

    Public Class FastaHeader

        Public Property VFID As String
        Public Property xref As String
        Public Property geneName As String
        Public Property fullName As String
        Public Property organism As String

        Public Overrides Function ToString() As String
            Return fullName
        End Function

        Public Shared Function ParseHeader(fasta As FastaSeq) As FastaHeader
            Dim title = Strings.Trim(fasta.Title)
            Dim orgName = title.Matches("\[.+?\]").Last
            Dim xref = title.Split.First

            title = title.Replace(orgName, "")
            title = title.Replace(xref, "")

            Dim geneName = title.Matches("\(.+?\)").First
            Dim external = xref.Match("\(.+\)")
            Dim VFID = xref.Replace(external, "")

            title = title.Replace(geneName, "").Trim
            xref = external.GetStackValue("(", ")")

            Return New FastaHeader With {
                .VFID = VFID,
                .xref = xref,
                .geneName = geneName,
                .fullName = title,
                .organism = orgName
            }
        End Function
    End Class

End Namespace
