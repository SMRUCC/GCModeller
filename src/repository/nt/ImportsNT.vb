'#Region "Microsoft.VisualBasic::e94d25e9fb57a8d7f0265afcd53530c1, nt\ImportsNT.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module ImportsNT
'    ' 
'    '     Function: Index
'    ' 
'    '     Sub: [Imports]
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.IO
'Imports System.Runtime.CompilerServices
'Imports Microsoft.VisualBasic.ComponentModel.Collection
'Imports Microsoft.VisualBasic.Serialization.JSON
'Imports Oracle.LinuxCompatibility.MySQL.Scripting
'Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
'Imports SMRUCC.genomics.SequenceModel.FASTA
'Imports mysqlClient = Oracle.LinuxCompatibility.MySQL.MySqli

'''' <summary>
'''' 向数据库之中导入NT数据的操作
'''' </summary>
'Public Module ImportsNT

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="mysql"></param>
'    ''' <param name="nt$">文件或者文件夹</param>
'    ''' <param name="EXPORT$">序列数据所保存的文件夹</param>
'    <Extension>
'    Public Sub [Imports](mysql As mysqlClient, nt$, EXPORT$, Optional writeMysql As Boolean = True)
'        Dim writer As New Dictionary(Of IndexWriter)
'        Dim titles As New Dictionary(Of TitleWriter)

'        Try
'            Call FileIO.FileSystem.DeleteDirectory(
'                EXPORT$,
'                FileIO.DeleteDirectoryOption.DeleteAllContents)
'        Catch ex As Exception

'        End Try

'        For Each seq As FastaSeq In StreamIterator.SeqSource(nt, debug:=True)
'            For Each h In NTheader.ParseNTheader(seq, throwEx:=False)
'                Dim gi& = CLng(Val(h.gi))

'                If CStr(gi) <> Trim(h.gi) Then
'                    Call h.GetJson.Warning
'                    Continue For
'                End If

'                Dim nt_header As New mysql.NCBI.nt With {
'                    .db = h.db,
'                    .description = MySqlEscaping(h.description),
'                    .gi = gi,
'                    .uid = h.uid
'                }
'                Dim index$ = nt_header.Index

'                If Not writer.ContainsKey(index) Then
'                    writer(index) = New IndexWriter(
'                        EXPORT,
'                        nt_header.db.ToLower,
'                        index)
'                End If
'                If Not titles.ContainsKey(index) Then
'                    titles(index) = New TitleWriter(
'                        EXPORT,
'                        nt_header.db.ToLower,
'                        index)
'                End If

'                If writeMysql Then
'                    Call mysql.ExecInsert(nt_header)
'                End If

'                Call writer(index).Write(seq.SequenceData, h)
'                Call titles(index).Write(h)
'            Next
'        Next

'        Call "Database imports Job Done! Closing file handles....".__DEBUG_ECHO

'        For Each file In writer.Values
'            Call file.Dispose()
'        Next
'        For Each file In titles.Values
'            Call file.Dispose()
'        Next
'    End Sub

'    <Extension>
'    Public Function Index$(nt As mysql.NCBI.nt)
'        Dim gi = Mid(CStr(nt.gi), 1, 2)
'        If gi.Length = 1 Then
'            gi = gi & "0"
'        End If
'        Return nt.db.ToLower & "-" & gi
'    End Function
'End Module
