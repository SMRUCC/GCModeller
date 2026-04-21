#Region "Microsoft.VisualBasic::3f7c4d41de26ed2c77947e5cd624441e, localblast\LocalBLAST\Web\Alignment\Extensions.vb"

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

'   Total Lines: 53
'    Code Lines: 39 (73.58%)
' Comment Lines: 6 (11.32%)
'    - Xml Docs: 83.33%
' 
'   Blank Lines: 8 (15.09%)
'     File Size: 2.08 KB


'     Module Extensions
' 
'         Function: ExportOrderByGI, GetHitsEntryList, TopBest
' 
' 
' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports r = System.Text.RegularExpressions.Regex

Namespace NCBIBlastResult.WebBlast

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 导出绘制的顺序
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>这里不能够使用并行拓展</remarks>
        ''' 
        <Extension>
        Public Function ExportOrderByGI(table As AlignmentTable) As String()
            Dim LQuery As String() = (From hit As HitRecord
                                      In table.Hits
                                      Select hit.GI.FirstOrDefault
                                      Distinct).ToArray
            Return LQuery
        End Function

        Const LOCUS_ID As String = "(emb|gb|dbj)\|[a-z]+\d+"

        <Extension>
        Public Function GetHitsEntryList(table As AlignmentTable) As String()
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From hit As HitRecord
                                           In table.Hits
                                           Let hitID As String = r.Match(hit.SubjectIDs, LOCUS_ID, RegexICSng).Value
                                           Where Not String.IsNullOrEmpty(hitID)
                                           Select hitID.Split(CChar("|")).Last
                                           Distinct
            Return LQuery
        End Function

        <Extension>
        Public Iterator Function TopBest(raw As IEnumerable(Of HitRecord)) As IEnumerable(Of HitRecord)
            Dim gg = From x As HitRecord In raw Select x Group x By x.QueryID Into Group

            For Each groups In gg
                Dim orders = From x As HitRecord
                             In groups.Group
                             Select x
                             Order By x.Identity Descending

                Yield orders.First
            Next
        End Function

        ''' <summary>
        ''' 解析 BLASTN 输出的 TSV 文件为 BlastResult 对象集合
        ''' </summary>
        ''' <returns>BlastResult 对象的列表</returns>
        Public Iterator Function ParseBlastTsvFile(file As Stream) As IEnumerable(Of HitRecord)
            ' 使用 File.ReadLines 逐行读取，避免大文件占用过多内存
            For Each line As String In file.ReadAllLines
                ' 跳过空行
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                ' 按制表符分割列
                Dim columns() As String = line.Split(ControlChars.Tab)

                ' 标准 outfmt 6 应该正好有 12 列，如果不等于12说明格式可能被破坏或存在表头
                If columns.Length <> 12 Then Continue For

                Dim result As New HitRecord With {
                    .QueryID = columns(0).Trim(),
                    .SubjectIDs = columns(1).Trim(),
                    .Identity = Double.Parse(columns(2).Trim(), CultureInfo.InvariantCulture),
                    .AlignmentLength = Integer.Parse(columns(3).Trim()),
                    .MisMatches = Integer.Parse(columns(4).Trim()),
                    .GapOpens = Integer.Parse(columns(5).Trim()),
                    .QueryStart = Integer.Parse(columns(6).Trim()),
                    .QueryEnd = Integer.Parse(columns(7).Trim()),
                    .SubjectStart = Integer.Parse(columns(8).Trim()),
                    .SubjectEnd = Integer.Parse(columns(9).Trim()),
                    .EValue = Double.Parse(columns(10).Trim(), CultureInfo.InvariantCulture),
                    .BitScore = Double.Parse(columns(11).Trim(), CultureInfo.InvariantCulture),
                    .QueryAccVer = .QueryID,
                    .SubjectAccVer = .SubjectIDs
                }

                Yield result
            Next
        End Function
    End Module
End Namespace
