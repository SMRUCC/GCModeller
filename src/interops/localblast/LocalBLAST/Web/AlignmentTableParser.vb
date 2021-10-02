#Region "Microsoft.VisualBasic::fd659703ec5cbc2a2cfa7fc03c02ed9c, localblast\LocalBLAST\Web\AlignmentTableParser.vb"

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

    '     Module AlignmentTableParser
    ' 
    '         Function: (+2 Overloads) __createFromBlastn, __hits, __hspHits, CreateFromBlastn, CreateFromBlastnFile
    '                   CreateFromBlastnFiles, CreateFromBlastX, IterateTables, LoadTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.BlastX
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

Namespace NCBIBlastResult

    Public Module AlignmentTableParser

        ''' <summary>
        ''' 当文件之中包含有多个表的时候使用
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function IterateTables(path$, headerSplit As Boolean) As IEnumerable(Of AlignmentTable)
            Return HitTableParser.IterateTables(path, headerSplit)
        End Function

        ''' <summary>
        ''' 适用于一个文件只有一个表的时候
        ''' </summary>
        ''' <param name="path">使用NCBI的web blast的结果输出表的默认格式的文件</param>
        ''' <param name="headerSplit">
        ''' 当<see cref="HitRecord.SubjectIDs"/>之中包含有多个比对结果序列的时候，是否使用分号``;``作为分隔符将表头分开？默认不分开
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadTable(path$, Optional headerSplit As Boolean = False) As AlignmentTable
            Return HitTableParser.LoadTable(path, headerSplit)
        End Function

        Private Function __createFromBlastn(sId As String, out As v228) As HitRecord()
            Dim LQuery As HitRecord() =
                LinqAPI.Exec(Of HitRecord) <= From Query As Query
                                              In out.Queries
                                              Select __createFromBlastn(sId, Query.SubjectHits)
            Return LQuery
        End Function

        <Extension>
        Private Function __createFromBlastn(sId As String, hits As SubjectHit()) As HitRecord()
            Dim LQuery As HitRecord() = LinqAPI.Exec(Of HitRecord) <=
 _
                From hspNT As SubjectHit
                In hits
                Let identity As Double = hspNT.Score.Identities.Value
                Let bits As Double = hspNT.Score.RawScore
                Let left As Integer = hspNT.QueryLocation.Left
                Let right As Integer = hspNT.QueryLocation.Right
                Select New HitRecord With {
                    .Identity = identity,
                    .DebugTag = hspNT.Name,
                    .SubjectIDs = sId,
                    .BitScore = bits,
                    .QueryStart = left,
                    .QueryEnd = right
                }

            Return LQuery
        End Function

        ''' <summary>
        ''' 从一个包含有blastn结果的文件夹之中构建出比对的结果表
        ''' </summary>
        ''' <param name="sourceDIR$"></param>
        ''' <returns></returns>
        Public Function CreateFromBlastn(sourceDIR$) As AlignmentTable
            Return (ls - l - r - "*.txt" <= sourceDIR).CreateFromBlastnFiles(sourceDIR.BaseName)
        End Function

        ''' <summary>
        ''' 从单个blastn结果输出文件之中构建出比对结果表
        ''' </summary>
        ''' <param name="file$">query vs multiple subjects</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为只是一条query比对多条序列，所以所输出的blastn结果之中只有一个query
        ''' </remarks>
        Public Function CreateFromBlastnFile(file$, Optional stripNameLength% = -1) As AlignmentTable
            Dim blastn As v228 = Parser.LoadBlastOutput(file)
            Dim query As Query = blastn.Queries.First
            Dim hits = query.SubjectHits _
                .GroupBy(Function(h) h.Name) _
                .Select(Function(g)
                            Dim name$ = g.Key

                            If stripNameLength > 0 Then
                                name = Mid(name, 1, stripNameLength)
                            End If

                            Return name.__createFromBlastn(hits:=g.ToArray)
                        End Function) _
                .ToVector
            Dim Tab As New AlignmentTable With {
                .Hits = hits,
                .RID = Now.ToShortDateString,
                .Program = "BLASTN",
                .Database = blastn.Database,
                .Query = file.BaseName
            }
            Return Tab
        End Function

        ''' <summary>
        ''' 从一组blastn数据的结果文件之中构建出比对结果表
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="database$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFromBlastnFiles(source As IEnumerable(Of String), database$) As AlignmentTable
            Dim Files = LinqAPI.Exec(Of NamedValue(Of v228)) <=
 _
                From path As String
                In source
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
                .RID = Now.ToShortDateString,
                .Program = "BLASTN",
                .Database = database,
                .Query = LinqAPI.DefaultFirst(Of String) <=
 _
                    From file As NamedValue(Of v228)
                    In Files
                    Let Q As Query() = file.Value.Queries
                    Where Not Q.IsNullOrEmpty
                    Select Q.First.QueryName
            }
            Return Tab
        End Function

        Public Function CreateFromBlastX(source As String) As AlignmentTable
            Dim Files = (From path As String
                         In ls - l - r - "*.txt" <= source
                         Select ID = path.BaseName,
                             XOutput = OutputReader.TryParseOutput(path)).ToArray
            Dim LQuery As HitRecord() = (From file In Files Select file.ID.__hits(file.XOutput)).ToVector
            Dim q$ = Files.First.XOutput.Queries.First.QueryName
            Dim tab As New AlignmentTable With {
                .Hits = LQuery,
                .Query = q,
                .RID = Now.ToShortDateString,
                .Program = "BlastX",
                .Database = source
            }
            Return tab
        End Function

        <Extension> Private Function __hits(id As String, out As v228_BlastX) As IEnumerable(Of HitRecord)
            Return out.Queries _
                .Select(Function(query) id.__hspHits(query)) _
                .IteratesALL _
                .IteratesALL
        End Function

        <Extension>
        Private Function __hspHits(id$, query As BlastX.Components.Query) As IEnumerable(Of IEnumerable(Of HitRecord))
            Return From subject As BlastX.Components.Subject
                   In query.Subjects
                   Select From hsp As BlastX.Components.HitFragment
                          In subject.Hits
                          Let row As HitRecord = New HitRecord With {
                              .Identity = hsp.Score.Identities.Value,
                              .DebugTag = subject.SubjectName,
                              .SubjectIDs = id,
                              .BitScore = hsp.Score.RawScore,
                              .QueryStart = hsp.Hsp.First.Query.Left,
                              .QueryEnd = hsp.Hsp.Last.Query.Right
                          }
                          Select row
        End Function
    End Module
End Namespace
