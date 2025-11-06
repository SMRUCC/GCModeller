#Region "Microsoft.VisualBasic::d7ea6e9f4bc027693c42a17a2b27e383, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\Parser.vb"

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

    '   Total Lines: 467
    '    Code Lines: 295 (63.17%)
    ' Comment Lines: 93 (19.91%)
    '    - Xml Docs: 93.55%
    ' 
    '   Blank Lines: 79 (16.92%)
    '     File Size: 21.26 KB


    '     Module Parser
    ' 
    '         Function: __parsingInner, IsBlastn, isUltraLargeFileSize, LoadBlastOutput, ParseDbName
    '                   ParsingSizeAuto, TryParse, UltraLarge
    '         Enum ReaderTypes
    ' 
    '             BLASTN, BLASTP, BLASTX
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: DefaultEncoding
    ' 
    '     Function: (+2 Overloads) __blockWorker, __queryParser, __tryParseBlastnOutput, __tryParseUltraLarge, [GetType]
    '               BuildGrepScript, doLoadDataInternal, IsBlastOut, measureParser, TryParseBlastnOutput
    '               TryParseUltraLarge
    ' 
    '     Sub: (+2 Overloads) Transform
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228
Imports defaultEncoding = Microsoft.VisualBasic.Language.Default.[Default](Of System.Text.Encoding)
Imports r = System.Text.RegularExpressions.Regex
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Module Parser

        Public Function ParseDbName(s As String) As String
            s = r.Match(s, "Database: .+?\s+\d+\s+sequences;", RegexOptions.Singleline).Value
            s = s.Replace("Database:", "").Replace(vbLf, "").Replace(vbCr, "")
            s = r.Replace(s, "\d+\s+sequences;", "").Trim

            ' 这个database属性只是调试之类使用的，好像也没有太多用途，空下来也无所谓
            If s.StringEmpty Then
                Return ""
            Else
                Return s.BaseName
            End If
        End Function

        ''' <summary>
        ''' The parser worker will be select automatically based on the blast output file size.
        ''' (当blast的输出文件非常大的时候，会自动选择分片段解析读取的方法工作)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Function ParsingSizeAuto(path As String, Optional encoding As Encoding = Nothing) As v228
            If isUltraLargeFileSize(path) Then
                Return Parser.TryParseUltraLarge(path, encoding:=encoding)
            Else
                Return Parser.TryParse(path)
            End If
        End Function

        Const UltraLargeSize& = CLng(1024) * 1024 * 1024 * 10

        ''' <summary>
        ''' 判断文件的大小是否是输入超大的文件类型的
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function UltraLarge(path As String) As Boolean
            Dim file = FileIO.FileSystem.GetFileInfo(path)
            Return file.Length >= UltraLargeSize
        End Function

        ''' <summary>
        ''' 单条字符串的最大长度好像只有700MB的内存左右
        ''' </summary>
        ''' <returns></returns>
        Private Function isUltraLargeFileSize(infile As String) As Boolean
            Dim fileInfo = FileIO.FileSystem.GetFileInfo(infile)

            If fileInfo.Length >= 768& * 1024 * 1024 Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Load the blast output from a local file, if the exceptions happened during the data loading process, then a 
        ''' null value will be return.
        ''' (读取blast日志文件，当发生错误的时候，函数返回空值)
        ''' </summary>
        ''' <param name="path">The file path of the blast output</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadBlastOutput(path As String) As v228
            Try
                Return TryParse(path:=path)
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Log file size smaller than 512MB is recommended using this loading method, if the log file size is large than 512MB, 
        ''' please using the method <see cref="TryParseUltraLarge"></see>.
        ''' (当blast输出的日志文件比较小的时候，可以使用当前的这个方法进行读取)
        ''' </summary>
        ''' <param name="Path">Original plant text file path of the blast output file.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParse(path As String, Optional encoding As Encoding = Nothing) As v228
            Call "Regular Expression parsing blast output...".debug

            If encoding Is Nothing Then
                encoding = Encoding.Default
            End If

            Using p As New CBusyIndicator(start:=True)
                Dim source As String = IO.File.ReadAllText(path, encoding)

                If IsBlastn(source) Then
                    Return __tryParseBlastnOutput(source, path)
                Else
                    Return __parsingInner(source, path)
                End If
            End Using
        End Function

        Private Function __parsingInner(source As String, Path As String) As v228
            Dim lstQuery As String() = __queryParser(source)

            Call "[Parsing Job Done!]".debug

            Dim Sw As Stopwatch = Stopwatch.StartNew
            Dim parallel As Boolean
#If DEBUG Then
            parallel = False
#Else
            parallel = True
#End If
            Dim Queries As Query() = lstQuery _
                .Select(Function(line) Query.TryParse(line)) _
                .ToArray
            Dim BLASTOutput As v228 = New v228 With {
                .FilePath = Path & ".xml",
                .Queries = Queries,
                .Database = ParseDbName(source)
            }

            Call $"BLASTOutput file loaded: {Sw.ElapsedMilliseconds}ms".debug

            Return BLASTOutput
        End Function

        Private Function IsBlastn(s As String) As Boolean
            Dim FirstLine As String = Mid(s, 1, 20)
            Return Regex.Match(FirstLine, "BLASTN \d+\.\d+\.\d+\+", RegexOptions.IgnoreCase).Success
        End Function

        ''' <summary>
        ''' + BLASTP
        ''' + BLASTN
        ''' + BLASTX
        ''' </summary>
        Public Enum ReaderTypes
            BLASTP
            BLASTN
            BLASTX
        End Enum

        ''' <summary>
        ''' 判断目标文件是否为一个blast+的原始输出的结果文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Function IsBlastOut(path$) As Boolean
            Dim firstLine$ = path.ReadFirstLine
            Dim result As Boolean = Regex.Match(firstLine, "BLAST.+?\d(\.\d)+", RegexICSng).Success
            Return result
        End Function

        Public ReadOnly Property DefaultEncoding As defaultEncoding = Encoding.UTF8

        ''' <summary>
        ''' The target blast output text file is from the blastp or blastn?
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="Encoding"></param>
        ''' <returns></returns>
        Public Function [GetType](path As String, Optional Encoding As Encoding = Nothing) As ReaderTypes
            Dim s As String

            Using IO As New FileStream(path, FileMode.Open, FileAccess.Read)
                Dim buf As Byte() = New Byte(1024 * 5) {}
                IO.Read(buf, 0, buf.Length - 1)
                s = (Encoding Or DefaultEncoding).GetString(buf)
            End Using

            If IsBlastn(s) Then
                Return ReaderTypes.BLASTN
            Else
                Return ReaderTypes.BLASTP
            End If
        End Function

        ''' <summary>
        ''' Load the blastn output data.(读取blastn日志输出的数据.)
        ''' </summary>
        ''' <param name="Path">The blastn output file path.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParseBlastnOutput(Path As String) As v228
            Return __tryParseBlastnOutput(FileIO.FileSystem.ReadAllText(Path), Path)
        End Function

        Private Function __tryParseBlastnOutput(sourceText As String, LogFile As String) As v228
            Dim Sections As String() = __queryParser(sourceText)

            Call "Parsing job done!".debug

            Dim Sw As Stopwatch = Stopwatch.StartNew
            Dim parallel As Boolean = True
#If DEBUG Then
            parallel = False
#End If
            Dim lstQuery = Sections.Select(AddressOf Query.BlastnOutputParser).ToArray
            Dim BLASTOutput As New v228 With {
                .FilePath = LogFile & ".xml",
                .Queries = lstQuery
            }

            Call Console.WriteLine($"BLASTOutput file loaded: {Sw.ElapsedMilliseconds}ms")

            Return BLASTOutput
        End Function

        ''' <summary>
        ''' It seems 768MB possibly is the up bound of the Utf8.GetString function.
        ''' </summary>
        ''' <remarks></remarks>
        Const CHUNK_SIZE As Long = 1024 * 1024 * 72
        Const BLAST_QUERY_HIT_SECTION As String = "Query=.+?Effective search space used: \d+"

        Private Function __queryParser(source As String) As String()
            Call $"Regular Expression working on the query parser => {source.Length} chars.... this may takes a while.....".debug
            Dim LQuery = Regex.Matches(source, BLAST_QUERY_HIT_SECTION, RegexICSng).ToArray
            Return LQuery
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function BuildGrepScript(script As String) As TextGrepMethod
            If script.StringEmpty OrElse script = "-" Then
                Return Function(str) str
            Else
                Return TextGrepScriptEngine _
                    .Compile(script) _
                    .PipelinePointer
            End If
        End Function

        ''' <summary>
        ''' File processor for the file size which is greater than 10GB.
        ''' (处理非常大的blast输出文件的时候所需要的，大小大于10GB的文件建议使用这个方法处理)
        ''' </summary>
        Public Sub Transform(path As Value(Of String), CHUNK_SIZE&, transform As Action(Of Query), Optional encoding As Encoding = Nothing, Optional grep As (q$, s$) = Nothing)
            Dim parser As QueryParser = measureParser(path = path.Value.FixPath)
            Dim readsBuffer As IEnumerable(Of String) = doLoadDataInternal(path, CHUNK_SIZE, encoding Or DefaultEncoding)

            Call $"Open file handle {path.Value.ToFileURL} for data loading...".debug

            Dim q, s As TextGrepMethod

            If grep.q.StringEmpty OrElse grep.q = "-" Then
                q = Function(str) str
            Else
                q = TextGrepScriptEngine.Compile(grep.q).PipelinePointer
            End If

            If grep.s.StringEmpty OrElse grep.s = "-" Then
                s = Function(str) str
            Else
                s = TextGrepScriptEngine.Compile(grep.s).PipelinePointer
            End If

            Dim script As (q As TextGrepMethod, s As TextGrepMethod) = (q, s)

            For Each line$ In readsBuffer
                Call line$.__blockWorker(transform, parser, grep:=script)
            Next
        End Sub

        ''' <summary>
        ''' 处理非常大的blast输出文件的时候所需要的，大小大于10GB的文件建议使用这个方法处理
        ''' </summary>
        Public Sub Transform(path$,
                             CHUNK_SIZE%,
                             transform As Action(Of Query()),
                             Optional encoding As Encoding = Nothing,
                             Optional grep As (q$, s$) = Nothing)

            Call path.FixPath

            Dim parser As QueryParser = measureParser(path)
            Dim readsBuffer As IEnumerable(Of String) = doLoadDataInternal(path, CHUNK_SIZE, encoding Or DefaultEncoding)

            Call $"Open file handle {path.ToFileURL} for data loading...".debug
            Call (From line As String In readsBuffer Select __blockWorker(line, transform, parser, grep)).ToArray
        End Sub

        Private Function __blockWorker(queryBlocks As String, transform As Action(Of Query()), parser As QueryParser, grep As (q$, s$)) As Boolean
            Dim queries As String() = __queryParser(queryBlocks.Replace(ASCII.NUL, " "c))
            Call ($"[Parsing Job Done!]  ==> {queries.Length} Queries..." & vbCrLf & vbTab & vbTab & "Start to loading blast query hits data...").debug
            Dim LQuery = (From x As String In queries.AsParallel Select parser(x)).ToArray
            Dim grepq As TextGrepScriptEngine = TextGrepScriptEngine.Compile(grep.q)
            Dim greps As TextGrepScriptEngine = TextGrepScriptEngine.Compile(grep.s)

            For Each q As Query In LQuery
                q.QueryName = grepq.Grep(q.QueryName)

                For Each s In q.SubjectHits
                    s.Name = greps.Grep(s.Name)
                Next
            Next

            Call transform(LQuery)

            Return True
        End Function

        <Extension>
        Private Function __blockWorker(queryBlocks$, transform As Action(Of Query), parser As QueryParser, grep As (q As TextGrepMethod, s As TextGrepMethod)) As Boolean
            Dim queries As String() = __queryParser(queryBlocks.Replace(ASCII.NUL, " "c))
            Call ($"[Parsing Job Done!]  ==> {queries.Length} Queries..." & vbCrLf & vbTab & vbTab & "Start to loading blast query hits data...").debug
            Dim LQuery As Query() = queries _
                .AsParallel _
                .Select(Function(s) parser(s)) _
                .ToArray

            For Each query As Query In LQuery
                query.QueryName = grep.q(query.QueryName)

                For Each s In query.SubjectHits.SafeQuery
                    s.Name = grep.s(s.Name)
                Next

                Call transform(query)
            Next

            Return True
        End Function

        ''' <summary>
        ''' 获取blastn或者blastp的解析器
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Private Function measureParser(path As String) As QueryParser
            Dim parser As QueryParser

            Select Case LocalBLAST.BLASTOutput.BlastPlus.Parser.GetType(path)
                Case ReaderTypes.BLASTN : parser = AddressOf Query.BlastnOutputParser
                Case ReaderTypes.BLASTP : parser = AddressOf Query.TryParse
                    'Case ReaderTypes.BLASTX
                Case Else
                    parser = AddressOf Query.TryParse
            End Select

            Return parser
        End Function

        ''' <summary>
        ''' Dealing with the file size large than 2GB.
        ''' (当Blast日志文件的大小大于100M的时候，可以使用这个方法进行加载，函数会自动判断日志是否为blastn还是blastp)
        ''' </summary>
        ''' <param name="path">File path of the blast output file.</param>
        ''' <param name="CHUNK_SIZE">The parameter unit for this value is Byte, so you need to multiply the 1024*1024 to get a MB level value.
        ''' It seems 768MB possibly is the up bound of the Utf8.GetString function. default is 64MB</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __tryParseUltraLarge(path As String, CHUNK_SIZE As Long, Encoding As Encoding) As v228
            Dim readsBuffer As IEnumerable(Of String) = doLoadDataInternal(path, CHUNK_SIZE, Encoding)
            Call "[Loading Job Done!] Start to regex parsing!".debug

            ' The regular expression parsing function just single thread, here using parallel to parsing 
            ' the cache data can speed up the regular expression parsing job when dealing with the ultra 
            ' large Text file.
            Dim sections As LinkedList(Of String) = (From strLine As String
                                                     In readsBuffer.AsParallel
                                                     Let strData As String = strLine.Replace(oldChar:=ASCII.NUL, newChar:=" "c)
                                                     Select __queryParser(strData)).MatrixToUltraLargeVector

            Call ($"[Parsing Job Done!]  ==> {sections.Count} Queries..." & vbCrLf & vbTab & vbTab &
                "Start to loading blast query hits data...").debug

            Dim Sw As Stopwatch = Stopwatch.StartNew
            Dim queryParser As QueryParser = measureParser(path)
            Dim LQuery As Query() = LinqAPI.Exec(Of Query) <= From line As String
                                                              In sections.AsParallel
                                                              Let query As Query = queryParser(line)
                                                              Select query
                                                              Order By query.QueryName Ascending
            Dim BLASTOutput As New v228 With {
                .FilePath = path & ".xml",
                .Queries = LQuery
            }

            Call $"BLASTOutput file loaded: {Sw.ElapsedMilliseconds}ms...".debug

            Return BLASTOutput
        End Function

        Private Iterator Function doLoadDataInternal(path As String, CHUNK_SIZE As Long, Encoding As Encoding) As IEnumerable(Of String)
            Dim readBuffer As Byte() = New Byte(CHUNK_SIZE - 1) {}
            Dim LastIndex As New StringBuilder '(capacity:=Integer.MaxValue)'StringBuilder不能够太大，会出现内存溢出的错误
            Dim bufLength& = FileIO.FileSystem.GetFileInfo(path).Length

            Call $"[{NameOf(bufLength)}:={bufLength} bits]".debug

            Using textReader As IO.FileStream = New IO.FileStream(path, IO.FileMode.Open)
                Do While textReader.Position < bufLength
                    Dim Delta As Double = bufLength - textReader.Position  ' 请注意，当文件上GB的大小的时候，在这里使用Integer数据类型的时候会导致溢出！

                    If Delta < CHUNK_SIZE Then readBuffer = New Byte(Delta - 1) {}
                    If Delta <= 1 Then Exit Do ' 已经读取到流的尾部了

                    Call textReader.Read(readBuffer, 0, readBuffer.Count - 1)

                    Dim SourceText As String = Encoding.GetString(readBuffer)

                    If Not LastIndex.IsNullOrEmpty Then
                        Call LastIndex.Append(SourceText)
                        SourceText = LastIndex.ToString
                    End If

                    Dim i_LastIndex As Integer = InStrRev(SourceText, "Effective search space used:")
                    If i_LastIndex = 0 Then  ' 当前区间之中没有一个完整的Section
                        Call LastIndex.Append(SourceText)
                        Continue Do
                    Else
                        Call LastIndex.Clear()
                        i_LastIndex += 42

                        If Not i_LastIndex >= Len(SourceText) Then
                            Call LastIndex.Append(Mid(SourceText, i_LastIndex)) ' There are some text in the last of this chunk is the part of the section in the next chunk.
                        End If

                        Call $"Yield data source [buffer={SourceText.LongCount}]".debug

                        Yield SourceText
                    End If
                Loop

                If LastIndex.Length > 0 Then
                    Call $"Yield data source [buffer={LastIndex.Length}]".debug
                    Yield LastIndex.ToString
                End If
            End Using
        End Function

        ''' <summary>
        ''' Dealing with the file size large than 2GB.(当Blast日志文件的大小大于100M的时候，可以使用这个方法进行加载，函数会自动判断日志是否为blastn还是blastp)
        ''' </summary>
        ''' <param name="path">File path of the blast output file.</param>
        ''' <param name="CHUNK_SIZE">The parameter unit for this value is Byte, so you need to multiply the 1024*1024 to get a MB level value.
        ''' It seems 768MB possibly is the up bound of the Utf8.GetString function. default is 64MB</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParseUltraLarge(path$, Optional CHUNK_SIZE& = Parser.CHUNK_SIZE, Optional encoding As Encoding = Nothing) As v228
            Call Console.WriteLine("Regular Expression parsing blast output...")
            Call path.FixPath

            Using busy As New CBusyIndicator(start:=True)
                ' The default text encoding of the blast log is utf8
                Return __tryParseUltraLarge(path, CHUNK_SIZE, encoding Or UTF8)
            End Using
        End Function
    End Module
End Namespace
