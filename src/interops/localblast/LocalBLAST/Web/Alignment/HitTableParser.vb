Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

Namespace NCBIBlastResult.WebBlast

    ''' <summary>
    ''' 从NCBI网站之中下载的比对结果的表格文本文件之中进行数据的解析操作
    ''' </summary>
    Module HitTableParser

        ''' <summary>
        ''' 当文件之中包含有多个表的时候使用
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function IterateTables(path$, headerSplit As Boolean) As IEnumerable(Of AlignmentTable)
            Dim headers As New List(Of String)
            Dim lines As New List(Of String)
            Dim line As New Value(Of String)
            Dim reader As StreamReader = path.OpenReader

            Do While Not reader.EndOfStream
                Do While (line = reader.ReadLine).First = "#"c
                    headers += (+line)
                Loop

                lines += (+line)

                Do While Not String.IsNullOrEmpty(line = reader.ReadLine) AndAlso
                    (+line).First <> "#"c
                    lines += (+line)
                Loop

                Yield lines.ToArray.__parseTable(path$, headers, headerSplit)

                headers *= 0
                lines *= 0
                headers += (+line)

                If String.IsNullOrEmpty(headers.First) Then
                    Do While Not reader.EndOfStream AndAlso String.IsNullOrEmpty(line = reader.ReadLine)
                    Loop
                    headers += (+line)
                End If
            Loop
        End Function

        ''' <summary>
        ''' 适用于一个文件只有一个表的时候
        ''' </summary>
        ''' <param name="path">使用NCBI的web blast的结果输出表的默认格式的文件</param>
        ''' <param name="headerSplit">
        ''' 当<see cref="HitRecord.SubjectIDs"/>之中包含有多个比对结果序列的时候，是否使用分号``;``作为分隔符将表头分开？默认不分开
        ''' </param>
        ''' <returns></returns>
        Public Function LoadTable(path$, Optional headerSplit As Boolean = False) As AlignmentTable
            Dim lines$() = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In path.ReadAllLines
                Where Not String.IsNullOrEmpty(s)
                Select s

            Dim header$() = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In lines
                Where InStr(s, "# ") = 1
                Select s

            If lines.IsNullOrEmpty Then
                Throw New InvalidExpressionException($"Target alignment table file ""{path}"" have no data!")
            End If

            Return lines _
                .Skip(header.Length) _
                .ToArray _
                .__parseTable(path$, header, headerSplit)
        End Function

        <Extension>
        Private Function __parseTable(lines$(), path$, header$(), headerSplit As Boolean) As AlignmentTable
            Dim hits As HitRecord() = lines _
                .Select(AddressOf HitTableParser.Mapper).ToArray
            Dim headAttrs As Dictionary(Of String, String) =
                header _
                .Skip(1) _
                .Select(Function(s) s.GetTagValue(": ")) _
                .ToDictionary(Function(x) x.Name,
                              Function(x) x.Value)

            If headerSplit Then
                hits = hits _
                    .Select(Function(x) x.SplitByHeaders) _
                    .ToVector
            End If

            Return New AlignmentTable With {
                .Hits = hits,
                .Program = header.First.Trim.Split.Last,
                .Query = headAttrs("# Query"),
                .Database = headAttrs("# Database"),
                .RID = headAttrs("# RID")
            }
        End Function

        ''' <summary>
        ''' Document line parser
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Function Mapper(s As String) As HitRecord
            Dim tokens As String() = s.Split(ASCII.TAB)
            Dim i As VBInteger = Scan0
            Dim hit As New HitRecord With {
                .QueryID = tokens(++i),
                .SubjectIDs = tokens(++i),
                .QueryAccVer = tokens(++i),
                .SubjectAccVer = tokens(++i),
                .Identity = Val(tokens(++i)),
                .AlignmentLength = Val(tokens(++i)),
                .MisMatches = Val(tokens(++i)),
                .GapOpens = Val(tokens(++i)),
                .QueryStart = Val(tokens(++i)),
                .QueryEnd = Val(tokens(++i)),
                .SubjectStart = Val(tokens(++i)),
                .SubjectEnd = Val(tokens(++i)),
                .EValue = Val(tokens(++i)),
                .BitScore = Val(tokens(++i))
            }

            Return hit
        End Function
    End Module
End Namespace