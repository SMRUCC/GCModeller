#Region "Microsoft.VisualBasic::7eef05cf9a0afd39c5f2ad51bbd3c500, localblast\CLI_tools\CLI\FastaTools.vb"

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


    ' Code Statistics:

    '   Total Lines: 138
    '    Code Lines: 116 (84.06%)
    ' Comment Lines: 2 (1.45%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 20 (14.49%)
    '     File Size: 6.10 KB


    ' Module CLI
    ' 
    '     Function: FetchTaxnData, Filter, MergeFetchTaxonData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.Entrez
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Fasta.Filters",
               Info:="Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.",
               Usage:="/Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/tokens /out <out.fasta> /p]")>
    <ArgumentAttribute("/key", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String), GetType(String())},
              Description:="A regexp string term that will be using for title search or file path of a text file contains lines of regexp.")>
    <ArgumentAttribute("/p",
                   True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.")>
    <Group(CLIGrouping.BBHTools)>
    Public Function Filter(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim key As String = args("/key")
        Dim combine As String =
            If(key.FileExists, key.BaseName, key.NormalizePathString.Replace(" ", "_"))
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & combine & ".fasta")
        Dim source As New StreamIterator([in])
        Dim parallel As Boolean = args("/p")

        Call "".SaveTo(out)

        If parallel Then
            Call "Using parallel edition!".debug
        Else
            ' Call "Using single thread mode on the 32bit platform".debug
        End If

        Using file As New System.IO.StreamWriter(New FileStream(out, FileMode.OpenOrCreate), Encoding.ASCII)

            file.AutoFlush = True

            If Not key.FileExists Then ' 使用单个单词进行查询
                Dim regex As New Regex(key, RegexICSng)

                Call $"Compare regexp by key: {key}".debug

                If parallel Then
                    For Each block In LQuerySchedule.Where(source.ReadStream, Function(fa) regex.Match(fa.Title).Success)
                        For Each x In block
                            Call file.WriteLine(x.GenerateDocument(-1))
                        Next
                    Next
                Else
                    For Each fa As FastaSeq In source.ReadStream
                        If regex.Match(fa.Title).Success Then
                            Call file.WriteLine(fa.GenerateDocument(-1))
                            ' Call fa.Title.debug
                        End If
                    Next
                End If
            Else
                Dim words As String() = key.ReadAllLines ' 使用文件之中的一组关键词进行查询

                If args("/tokens") Then
                    words = words.Select(Function(s) s.Split) _
                        .IteratesALL _
                        .Distinct _
                        .ToArray
                End If

                Call words.GetJson.debug
                Call Tasks.Parallel.ForEach(source.ReadStream,
                    Sub(fa)
                        For Each sKey As String In words
                            Dim title As String = fa.Title

                            If InStr(title, sKey, CompareMethod.Text) > 0 OrElse
                                Regex.Match(title, sKey, RegexICSng).Success Then

                                SyncLock file
                                    Call file.WriteLine(fa.GenerateDocument(-1))
                                End SyncLock
                            End If
                        Next
                    End Sub)
            End If
        End Using

        Return 0
    End Function

    <ExportAPI("/Taxonomy.efetch",
               Info:="Fetch the taxonomy information of the fasta sequence from NCBI web server.",
               Usage:="/Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function FetchTaxnData(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy.efetch/")
        Dim reader As New StreamIterator([in])
        Dim i As New Pointer

        For Each result In reader.ReadStream.efetch
            Dim out As String = $"{EXPORT}/part{++i}.Xml"
            Call result.SaveAsXml(out)
            Call Console.Write("|")
            Call Thread.Sleep(1500)
        Next

        Return App.SelfFolk($"{GetType(CLI).API(NameOf(MergeFetchTaxonData))} /in {EXPORT.CLIPath}").Run
    End Function

    <ExportAPI("/Taxonomy.efetch.Merge", Usage:="/Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function MergeFetchTaxonData(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in] & "/Taxonomy.efetch.Merge.Csv")

        Using writer As New WriteStream(Of SeqBrief)(out)
            For Each file As String In ls - l - r - wildcards("*.Xml") <= [in]
                Dim xml = file.LoadXml(Of TSeqSet)
                Dim info = xml.TSeq.Select(Function(x) DirectCast(x, SeqBrief))
                Call writer.Flush(info)
            Next

            Return 0
        End Using
    End Function
End Module
