Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Module BlastpOutputReader

        Public Iterator Function QueryBlockIterates(path$, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of String)
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

        ''' <summary>
        ''' Calls the blastp parser for the blast+ alignment result.
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Iterator Function RunParser(path$, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of Query)
            Dim source As IEnumerable(Of String) = QueryBlockIterates(path, encoding)

            For Each queryText As String In source
                Yield QueryParser(queryText)
            Next
        End Function

        Private Function QueryParser(block As String) As Query
            Dim queryInfo As NamedValue(Of Integer) = BlastX.OutputReader.queryInfo(block)

            If InStr(block, "***** No hits found *****") Then
                Return New Query With {
                    .QueryName = queryInfo.Name,
                    .QueryLength = queryInfo.Value
                }
            Else
                Return r _
                    .Split(block, "^Lambda\s+", RegexICMul) _
                    .First _
                    .Trim _
                    .__queryParser(queryInfo, top:=False)
            End If
        End Function

        <Extension>
        Private Function __queryParser(block$, queryInfo As NamedValue(Of Integer), top As Boolean) As Query
            Dim bufs As New List(Of SubjectHit)
            Dim parts$() = r _
                .Split(block, "^>", RegexOptions.Multiline) _
                .Skip(1) _
                .ToArray

            For Each subject As String In parts
                bufs += subject _
                    .Trim _
                    .subjectParser()

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

        <Extension>
        Private Function subjectParser(block$) As SubjectHit
            Dim subjectInfo As NamedValue(Of Integer) = BlastX.OutputReader.subjectInfo(block)
            Dim tmp As New List(Of HitSegment)
            Dim scoreText$ = r _
                .Match(block, REGEX_BLASTP_SCORE, RegexICSng) _
                .Value
            Dim score As Score = Score.TryParse(scoreText)
            Dim hsp = Strings.Split(block, scoreText) _
                .Last _
                .lTokens _
                .Where(Function(s) Not s.StringEmpty) _
                .CreateSlideWindows(slideWindowSize:=3, offset:=3) _
                .ToArray
            Dim segments = hsp _
                .Select(Function(h) HitSegment.TryParse(h.ToArray)) _
                .ToArray

            Return New SubjectHit With {
                .Hsp = segments,
                .Length = subjectInfo.Value,
                .Name = subjectInfo.Name,
                .Score = score
            }
        End Function
    End Module
End Namespace