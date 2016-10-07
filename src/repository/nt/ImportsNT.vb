Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON
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
    ''' <param name="nt$">文件或者文件夹</param>
    ''' <param name="EXPORT$">序列数据所保存的文件夹</param>
    <Extension>
    Public Sub [Imports](mysql As mysqlClient, nt$, EXPORT$, Optional writeMysql As Boolean = True)
        Dim writer As New Dictionary(Of IndexWriter)
        Dim titles As New Dictionary(Of TitleWriter)

        Try
            Call FileIO.FileSystem.DeleteDirectory(
                EXPORT$,
                FileIO.DeleteDirectoryOption.DeleteAllContents)
        Catch ex As Exception

        End Try

        For Each seq As FastaToken In StreamIterator.SeqSource(nt, debug:=True)
            For Each h In NTheader.ParseNTheader(seq, throwEx:=False)
                Dim gi& = CLng(Val(h.gi))

                If CStr(gi) <> Trim(h.gi) Then
                    Call h.GetJson.Warning
                    Continue For
                End If

                Dim nt_header As New mysql.NCBI.nt With {
                    .db = h.db,
                    .description = MySqlEscaping(h.description),
                    .gi = gi,
                    .uid = h.uid
                }
                Dim index$ = nt_header.Index

                If Not writer.ContainsKey(index) Then
                    writer(index) = New IndexWriter(
                        EXPORT,
                        nt_header.db.ToLower,
                        index)
                End If
                If Not titles.ContainsKey(index) Then
                    titles(index) = New TitleWriter(
                        EXPORT,
                        nt_header.db.ToLower,
                        index)
                End If

                If writeMysql Then
                    Call mysql.ExecInsert(nt_header)
                End If

                Call writer(index).Write(seq.SequenceData, h)
                Call titles(index).Write(h)
            Next
        Next

        Call "Database imports Job Done! Closing file handles....".__DEBUG_ECHO

        For Each file In writer.Values
            Call file.Dispose()
        Next
        For Each file In titles.Values
            Call file.Dispose()
        Next
    End Sub

    <Extension>
    Public Function Index$(nt As mysql.NCBI.nt)
        Dim gi = Mid(CStr(nt.gi), 1, 2)
        If gi.Length = 1 Then
            gi = gi & "0"
        End If
        Return nt.db.ToLower & "-" & gi
    End Function
End Module
