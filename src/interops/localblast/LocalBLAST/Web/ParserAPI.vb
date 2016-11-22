#Region "Microsoft.VisualBasic::364ae502db9c5f39a238dcb78dc847ce, ..\interops\localblast\LocalBLAST\Web\ParserAPI.vb"

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
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.BlastX

Namespace NCBIBlastResult

    ''' <summary>
    ''' 从NCBI网站之中下载的比对结果的表格文本文件之中进行数据的解析操作
    ''' </summary>
    Public Module ParserAPI

        ''' <summary>
        ''' 适用于一个文件只有一个表的时候
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function LoadDocument(path As String, Optional headerSplit As Boolean = False) As AlignmentTable
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

            Return lines _
                .Skip(header.Length) _
                .ToArray _
                .__parseTable(path$, header, headerSplit)
        End Function

        <Extension>
        Private Function __parseTable(lines$(), path$, header$(), headerSplit As Boolean) As AlignmentTable
            Dim hits As HitRecord() = lines _
                .ToArray(AddressOf HitRecord.Mapper)
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
                .FilePath = path,
                .Program = header.First.Trim.Split.Last,
                .Query = headAttrs("# Query"),
                .Database = headAttrs("# Database"),
                .RID = headAttrs("# RID")
            }
        End Function

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

        Private Function __createFromBlastn(sId As String, out As v228) As HitRecord()
            Dim LQuery As HitRecord() =
                LinqAPI.Exec(Of HitRecord) <= From Query As Query
                                              In out.Queries
                                              Select __createFromBlastn(sId, Query.SubjectHits)
            Return LQuery
        End Function

        Private Function __createFromBlastn(sId As String, hits As SubjectHit()) As HitRecord()
            Dim LQuery As HitRecord() = LinqAPI.Exec(Of HitRecord) <=
 _
                From hspNT As SubjectHit
                In hits
                Let row As HitRecord = New HitRecord With {
                    .Identity = hspNT.Score.Identities.Value,
                    .DebugTag = hspNT.Name,
                    .SubjectIDs = sId,
                    .BitScore = hspNT.Score.RawScore,
                    .QueryStart = hspNT.QueryLocation.Left,
                    .QueryEnd = hspNT.QueryLocation.Right
                }
                Select row

            Return LQuery
        End Function

        Public Function CreateFromBlastn(sourceDIR As String) As AlignmentTable
            Dim Files = LinqAPI.Exec(Of NamedValue(Of v228)) <=
 _
                From path As String
                In ls - l - r - "*.txt" <= sourceDIR
                Let XOutput As v228 = Parser.LoadBlastOutput(path)
                Where Not XOutput Is Nothing AndAlso
                    Not XOutput.Queries.IsNullOrEmpty
                Select New NamedValue(Of v228) With {
                    .Name = path.BaseName,
                    .Value = XOutput
                }

            Dim LQuery As HitRecord() = (From file In Files Select __createFromBlastn(file.Name, file.Value)).ToVector
            Dim Tab As New AlignmentTable With {
                .Hits = LQuery,
                .Query = (From file As NamedValue(Of v228)
                          In Files
                          Let Q As Query() = file.Value.Queries
                          Where Not Q.IsNullOrEmpty
                          Select Q.First.QueryName).FirstOrDefault,
                .RID = Now.ToShortDateString,
                .Program = "BLASTN",
                .Database = sourceDIR
            }
            Return Tab
        End Function

        Public Function CreateFromBlastX(source As String) As AlignmentTable
            Dim Files = (From path As String
                         In ls - l - r - wildcards("*.txt") <= source
                         Select ID = path.BaseName,
                             XOutput = OutputReader.TryParseOutput(path)).ToArray
            Dim LQuery As HitRecord() = (From file In Files Select file.ID.__hits(file.XOutput)).ToVector
            Dim tab As New AlignmentTable With {
                .Hits = LQuery,
                .Query = Files.First.XOutput.Queries.First.QueryName,
                .RID = Now.ToShortDateString,
                .Program = "BlastX",
                .Database = source
            }
            Return tab
        End Function

        <Extension> Private Function __hits(id As String, out As v228_BlastX) As IEnumerable(Of HitRecord)
            Return out.Queries _
                .Select(Function(query) id.__hspHits(query)) _
                .IteratesALL
        End Function

        <Extension>
        Private Function __hspHits(id$, query As BlastX.Components.Query) As IEnumerable(Of HitRecord)
            Return From hsp As BlastX.Components.HitFragment
                   In query.Hits
                   Let row As HitRecord = New HitRecord With {
                       .Identity = hsp.Score.Identities.Value,
                       .DebugTag = query.SubjectName,
                       .SubjectIDs = id,
                       .BitScore = hsp.Score.RawScore,
                       .QueryStart = hsp.Hsp.First.Query.Left,
                       .QueryEnd = hsp.Hsp.Last.Query.Right
                   }
                   Select row
        End Function
    End Module
End Namespace
