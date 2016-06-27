Imports System.Linq
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ListExtensions

Public Class MotifScanner : Inherits IScanner

    Sub New(nt As I_PolymerSequenceModel)
        Call MyBase.New(nt)
    End Sub

    Public Overrides Function Scan(pattern As String) As SimpleSegment()
        Return (Scan(__nt, pattern, AddressOf Equals).ToList + Scan(__nt, Complement(pattern), AddressOf Equals)).OrderBy(Function(x) x.Start).ToArray
    End Function

    ReadOnly __rand As Random = New Random

    Public Overloads Function Equals(pattern As String, residue As String) As Integer
        Dim r As Char = residue.FirstOrDefault(NIL)

        If pattern.Length = 1 Then
            Dim p As Char = pattern.FirstOrDefault(NIL)
            If p = "."c OrElse p = "N"c Then
                Return 10
            End If
            If Char.IsUpper(p) Then
                If p = r Then
                    Return 10
                Else
                    Return -10
                End If
            Else  ' 小写的，有一定的概率是别的字符
                If p = Char.ToLower(r) Then
                    Return 5
                Else
                    If __rand.NextDouble < 0.75 Then  '  例如a大多数情况下是A
                        Return -5
                    Else
                        Return 5
                    End If
                End If
            End If
        Else
            ' []匹配
            For Each c As Char In pattern
                If c = r Then
                    Return 10
                End If
            Next

            Return -10
        End If
    End Function

    Public Overloads Shared Function Scan(nt As String, pattern As String, equals As ISimilarity(Of String)) As SimpleSegment()
        Dim words As String() = Patterns.SimpleTokens(pattern)
        Dim subject As String() = nt.ToArray(Function(c) CStr(c))
        Dim GSW As New GSW(Of String)(words, subject, equals, AddressOf ToChar)
        Dim out As Output = GetOutput(GSW, 0, (2 / 3) * words.Length)
        Return (From x In out.HSP Select New SimpleSegment With {.SequenceData = x.Subject, .Start = x.FromB, .Ends = x.ToB}).ToArray
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="cutoff">0%-100%</param>
    ''' <returns></returns>
    Public Shared Function GetOutput(this As GSW(Of String), cutoff As Double, minW As Integer) As Output
        Return Output.CreateObject(this, Function(x) x, cutoff, minW)
    End Function

    Public Shared Function ToChar(s As String) As Char
        If s.Length = 1 Then
            Dim c As Char = s.FirstOrDefault(NIL)
            If c = "." Then
                Return "N"c
            Else
                Return c
            End If
        Else
            Dim c As Char = s.FirstOrDefault(NIL)
            Return Char.ToLower(c)
        End If
    End Function
End Class
