#Region "Microsoft.VisualBasic::45ce3cdb2bdc551c2fabd758ced2d8eb, CLI_tools\NCBI_tools\CLI\NT_tools.vb"

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

    ' Module CLI
    ' 
    '     Function: GetWordTokens, NtAccessionMatches, NtKeyMatches, NtNameMatches
    ' 
    ' Class WordTokens
    ' 
    '     Properties: name, tokens
    ' 
    '     Function: GetTokens, Match, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ' 这里主要是和处理nt数据库文件的相关工具

    <ExportAPI("/nt.matches.accession")>
    <Usage("/nt.matches.accession /in <nt.fasta> /list <accession.list> [/accid <default=""tokens '.' first""> /out <subset.fasta>]")>
    <Description("Create subset of the nt database by a given list of Accession ID.")>
    <Group(CLIGrouping.NTTools)>
    Public Function NtAccessionMatches(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim list$ = args <= "/list"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}_subsetof({list.BaseName}).fasta"
        Dim idlist As String() = list.ReadAllLines
        Dim accid As TextGrepMethod = TextGrepScriptEngine _
            .Compile(args("/accid") Or "tokens '.' first") _
            .PipelinePointer
        Dim nt As IEnumerable(Of FastaSeq) = New StreamIterator([in]).ReadStream

        Using writer As StreamWriter = out.OpenWriter(encoding:=Encodings.ASCII)
            For Each fa As FastaSeq In Nucleotide.NtAccessionMatches(nt, idlist, accid)
                Call writer.WriteLine(fa.GenerateDocument(120))
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/nt.matches.key", Usage:="/nt.matches.key /in <nt.fasta> /list <words.txt> [/out <out.fasta>]")>
    <Group(CLIGrouping.NTTools)>
    Public Function NtKeyMatches(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim list As String = args("/list")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_" & list.BaseName & ".fasta")
        Dim terms As String() =
            list _
            .ReadAllLines _
            .Where(Function(s) Not s.StringEmpty) _
            .Select(Function(s) s.Trim.ToLower).ToArray
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
            For Each fa As FastaSeq In New StreamIterator([in]).ReadStream
                For Each word As String In words.Keys

                    Dim attrs As String() = fa.Headers
                    Dim title As String = fa.Title.ToLower
                    Dim writeData = Sub()
                                        Dim hit As New List(Of String)(attrs)
                                        hit += "word:=" & word
                                        hit += String.Join("; ", words(word))
                                        Dim write As New FastaSeq(hit, fa.SequenceData)

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
            For Each fasta As FastaSeq In stream.ReadStream
                Dim title As String = fasta.Headers.Last.Trim
                Dim ms$() = LinqAPI.Exec(Of String) <=
 _
                    From x As WordTokens
                    In names.AsParallel
                    Where x.Match(title)
                    Select x.name

                Dim data As String = fasta.GenerateDocument(120)

                If ms.Length > 0 Then
                    For Each m As String In ms
                        Call writer(m).WriteLine(data)
                    Next

                    Call Console.Write(".")
                Else
                    Call writer(UNKNOWN).WriteLine(data)
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
