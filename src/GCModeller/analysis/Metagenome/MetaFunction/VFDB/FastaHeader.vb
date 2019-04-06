Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace VFDB

    ''' <summary>
    ''' http://www.mgc.ac.cn/VFs/download.htm
    ''' </summary>
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

            title = title.Remove(orgName, RegexOptions.None)
            title = title.Remove(xref, RegexOptions.None)

            Dim geneName = title.Matches("\(.+?\)").First
            Dim external = xref.Match("\(.+\)")
            Dim VFID = xref.Remove(external, RegexOptions.None)

            title = title.Remove(geneName, RegexOptions.None).Trim
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
