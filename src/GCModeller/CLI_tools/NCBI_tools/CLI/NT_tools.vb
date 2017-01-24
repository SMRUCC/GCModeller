#Region "Microsoft.VisualBasic::1a2e2aeebaf815d34bc1216ad2725214, ..\GCModeller\CLI_tools\NCBI_tools\CLI\NT_tools.vb"

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
    <Group(CLIGrouping.NTTools)>
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
                                      word = w).IteratesALL
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
    <Argument("/list", AcceptTypes:={GetType(WordTokens)})>
    <Group(CLIGrouping.NTTools)>
    Public Function NtNameMatches(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim list As String = args("/list")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & list.BaseName & ".match/")
        Dim names As WordTokens() = list.LoadCsv(Of WordTokens)
        Dim writer As Dictionary(Of String, StreamWriter)

        Const UNKNOWN$ = "#UNKNOWN"

        Try
            writer = names.ToDictionary(
                Function(x) x.name,
                Function(x) $"{out}/{x.name.NormalizePathString}.fasta".OpenWriter(Encodings.ASCII))
        Catch ex As Exception
            ex = New Exception("There is duplicated name in your input list data!", ex)
            Throw ex
        Finally
        End Try

        Call writer.Add(UNKNOWN, $"{out}/{UNKNOWN}.fasta".OpenWriter(Encodings.ASCII))

        Using stream As New StreamIterator([in])
            For Each fasta As FastaToken In stream.ReadStream
                Dim title As String = fasta.Attributes.Last.Trim
                Dim ms$() = LinqAPI.Exec(Of String) <=
 _
                    From x As WordTokens
                    In names.AsParallel
                    Where x.Match(title)
                    Select x.name

                Dim data As String = fasta.GenerateDocument(120)

                If ms.Length > 0 Then
                    For Each m As String In ms
                        Call writer(m).WriteLine(Data)
                    Next

                    Call Console.Write(".")
                Else
                    Call writer(UNKNOWN).WriteLine(Data)
                End If
            Next
        End Using

        For Each file In writer.Values
            Call file.Flush()
            Call file.Close()
            Call file.Dispose()
        Next

        Return True
    End Function

    <ExportAPI("/word.tokens", Usage:="/word.tokens /in <list.txt> [/out <out.csv>]")>
    <Group(CLIGrouping.NTTools)>
    Public Function GetWordTokens(args As CommandLine) As Integer
        Dim list$ = args("/in")
        Dim out$ = args.GetValue("/out", list.TrimSuffix & ".csv")
        Dim words$() = list.ReadAllLines
        Return WordTokens.GetTokens(words).ToArray.SaveTo(out).CLICode
    End Function
End Module


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

        If Similarity.Evaluate(title, name) >= 0.85 Then
#If DEBUG Then
            Call Console.Write("*")
#End If
            Return True
        Else
            Return title.IsOrdered(tokens, False, True)
        End If
    End Function

    Public Shared Iterator Function GetTokens(lines As IEnumerable(Of String)) As IEnumerable(Of WordTokens)
        For Each line$ In lines

            Yield New WordTokens With {
                    .name = line.Trim(" "c, ASCII.TAB),
                    .tokens = line.Trim _
                        .StripSymbol _
                        .Split _
                        .Distinct _
                        .Where(Function(s) Not String.IsNullOrEmpty(s.Trim(ASCII.TAB))) _
                        .ToArray
                }
        Next
    End Function
End Class
