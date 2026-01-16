#Region "Microsoft.VisualBasic::6d3e8131ddf2ac1e3309babf3679e089, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\OutputReader.vb"

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

    '   Total Lines: 248
    '    Code Lines: 180 (72.58%)
    ' Comment Lines: 27 (10.89%)
    '    - Xml Docs: 62.96%
    ' 
    '   Blank Lines: 41 (16.53%)
    '     File Size: 9.46 KB


    '     Module OutputReader
    ' 
    '         Function: __parser, (+2 Overloads) __queryParser, hspParser, parseFragment, parseHitFragments
    '                   QueryBlockIterator, queryInfo, subjectInfo, subjectParser, TryParseOutput
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports r = System.Text.RegularExpressions.Regex
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    Public Module OutputReader

        Const SECTION_REGEX As String = "Query= .+?Length=\d+.+?Effective search space used: \d+"
        Const HSP_REGEX As String = "^Query.+?$^Sbjct.+?$"

        ''' <summary>
        ''' Try load the blastx output file data.(尝试使用这个方法来加载blastx的输出数据)
        ''' </summary>
        ''' <param name="path">The file path of the blastx output file.(blastx输出文件的文件路径)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParseOutput(path$, Optional UncharacterizedExclude As Boolean = False, Optional encoding As Encoding = Nothing) As v228_BlastX
            Dim LQuery As Components.Query() = path _
                .QueryBlockIterator(encoding:=encoding Or UTF8) _
                .Select(Function(block) __queryParser(block, UncharacterizedExclude)) _
                .ToArray

            Return New v228_BlastX With {
                .FilePath = path & ".xml",
                .Queries = LQuery,
                .Database = path.BaseName
            }
        End Function

        <Extension> Public Iterator Function QueryBlockIterator(path$, encoding As Encoding) As IEnumerable(Of String)
            Dim skip As Boolean = True
            Dim buffer As New List(Of String)

            For Each line As String In path.IterateAllLines
                If InStr(line, "Query=", CompareMethod.Binary) = 1 Then
                    ' 新的block数据块
                    ' 则需要将前面的buffer数据抛出去
                    If Not skip Then
                        Yield buffer.JoinBy(ASCII.LF)
                    End If

                    buffer *= 0
                    buffer += line
                    skip = False
                Else
                    If Not skip Then
                        buffer += line
                    End If
                End If
            Next

            Yield buffer.JoinBy(ASCII.LF)
        End Function

        Const subjectInfoRegexp$ = ".+?Length=\d+"

        Friend Function subjectInfo(block$) As NamedValue(Of Integer)
            Dim info$ = r.Match(block, subjectInfoRegexp, RegexICSng) _
                .Value _
                .TrimNewLine
            Dim tuple = Strings.Split(info, "Length=")
            Dim name$ = tuple(0)

            Return New NamedValue(Of Integer) With {
                .Name = name,
                .Value = tuple(1)
            }
        End Function

        <Extension> Private Function subjectParser(subject$) As Components.Subject
            Dim info As NamedValue(Of Integer) = subjectInfo(subject)
            Dim fragments = parseHitFragments(subject, info)

            Return New Components.Subject With {
                .SubjectName = info.Name,
                .SubjectLength = info.Value,
                .Hits = fragments
            }
        End Function

        <Extension> Friend Function parseFragment(block$, scores$, pos%) As String
            Dim start% = InStr(pos, block, scores)
            Dim end% = InStr(start + 5, block, " Score =")

            block = Mid(block, start)

            If [end] > 0 Then
                block = Mid(block, 1, [end] - start)
            End If

            block = block _
                .LineTokens _
                .Skip(3) _
                .Where(Function(s) Not s.StringEmpty) _
                .JoinBy(ASCII.LF) _
                .Trim

            Return block
        End Function

        Private Function parseHitFragments(block$, subjectInfo As NamedValue(Of Integer)) As List(Of Components.HitFragment)
            Dim tmp As New List(Of Components.HitFragment)
            Dim HSP$() = r _
                .Matches(block, BlastXScore.REGEX_BLASTX_SCORE, RegexICSng) _
                .ToArray
            Dim pos% = 1
            Dim hspRegion$

            For Each score As String In HSP
                hspRegion = parseFragment(block, score, pos)
                tmp += hspParser(hspRegion, score)
                pos += score.Length
            Next

            For Each x In tmp
                x.HitLen = subjectInfo.Value
                x.HitName = subjectInfo.Name
            Next

            Return tmp
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' ###### 20190615
        ''' 当queryName是空的时候，这个正则表达式将无法正常工作
        ''' </remarks>
        Const queryInfoRegexp$ = "Query\s*[=]\s*(.+?)?Length\s*[=]\s*\d+"

        <Extension>
        Friend Function queryInfo(block$) As NamedValue(Of Integer)
            If block = "" Then
                Return Nothing
            Else
                Dim info$ = r.Match(block, queryInfoRegexp, RegexICSng).Value.TrimNewLine
                Dim tuple = Strings.Split(info, "Length=")
                Dim name$ = tuple(0).GetTagValue("=", trim:=True).Value

                Return New NamedValue(Of Integer) With {
                    .Name = name,
                    .Value = Val(tuple(1).Trim.Split.First)
                }
            End If
        End Function

        <Extension>
        Private Function __queryParser(block$, queryInfo As NamedValue(Of Integer), UncharacterizedExclude As Boolean) As Components.Query
            Dim bufs As New List(Of Components.Subject)
            Dim parts$() = r _
                .Split(block, "^>", RegexOptions.Multiline) _
                .Skip(1) _
                .ToArray

            'If queryInfo.Name = "TRINITY_DN15819_c0_g2 Trans_ID=TRINITY_DN15819_c0_g2_i4" Then
            '    Console.WriteLine()
            'End If

            For Each subject As String In parts
                bufs += subject _
                    .Trim _
                    .subjectParser()
            Next

            ' 只导出最好的第一条比对结果？
            If UncharacterizedExclude Then
                Dim tmp = bufs _
                    .Where(Function(sbj)
                               Return InStr(sbj.SubjectName, "Uncharacterized", CompareMethod.Text) = 0 AndAlso
                                      InStr(sbj.SubjectName, "Uncharacter", CompareMethod.Text) = 0 AndAlso
                                      InStr(sbj.SubjectName, "Unknow", CompareMethod.Text) = 0
                           End Function) _
                    .AsList

                If tmp = 0 Then
                    ' 说明所有的hits蛋白都是未鉴定的，则没有办法只能够返回原始结果
                    ' 则在这里不做任何修改
                    Call cat("~")
                Else
                    ' tmp是已经有功能鉴定的蛋白结果，替换掉bufs原先的值
                    bufs = tmp
                End If
            End If

            Return New Components.Query With {
                .QueryLength = queryInfo.Value,
                .QueryName = queryInfo.Name,
                .Subjects = bufs
            }
        End Function

        ''' <summary>
        ''' 一个query对应多个hits，每一个hit与query之间又会存在多个比对的高分区碎片
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        Private Function __queryParser(str$, UncharacterizedExclude As Boolean) As Components.Query
            Dim queryInfo = str.queryInfo

            If InStr(str, "***** No hits found *****") Then
                Return New Components.Query With {
                    .QueryName = queryInfo.Name,
                    .QueryLength = queryInfo.Value
                }
            Else
                Return r _
                    .Split(str, "^Lambda\s+", RegexICMul) _
                    .First _
                    .Trim _
                    .__queryParser(queryInfo, UncharacterizedExclude)
            End If
        End Function

        Private Function __parser(ByRef p As Integer, Match As String, Tokens As String()) As String
            Dim Temp As String = ""

            Do While p < Tokens.Length - 1
                Dim sssss As String = Tokens(p)

                If InStr(sssss, Match) = 1 Then
                    Return Temp
                Else
                    Temp &= " " & sssss
                End If

                p += 1
            Loop

            Return Temp
        End Function

        Private Function hspParser(s As String, Score As String) As Components.HitFragment
            Dim hsp = s.LineTokens.Split(3)
            Dim LQuery As HitSegment() = hsp _
                .Select(Function(x) HitSegment.TryParse(x)) _
                .ToArray

            Return New Components.HitFragment With {
                .Score = BlastXScore.ParseText(Score),
                .Hsp = LQuery
            }
        End Function
    End Module
End Namespace
