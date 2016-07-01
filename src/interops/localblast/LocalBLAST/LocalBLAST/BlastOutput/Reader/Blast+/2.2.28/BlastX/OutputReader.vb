Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    Public Module OutputReader

        Const SECTION_REGEX As String = "Query= .+?Length=\d+.+?Effective search space used: \d+"
        Const HSP_REGEX As String = "^Query.+?$^Sbjct.+?$"

        ''' <summary>
        ''' Try load the blastx output file data.(尝试使用这个方法来加载blastx的输出数据)
        ''' </summary>
        ''' <param name="Path">The file path of the blastx output file.(blastx输出文件的文件路径)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParseOutput(Path As String) As v228_BlastX
            Dim SourceText As String = FileIO.FileSystem.ReadAllText(Path)
            Dim Sections As String() = (From m As Match In Regex.Matches(SourceText, SECTION_REGEX, RegexOptions.Singleline) Select m.Value).ToArray
            Dim LQuery As Components.Query()
            '  Try
            LQuery = (From str As String In Sections Select __queryParser(str)).ToArray
            '  Catch ex As Exception
            '  ex = New Exception(Path.ToFileURL, ex)
            '  Throw ex
            '  End Try
            Return New v228_BlastX With {
                .FilePath = Path & ".xml",
                .Queries = LQuery,
                .Database = IO.Path.GetFileNameWithoutExtension(Path)
            }
        End Function

        Private Function __hitFragments(sec As String) As List(Of Components.HitFragment)
            Dim HSP As String() = (From m As Match
                                      In Regex.Matches(sec, BlastXScore.REGEX_BLASTX_SCORE, RegexOptions.Singleline)
                                   Select m.Value).ToArray

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
            Call tmp.Add(__hspParser(last, HSP.Last))

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

        Private Function __queryParser(str As String) As Components.Query
            Dim ChunkBuffer As New List(Of Components.HitFragment)

            If InStr(str, "***** No hits found *****") Then
                GoTo ENTRY_INFO_PARSER
            End If

            Dim names As String() = Regex.Split(str, "^>", RegexOptions.Multiline).Skip(1).ToArray

            For i As Integer = 0 To names.Length - 2
                Dim sec As String = names(i)
                ChunkBuffer += __hitFragments(sec).ToArray
            Next

            Dim last As String = names.Last
            last = Regex.Split(last, "Lambda\s+K", RegexOptions.Singleline).First

            ChunkBuffer += __hitFragments(last).ToArray

ENTRY_INFO_PARSER:

            Dim Tokens As String() = (From s As String
                                      In Strings.Split(str.Replace(vbCr, ""), vbLf)
                                      Where Not String.IsNullOrEmpty(s)
                                      Select s).ToArray
            Dim p As Integer
            Dim QueryName As String = __parser(p, "Length=", Tokens).Trim
            Dim QueryLength As String = Tokens(p.MoveNext).Trim
            Dim SubjectName As String = __parser(p, "Length=", Tokens).Trim
            Dim SubjectLength As String = Tokens(p.MoveNext).Trim

            QueryName = Mid(QueryName, 7).Trim
            SubjectName = Mid(SubjectName, 9).Trim
            QueryLength = Regex.Match(QueryLength, "\d+").Value
            SubjectLength = Regex.Match(SubjectLength, "\d+").Value

            Return New Components.Query With {
                .Hits = ChunkBuffer.ToArray,
                .QueryName = QueryName,
                .QueryLength = Val(QueryLength),
                .SubjectLength = Val(SubjectLength),
                .SubjectName = SubjectName
            }
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
            Dim Hsp = (From sss As String In s.lTokens
                       Where Not String.IsNullOrEmpty(sss)
                       Select sss).ToArray.CreateSlideWindows(3, offset:=3)
            Dim LQuery = (From chunk In Hsp Select BLASTOutput.ComponentModel.HitSegment.TryParse(chunk.Elements)).ToArray
            Return New Components.HitFragment With {
                .Score = __scoreParser(Score),
                .Hsp = LQuery
            }
        End Function

        Private Function __scoreParser(s As String) As BLASTOutput.ComponentModel.BlastXScore
            Dim Tokens As String() = (From str As String In Strings.Split(s.Replace(vbCr, ""), vbLf) Where Not String.IsNullOrEmpty(str) Select str).ToArray
            Dim Score As BLASTOutput.ComponentModel.BlastXScore = ComponentModel.BlastXScore.TryParse(Of BLASTOutput.ComponentModel.BlastXScore)(s)
            Score.Frame = Val(Tokens.Last.Replace("Frame =", "").Trim)

            Return Score
        End Function
    End Module
End Namespace
