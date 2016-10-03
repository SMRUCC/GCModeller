#Region "Microsoft.VisualBasic::0d05de300cc184de0f400bb6f7c90864, ..\GCModeller\CLI_tools\NCBI_tools\CLI\NT_tools.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ' 这里主要是和处理nt数据库文件的相关工具

    <ExportAPI("/nt.matches.key", Usage:="/nt.matches.key /in <nt.fasta> /list <words.txt> [/out <out.fasta>]")>
    Public Function NtKeyMatches(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim list As String = args("/list")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_" & list.BaseName & ".fasta")
        Dim terms As String() =
            list _
            .ReadAllLines _
            .Where(Function(s) Not s.IsBlank) _
            .ToArray(Function(s) s.Trim.ToLower)
        Dim words As Dictionary(Of String, String()) =
            (From term In (From line As String
                           In terms
                           Let ws As String() = line.Split
                           Select From w As String
                                  In ws
                                  Select termLine = line,
                                      word = w).MatrixAsIterator
             Select term
             Group term.termLine By term.word Into Group) _
                .ToDictionary(Function(x) x.word,
                              Function(x) x.Group.Distinct.ToArray)

        Call words.GetJson(True).__DEBUG_ECHO

        Using writer As StreamWriter = out.OpenWriter(encoding:=Encodings.ASCII)
            For Each fa As FastaToken In New StreamIterator([in]).ReadStream
                For Each word As String In words.Keys

                    Dim attrs As String() = fa.Attributes
                    Dim title As String = fa.Title.ToLower
                    Dim writeData = Sub()
                                        Dim hit As New List(Of String)(attrs)
                                        hit += "word:=" & word
                                        hit += String.Join("; ", words(word))
                                        Dim write As New FastaToken(hit, fa.SequenceData)

                                        Call writer.WriteLine(write.GenerateDocument(120))
                                        Call write.Title.__DEBUG_ECHO
                                    End Sub

                    If InStr(title, word) > 0 Then
                        Call writeData()
                    Else
                        For Each s As String In attrs
                            For Each x As String In s.Trim.ToLower.Split

                                Dim d = LevenshteinDistance.ComputeDistance(x, word)

                                If Not (d Is Nothing OrElse d.Score < 0.8) Then
                                    Call writeData()
                                End If
                            Next
                        Next
                    End If
                Next
#If DEBUG Then
                Console.WriteLine()
#End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/nt.matches.name", Usage:="/nt.matches.name /in <nt.fasta> /list <names.csv> [/out <out.fasta>]")>
    <ParameterInfo("/list", AcceptTypes:={GetType(WordTokens)})>
    Public Function NtNameMatches(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim list As String = args("/list")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & list.BaseName & ".match/")
        Dim names As WordTokens() = list.LoadCsv(Of WordTokens)
        Dim writer As Dictionary(Of String, StreamWriter) = names.ToDictionary(
            Function(x) x.name,
            Function(x) $"{out}/{x.name.NormalizePathString}.fasta".OpenWriter(Encodings.ASCII))

        Using stream As New StreamIterator([in])
            For Each fasta As FastaToken In stream.ReadStream
                Dim title As String = fasta.Title
            Next
        End Using

        For Each file In writer.Values
            Call file.Flush()
            Call file.Close()
            Call file.Dispose()
        Next

        Return True
    End Function

    Public Class WordTokens

        Public Property name As String
        Public Property tokens As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Match(title As String) As Boolean
            If InStr(name, title, CompareMethod.Text) > 0 OrElse
                InStr(title, name, CompareMethod.Text) > 0 Then
                Return True
            End If

            Dim dist = LevenshteinDistance.ComputeDistance(title.ToLower, name.ToLower)

            If Not dist Is Nothing AndAlso dist.MatchSimilarity >= 0.65 Then
                Return True
            End If

            Dim n As Integer

            For Each t$ In tokens
                If InStr(title, t, CompareMethod.Text) > 0 Then
                    n += 1
                End If
            Next

            Return n > 1
        End Function

        Public Shared Iterator Function GetTokens(lines As IEnumerable(Of String)) As IEnumerable(Of WordTokens)
            For Each line As String In lines
                Yield New WordTokens With {
                    .name = line,
                    .tokens = line.Trim _
                        .StripSymbol _
                        .Split _
                        .Distinct _
                        .Where(Function(s) Not String.IsNullOrEmpty(s.Trim(vbTab))) _
                        .ToArray
                }
            Next
        End Function
    End Class

    <ExportAPI("/word.tokens", Usage:="/word.tokens /in <list.txt> [/out <out.csv>]")>
    Public Function GetWordTokens(args As CommandLine) As Integer
        Dim list$ = args("/in")
        Dim out$ = args.GetValue("/out", list.TrimSuffix & ".csv")
        Dim words$() = list.ReadAllLines
        Return WordTokens.GetTokens(words).ToArray.SaveTo(out).CLICode
    End Function
End Module

