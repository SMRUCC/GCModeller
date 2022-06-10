Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    ''' <summary>
    ''' 使用正则表达式扫描序列得到可能的motif位点
    ''' </summary>
    Public Class Scanner : Inherits IScanner

        Sub New(nt As IPolymerSequenceModel)
            Call MyBase.New(nt)
        End Sub

        Public Overrides Function Scan(pattern As String) As SimpleSegment()
            Return (Scan(nt, pattern, "+"c).AsList + Scan(nt, Complement(pattern), "-"c)).OrderBy(Function(x) x.Start).ToArray
        End Function

        Public Overloads Shared Function Scan(nt As String, pattern As String, strand As Char) As SimpleSegment()
            Dim ms$() = Regex.Matches(nt, pattern) _
                .ToArray _
                .Distinct _
                .ToArray
            Dim locis = LinqAPI.Exec(Of SimpleSegment) _
                                                       _
                () <= From m As String
                      In ms
                      Let pos As Integer() = FindLocation(nt, m)
                      Let rc As String = New String(NucleicAcid.Complement(m).Reverse.ToArray)
                      Select LinqAPI.Exec(Of Integer, SimpleSegment)(pos) _
                                                                          _
                          <= Function(ind As Integer) As SimpleSegment
                                 Return New SimpleSegment With {
                                     .Ends = ind + m.Length,
                                     .Start = ind,
                                     .SequenceData = m,
                                     .strand = strand.ToString  ' .Complement = rc
                                 }
                             End Function

            Return locis
        End Function
    End Class
End Namespace