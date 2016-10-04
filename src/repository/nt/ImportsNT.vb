Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports mysqlClient = Oracle.LinuxCompatibility.MySQL.MySQL

''' <summary>
''' 向数据库之中导入NT数据的操作
''' </summary>
Public Module ImportsNT

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mysql"></param>
    ''' <param name="nt$"></param>
    ''' <param name="EXPORT$">序列数据所保存的文件夹</param>
    <Extension>
    Public Sub [Imports](mysql As mysqlClient, nt$, EXPORT$)
        Dim writer As New Dictionary(Of String, StreamWriter)

        For Each seq As FastaToken In New StreamIterator(nt).ReadStream
            For Each h In NTheader.ParseNTheader(seq)
                Dim nt_header As New mysql.NCBI.nt With {
                    .db = h.db,
                    .description = MySqlEscaping(h.description),
                    .gi = h.gi,
                    .uid = h.uid
                }
                Dim indexFile As String = $"{EXPORT}/{nt_header.Index}.nt"
                Dim line$ = nt_header.gi & vbTab & seq.SequenceData

                If Not writer.ContainsKey(indexFile) Then
                    writer(indexFile) = indexFile.OpenWriter(Encodings.ASCII)
                End If

                Call writer(indexFile).WriteLine(line$)
                Call mysql.ExecInsert(nt_header)
            Next
        Next
    End Sub

    <Extension>
    Public Function Index$(nt As mysql.NCBI.nt)
        Return nt.db.ToLower & "-" & Mid(CStr(nt.gi), 1, 1)
    End Function
End Module
