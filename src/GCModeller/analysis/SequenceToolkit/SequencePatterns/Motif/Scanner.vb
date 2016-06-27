Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif.Patterns
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Namespace Motif

    Public MustInherit Class IScanner

        Protected ReadOnly __nt As String

        Sub New(nt As I_PolymerSequenceModel)
            __nt = nt.SequenceData.ToUpper
        End Sub

        Public Overrides Function ToString() As String
            Return $"{__nt.Length}bp...."
        End Function

        Public MustOverride Function Scan(pattern As String) As SimpleSegment()

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        <ExportAPI("Loci.Find.Location", Info:="Found out all of the loci site on the target sequence.")>
        Public Shared Function FindLocation(Sequence As String, Loci As String) As Integer()
            Dim locis As New List(Of Integer)
            Dim p As Integer = 1

            Do While True
                p = InStr(Start:=p, String1:=Sequence, String2:=Loci)
                If p > 0 Then
                    locis += p
                    p += 1
                Else
                    Exit Do
                End If
            Loop

            Return locis.ToArray
        End Function

        Public Shared Function Complement(pattern As String) As String
            Dim tokens As String() = PatternParser.SimpleTokens(pattern)
            For i As Integer = 0 To tokens.Length - 1
                Dim s As String = tokens(i)
                If s.Length = 1 Then
                    s = CStr(__complement(s.First))
                Else
                    s = s.GetStackValue("[", "]")
                    Dim temp As New List(Of Char)

                    For Each c As Char In s
                        temp += __complement(c)
                    Next

                    s = New String("["c + temp + "]"c)
                End If

                tokens(i) = s
            Next

            tokens = tokens.Reverse.ToArray
            pattern = String.Join("", tokens)

            Return pattern
        End Function

        Private Shared Function __complement(c As Char) As Char
            Select Case c
                Case "A"c
                    Return "T"c
                Case "G"c
                    Return "C"c
                Case "C"c
                    Return "G"c
                Case "T"c
                    Return "A"c
                Case "N"c, "."c
                    Return "."c
                Case Else
                    Throw New Exception("Illegal characters in the pattern expression!")
            End Select
        End Function

    End Class

    ''' <summary>
    ''' 使用正则表达式扫描序列得到可能的motif位点
    ''' </summary>
    Public Class Scanner : Inherits IScanner

        Sub New(nt As I_PolymerSequenceModel)
            Call MyBase.New(nt)
        End Sub

        Public Overrides Function Scan(pattern As String) As SimpleSegment()
            Return (Scan(__nt, pattern, "+"c).ToList + Scan(__nt, Complement(pattern), "-"c)).OrderBy(Function(x) x.Start).ToArray
        End Function

        Public Overloads Shared Function Scan(nt As String, pattern As String, strand As Char) As SimpleSegment()
            Dim ms As String() = Regex.Matches(nt, pattern).ToArray.Distinct.ToArray
            Dim locis As SimpleSegment() =
                LinqAPI.Exec(Of SimpleSegment) <= From m As String
                                                  In ms
                                                  Let pos As Integer() = FindLocation(nt, m)
                                                  Let rc As String = New String(NucleicAcid.Complement(m).Reverse.ToArray)
                                                  Select
                                                      LinqAPI.Exec(Of Integer, SimpleSegment)(pos) <=
                                                           Function(ind) New SimpleSegment With {
                                                                .Ends = ind + m.Length,
                                                                .Start = ind,
                                                                .SequenceData = m,
                                                                .Strand = strand.ToString,
                                                                .Complement = rc
                                                      }
            Return locis
        End Function
    End Class
End Namespace