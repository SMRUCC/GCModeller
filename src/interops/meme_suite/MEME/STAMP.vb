#Region "Microsoft.VisualBasic::cc61bb9b1a1cb100c105079b9bddfb75, meme_suite\MEME\STAMP.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class STAMP
    ' 
    '     Function: Convert, GetValue, ParseMatches
    ' 
    '     Sub: Invoke
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat

Public Class STAMP

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="meme_log"></param>
    ''' <param name="Id"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ------------------------
    ''' Motif 2 position-specific probability matrix
    ''' ------------------------
    ''' letter-probability matrix: alength= 4 w= 6 nsites= 31
    ''' 0 31 0 0
    ''' 29 0 0 2
    ''' 0 30 0 1
    ''' 2 1 28 0
    ''' 0 3 0 28
    ''' 0 0 31 0
    ''' </remarks>
    Public Function Convert(meme_log As XmlOutput.MEME.MEME, Id As String) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each Motif In meme_log.Motifs
            Handle += 1

            Call sBuilder.AppendLine("------------------------")
            Call sBuilder.AppendLine(String.Format("#{0}#{1} position-specific probability matrix", Handle, Id & "-" & Motif.Id))
            Call sBuilder.AppendLine("------------------------")
            Call sBuilder.AppendLine(String.Format("letter-probability matrix: alength= {0} w= {1} nsites= {2}", meme_log.TrainingSet.Alphabet.Length, Motif.Width, Motif.Sites))
            For Each Array In Motif.Probabilities.AlphabetMatrix
                Call sBuilder.AppendLine(String.Format("{0} {1} {2} {3}",
                                                       Array.GetValue("letter_A"),
                                                       Array.GetValue("letter_C"),
                                                       Array.GetValue("letter_G"),
                                                       Array.GetValue("letter_T")))
            Next
            Call sBuilder.AppendLine()
        Next

        Return sBuilder.ToString
    End Function

    ''' <summary>
    ''' 从MEME软件的输出文件夹中获取Motif信息
    ''' </summary>
    ''' <param name="LogDir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValue(LogDir As String) As String
        Dim FilePath As String = String.Format("{0}/meme.xml", LogDir)
        If Not FileIO.FileSystem.FileExists(FilePath) Then
            Console.WriteLine("IO_FILE_NOT_FOUND()::{0}", FilePath)
            Return ""
        End If
        Dim Output As XmlOutput.MEME.MEME = FilePath.LoadXml(Of XmlOutput.MEME.MEME)()
        Dim Id As String = FileIO.FileSystem.GetDirectoryInfo(LogDir).Name.Replace(".fsa", "")
        Return Me.Convert(Output, Id)
    End Function

    Dim Handle As Integer

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Dir">需要进行搜索的文件夹</param>
    ''' <param name="Exported">保存的文件名</param>
    ''' <param name="Split">分割的块的大小，默认不分割</param>
    ''' <remarks></remarks>
    Public Sub Invoke(Dir As String, Exported As String, Optional Split As Integer = -1)
        Dim sBuilder As StringBuilder = New StringBuilder(4096)

        Handle = 0

        If Split <= 0 Then
            For Each logdir As String In FileIO.FileSystem.GetDirectories(Dir)
                Call sBuilder.AppendLine(GetValue(logdir))
            Next
            Call sBuilder.ToString.SaveTo(Exported, System.Text.Encoding.ASCII)
        Else
            Dim i As Integer, j As Integer = 1

            For Each logdir As String In FileIO.FileSystem.GetDirectories(Dir)
                If i < Split Then
                    Call sBuilder.AppendLine(GetValue(logdir))
                    i += 1
                Else
                    Call sBuilder.ToString.SaveTo(Exported & "_part" & j, System.Text.Encoding.ASCII)
                    Call sBuilder.Clear()
                    j += 1
                    i = 0
                End If
            Next
            Call sBuilder.ToString.SaveTo(Exported & "_part" & j, System.Text.Encoding.ASCII)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="OriginalFile">输入到STAMP服务器中的原始数据</param>
    ''' <param name="Matches">从STAMP服务器获取的匹配数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ParseMatches(OriginalFile As String, Matches As String) As IO.File
        Dim MatcheCollection = Regex.Matches(FileIO.FileSystem.ReadAllText(OriginalFile), "[#]\d+[#]\S+", RegexOptions.Singleline)
        Dim Tokens As String() = Matches.ReadAllLines
        Dim MatchList As List(Of String()) = New List(Of String())
        Dim ChunkBuffer As String()

        For i As Integer = 0 To Tokens.Count - 1 Step 4
            ChunkBuffer = New String(2) {}
            Call Array.ConstrainedCopy(Tokens, i, ChunkBuffer, 0, 3)
            Call MatchList.Add(ChunkBuffer)
        Next

        Dim MatchPairs = (From item As String()
                              In MatchList
                          Let Handle As Integer = Val(Mid(item.First(), 6))
                          Let pair = New KeyValuePair(Of Integer, String())(Handle, {item(1), Mid(item(2), 9, 10)})
                          Select pair
                          Order By pair.Key Ascending).ToArray

        Dim File As IO.File = New IO.File
        Call File.AppendLine(New String() {"Id", "PWY_Id", "Motif", "E_val"})
        For Each Match As Match In MatcheCollection
            Dim Value As String = Match.Value
            Dim Handle_TEMP As String = Regex.Match(Value, "[#]\d+[#]").Value
            Dim Handle As Integer = Val(Regex.Match(Handle_TEMP, "\d+").Value)
            Dim IdName As String = Mid(Value, Len(Handle_TEMP) + 1)
            Dim Row As IO.RowObject = New IO.RowObject From {Handle, IdName}
            Dim Pair = MatchPairs(Handle - 1)
            Call Row.AddRange(New String() {Pair.Value.First, Pair.Value.Last})

            Call File.AppendLine(Row)
        Next

        Return File
    End Function
End Class
