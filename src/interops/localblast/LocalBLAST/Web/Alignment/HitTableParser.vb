#Region "Microsoft.VisualBasic::7b7d1b360d013f82f9063cdbb44ddd86, localblast\LocalBLAST\Web\Alignment\HitTableParser.vb"

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

'     Module HitTableParser
' 
'         Function: IterateTables, LoadTable, Mapper, parseTable
' 
' 
' /********************************************************************************/

#End Region

#If NETCOREAPP Then
Imports System.Data
#End If

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
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
                    headers += line
                Loop

                lines += line

                Do While Not String.IsNullOrEmpty(line = reader.ReadLine) AndAlso
                    line.First <> "#"c
                    lines += line
                Loop

                Yield lines.ToArray.parseTable(path$, headers, headerSplit)

                headers *= 0
                lines *= 0
                headers += line

                If String.IsNullOrEmpty(headers.First) Then
                    Do While Not reader.EndOfStream AndAlso String.IsNullOrEmpty(line = reader.ReadLine)
                    Loop
                    headers += line
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
                .parseTable(path$, header, headerSplit)
        End Function

        <Extension>
        Private Function parseTable(lines$(), path$, header$(), headerSplit As Boolean) As AlignmentTable
            Dim headAttrs As Dictionary(Of String, String) =
                header _
                .Skip(1) _
                .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                .ToDictionary(Function(x) x.Name,
                              Function(x)
                                  Return x.Value
                              End Function)
            Dim fields As Index(Of String) = headAttrs("# Fields").StringSplit("\s*,\s+")
            Dim hits As HitRecord() = lines _
                .Select(Function(line)
                            Return HitTableParser.Mapper(line, fields)
                        End Function) _
                .ToArray

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
        Private Function Mapper(s As String, fields As Index(Of String)) As HitRecord
            Dim tokens As String() = s.Split(ASCII.TAB)
            Dim i As i32 = Scan0
            Dim queryID As String = Nothing
            Dim subjectID As String = Nothing

            If "query id" Like fields AndAlso "subject ids" Like fields Then
                queryID = fields(++i)
                subjectID = fields(++i)
            End If

            Dim hit As New HitRecord With {
                .QueryAccVer = tokens(++i),
                .SubjectAccVer = tokens(++i),
                .QueryID = queryID Or .QueryAccVer.AsDefault,
                .SubjectIDs = subjectID Or .SubjectAccVer.AsDefault,
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
