#Region "Microsoft.VisualBasic::62d65cb77b8d7cd14621916bec3d0f4a, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\blastp\BlastpOutputReader.vb"

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

    '   Total Lines: 161
    '    Code Lines: 112 (69.57%)
    ' Comment Lines: 25 (15.53%)
    '    - Xml Docs: 68.00%
    ' 
    '   Blank Lines: 24 (14.91%)
    '     File Size: 6.13 KB


    '     Module BlastpOutputReader
    ' 
    '         Function: HspParserInternal, internalQueryParser, QueryBlockIterates, QueryParser, RunParser
    '                   subjectParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Module BlastpOutputReader

        ''' <summary>
        ''' 每次只生成一个query的文本内容数据
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Iterator Function QueryBlockIterates(path$, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of String)
            Dim skip As Boolean = True
            Dim buffer As New List(Of String)

            For Each line As String In path.IterateAllLines(tqdm_wrap:=App.EnableTqdm)
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

        ''' <summary>
        ''' Calls the blastp parser for the blast+ alignment result.
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Iterator Function RunParser(path$, Optional fast As Boolean = True, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of Query)
            Dim source As IEnumerable(Of String) = QueryBlockIterates(path, encoding)

            For Each queryText As String In source
                If queryText <> "" Then
                    Yield queryText.QueryParser(fast)
                End If
            Next
        End Function

        <Extension>
        Private Function QueryParser(block As String, fast As Boolean) As Query
            Dim queryInfo As NamedValue(Of Integer) = BlastX.OutputReader.queryInfo(block)

            If InStr(block, "***** No hits found *****") Then
                Return New Query With {
                    .QueryName = queryInfo.Name,
                    .QueryLength = queryInfo.Value
                }
            Else
                ' 20191111
                ' due to the reason of word lambda may exists in bacteria species name
                ' so that this regex match should be case sensitive.
                Dim blocks = r.Split(block, "^Lambda\s{2,}", RegexOptions.Multiline)
                Dim alignDetails = blocks(Scan0).Trim

                Return alignDetails.internalQueryParser(queryInfo, top:=False, fast:=fast)
            End If
        End Function

        <Extension>
        Private Function internalQueryParser(block$, queryInfo As NamedValue(Of Integer), top As Boolean, fast As Boolean) As Query
            Dim bufs As New List(Of SubjectHit)
            Dim parts$() = r _
                .Split(block, "^>", RegexOptions.Multiline) _
                .Skip(1) _
                .ToArray

            For Each subject As String In parts
                bufs += subject _
                    .Trim _
                    .subjectParser(fast)

                ' 只导出最好的第一条比对结果？
                If top Then
                    Exit For
                End If
            Next

            Return New Query With {
                .QueryLength = queryInfo.Value,
                .QueryName = queryInfo.Name,
                .SubjectHits = bufs
            }
        End Function

        Public Const REGEX_BLASTP_SCORE$ = "Score\s*[=]\s*\d+.+?Gaps\s*[=]\s*\d+/\d+\s*\(.+?\)"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="block"></param>
        ''' <param name="fast">fast模式下,将不会解析匹配的具体序列匹配内容,只解析出query,subject和具体的得分</param>
        ''' <returns></returns>
        <Extension>
        Private Function subjectParser(block$, fast As Boolean) As SubjectHit
            Dim subjectInfo As NamedValue(Of Integer) = BlastX.OutputReader.subjectInfo(block)
            Dim tmp As New List(Of FragmentHit)
            Dim HSP$() = r _
                .Matches(block, REGEX_BLASTP_SCORE, RegexICSng) _
                .ToArray
            Dim pos% = 1
            Dim hspRegion$

            For Each score As String In HSP
                hspRegion = BlastX.parseFragment(block, score, pos)
                tmp += HspParserInternal(hspRegion, score, fast)
                pos += score.Length
            Next

            For Each x As FragmentHit In tmp
                x.HitLength = subjectInfo.Value
                x.HitName = subjectInfo.Name
            Next

            Dim qscore As Score

            If tmp.Count = 1 Then
                qscore = tmp(0).Score
            ElseIf tmp.Count > 1 Then
                qscore = New Score With {
                    .Score = tmp.Average(Function(a) a.Score.Score),
                    .Expect = tmp.Average(Function(a) a.Score.Expect),
                    .Method = tmp.First.Score.Method,
                    .RawScore = tmp.Average(Function(a) a.Score.RawScore),
                    .Gaps = New Percentage() With {
                        .Denominator = 1,
                        .Numerator = tmp.Average(Function(a) CDbl(a.Score.Gaps))
                    },
                    .Identities = New Percentage() With {
                        .Denominator = 1,
                        .Numerator = tmp.Average(Function(a) CDbl(a.Score.Identities))
                    },
                    .Positives = New Percentage With {
                        .Denominator = 1,
                        .Numerator = tmp.Average(Function(a) CDbl(a.Score.Positives))
                    }
                }
            Else
                qscore = Nothing
            End If

            Return New BlastpSubjectHit With {
                .Length = subjectInfo.Value,
                .Name = subjectInfo.Name,
                .FragmentHits = tmp,
                .Score = qscore
            }
        End Function

        Private Function HspParserInternal(s$, scoreText$, fast As Boolean) As FragmentHit
            Dim LQuery As HitSegment()

            If fast Then
                LQuery = {}
            Else
                Dim hsp = s.LineTokens.Split(3)

                ' parsing sequence aligmment details.
                LQuery = hsp _
                    .Select(Function(x) HitSegment.TryParse(x)) _
                    .ToArray
            End If

            Return New FragmentHit With {
                .Score = Score.TryParse(scoreText),
                .Hsp = LQuery
            }
        End Function
    End Module
End Namespace
