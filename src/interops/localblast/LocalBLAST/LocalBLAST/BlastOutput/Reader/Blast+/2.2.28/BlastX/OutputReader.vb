#Region "Microsoft.VisualBasic::db2f43a7d455f25bef5a3331f1496347, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\BlastX\OutputReader.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

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
        Public Function TryParseOutput(path$, Optional encoding As Encoding = Nothing) As v228_BlastX
            Dim LQuery As Components.Query() = path _
                .QueryBlockIterator(encoding:=encoding Or UTF8) _
                .Select(AddressOf __queryParser) _
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

        Private Function subjectParser(subject$) As Components.Subject

        End Function

        Private Function __hitFragments(sec As String) As List(Of Components.HitFragment)
            Dim HSP As String() = Regex.Matches(sec, BlastXScore.REGEX_BLASTX_SCORE, RegexOptions.Singleline).ToArray

            Dim tmp As New List(Of Components.HitFragment)

            For j As Integer = 0 To HSP.Length - 2
                Dim s As String = HSP(j)
                Dim pp As Integer = InStr(sec, s)
                Dim pNext As Integer = InStr(sec, HSP(j + 1))
                Dim lennn As Integer = pNext - pp
                Dim s1 As String = Mid(sec, pp, lennn)
                Dim sss As String = Regex.Split(s1.Replace(s, ""), "^>", RegexOptions.Multiline).First

                tmp += __hspParser(sss, s)
            Next

            Dim pLast As Integer = InStr(sec, HSP.Last)
            Dim last As String = Regex.Split(Mid(sec, pLast), "Lambda\s+K").First.Replace(HSP.Last, "")

            tmp += __hspParser(last, HSP.Last)

            Dim name As String = Strings.Split(sec, " Score =", Compare:=CompareMethod.Binary).First
            Dim len As String = Regex.Match(name, "Length\s*=\s*\d+", RegexOptions.Singleline).Value
            name = name.Replace(len, "").Trim
            name = name.lTokens.JoinBy(" ")
            len = len.Split("="c).Last.Trim

            Dim l As Integer = Scripting.CTypeDynamic(Of Integer)(len)

            For Each x In tmp
                x.HitLen = l
                x.HitName = name
            Next

            Return tmp
        End Function

        Const queryInfoRegexp$ = "Query=\s*.+?Length=\d+"

        <Extension> Private Function queryInfo(block$) As NamedValue(Of Integer)
            Dim info$ = r.Match(block, queryInfoRegexp, RegexICSng).Value.TrimNewLine
            Dim tuple = Strings.Split(info, "Length=")
            Dim name$ = tuple(0).GetTagValue("=", trim:=True).Value

            Return New NamedValue(Of Integer) With {
                .Name = name,
                .Value = tuple(1)
            }
        End Function

        <Extension>
        Private Function __queryParser(block$, queryInfo As NamedValue(Of Integer)) As Components.Query
            Dim bufs As New List(Of Components.Subject)
            Dim parts$() = r _
                .Split(block, "^>", RegexOptions.Multiline) _
                .Skip(1) _
                .ToArray

            For Each subject As String In parts
                bufs += subjectParser(subject)
            Next

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
        Private Function __queryParser(str$) As Components.Query
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
                    .__queryParser(queryInfo)
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

        Private Function __hspParser(s As String, Score As String) As Components.HitFragment
            Dim hsp = s.lTokens _
                .Where(Function(ss) Not String.IsNullOrEmpty(ss)) _
                .CreateSlideWindows(3, offset:=3)
            Dim LQuery As HitSegment() =
                hsp.Select(Function(x) HitSegment.TryParse(x.Items)).ToArray

            Return New Components.HitFragment With {
                .Score = __scoreParser(Score),
                .Hsp = LQuery
            }
        End Function

        Private Function __scoreParser(s As String) As BlastXScore
            Dim Tokens As String() = LinqAPI.Exec(Of String) <=
 _
                From str As String
                In Strings.Split(s.Replace(vbCr, ""), vbLf)
                Where Not String.IsNullOrEmpty(str)
                Select str

            Dim Score As BlastXScore = BlastXScore.TryParse(Of BlastXScore)(s)
            Score.Frame = Val(Tokens.Last.Replace("Frame =", "").Trim)

            Return Score
        End Function
    End Module
End Namespace
