Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Gast

    <Extension>
    Public Function ExportSILVA([in] As String, EXPORT As String) As Boolean
        Dim reader As New StreamIterator([in])
        Dim out As String = EXPORT & "/" & [in].BaseName & ".fasta"
        Dim tax As String = out.TrimFileExt & ".tax"

        Using ref As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate)),
            taxon As New StreamWriter(New FileStream(tax, FileMode.OpenOrCreate))

            ref.NewLine = vbLf
            taxon.NewLine = vbLf

            For Each fa As FastaToken In reader.ReadStream
                Dim title As String = fa.Title
                Dim uid As String = title.Split.First
                Dim taxnomy As String = Mid(title, uid.Length + 1).Trim

                uid = uid.Replace(".", "_")
                fa = New FastaToken({uid}, fa.SequenceData)
                title = {uid, taxnomy, "1"}.JoinBy(vbTab)

                Call ref.WriteLine(fa.GenerateDocument(60))
                Call taxon.WriteLine(title)
            Next
        End Using

        Return True
    End Function
End Module
