Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.Entrez
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/Fasta.Filters",
               Usage:="/Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/out <out.fasta> /p]")>
    <ParameterInfo("/p",
                   True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.")>
    Public Function Filter(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim key As String = args("/key")
        Dim combine As String =
            If(key.FileExists, key.BaseName, key.NormalizePathString.Replace(" ", "_"))
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & combine & ".fasta")
        Dim source As New StreamIterator([in])
        Dim parallel As Boolean = args.GetBoolean("/p")

        Call "".SaveTo(out)

        If parallel Then
            Call "Using parallel edition!".__DEBUG_ECHO
        Else
            ' Call "Using single thread mode on the 32bit platform".__DEBUG_ECHO
        End If

        Using file As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate), Encoding.ASCII)

            file.AutoFlush = True

            If Not key.FileExists Then ' 使用单个单词进行查询
                Dim regex As New Regex(key, RegexICSng)

                If parallel Then
                    For Each block In LQuerySchedule.Where(source.ReadStream, Function(fa) regex.Match(fa.Title).Success)
                        For Each x In block
                            Call file.WriteLine(x.GenerateDocument(-1))
                        Next
                    Next
                Else
                    For Each fa As FastaToken In source.ReadStream
                        If regex.Match(fa.Title).Success Then
                            Call file.WriteLine(fa.GenerateDocument(-1))
                            ' Call fa.Title.__DEBUG_ECHO
                        End If
                    Next
                End If
            Else
                Dim words As String() = key.ReadAllLines ' 使用文件之中的一组关键词进行查询
                words = words.Select(Function(s) s.Split) _
                    .MatrixAsIterator _
                    .Distinct _
                    .ToArray

                Call words.GetJson.__DEBUG_ECHO

                For Each fa As FastaToken In source.ReadStream
                    For Each sKey As String In words
                        Dim title As String = fa.Title

                        If InStr(title, sKey, CompareMethod.Text) > 0 OrElse
                            Regex.Match(title, sKey, RegexICSng).Success Then

                            Call file.WriteLine(fa.GenerateDocument(-1))
                        End If
                    Next
                Next
            End If
        End Using

        Return 0
    End Function

    Const Interval As String = "NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN"

    <ExportAPI("/Contacts", Usage:="/Contacts /in <in.fasta> [/out <out.DIR>]")>
    Public Function Contacts(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim i As Integer = 1
        Dim contigs As New List(Of SimpleSegment)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Contigs/")
        Dim outNt As String = out & "/nt.fasta"
        Dim outContigs As String = out & "/contigs.csv"
        Dim il As Integer = Interval.Length

        Call "".SaveTo(outNt)

        Using writer As New StreamWriter(New FileStream(outNt, FileMode.OpenOrCreate), Encoding.ASCII)

            Call writer.WriteLine("> " & [in].BaseName)

            For Each fa As FastaToken In New StreamIterator([in]).ReadStream
                Call writer.Write(fa.SequenceData)
                Call writer.Write(Interval)

                Dim nx As Integer = i + fa.Length

                contigs += New SimpleSegment With {
                    .Start = i,
                    .Ends = nx,
                    .ID = fa.ToString,
                    .Strand = "+"
                }
                i = nx + il

                ' Call Console.Write(".")
            Next

            Call contigs.SaveTo(outContigs)
        End Using

        Return 0
    End Function

    <ExportAPI("/Taxonomy.efetch", Usage:="/Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]")>
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

        Return App.SelfFolk($"{GetType(CLI).API(NameOf(MergeFetchTaxonData))} /in {EXPORT.CliPath}").Run
    End Function

    <ExportAPI("/Taxonomy.efetch.Merge", Usage:="/Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]")>
    Public Function MergeFetchTaxonData(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in] & "/Taxonomy.efetch.Merge.Csv")

        Using writer As New WriteStream(Of SeqBrief)(out)
            For Each file As String In ls - l - r - wildcards("*.Xml") <= [in]
                Dim xml = file.LoadXml(Of TSeqSet)
                Dim info = xml.TSeq.ToArray(Function(x) DirectCast(x, SeqBrief))
                Call writer.Flush(info)
            Next

            Return 0
        End Using
    End Function
End Module
