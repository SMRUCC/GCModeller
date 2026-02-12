Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq

Public Module SmithWaterman

    Public Function Compare(prob As Residue, base As Residue) As Double
        For Each b As Char In SequenceModel.NT
            ' 如果当前的碱基是b的时候
            If base.frequency(b) = 1.0R Then
                ' 则比较的得分就是当前的碱基b在motif模型中
                ' 对应的出现频率的高低
                ' 很明显，出现的频率越高，得分越高
                Return prob.frequency(b) * 10
            End If
        Next

        ' 当前的序列位点为N任意碱基的时候
        ' 则取最大的出现频率的得分
        With prob.frequency.ToArray
            Return .ElementAt(which.Min(.Values)) _
                   .Value * 10
        End With
    End Function

    Public Function MakeSymbol(Optional equals As Double = 0.85) As GenericSymbol(Of Residue)
        Return New GenericSymbol(Of Residue)(
            equals:=Function(a, b) Compare(a, b) >= equals,
            similarity:=AddressOf Compare,
            toChar:=AddressOf Residue.Max,
            empty:=AddressOf Residue.GetEmpty
        )
    End Function

    ''' <summary>
    ''' Make smith-waterman alignment between two motif PWM data
    ''' </summary>
    ''' <param name="pwm1">the query motif input</param>
    ''' <param name="pwm2">the subject motif input</param>
    ''' <param name="cutoff"></param>
    ''' <param name="minW"></param>
    ''' <param name="top"></param>
    ''' <returns></returns>
    Public Function MakeAlignment(pwm1 As IEnumerable(Of Residue), pwm2 As IEnumerable(Of Residue),
                                  Optional cutoff As Double = 0.6,
                                  Optional minW As Integer = 6,
                                  Optional top As Integer = 9) As IEnumerable(Of Match)

        Dim symbol As GenericSymbol(Of Residue) = SmithWaterman.MakeSymbol(equals:=0.85)
        ' create a general smith-waterman(GSW) alignment algorithm
        Dim core As New GSW(Of Residue)(pwm1, pwm2, symbol)
        Dim score_cutoff As Double = cutoff * core.MaxScore

        Return From hsp As Match
               In core _
                   .BuildMatrix _
                   .GetMatches(score_cutoff)
               Where (hsp.toB - hsp.fromB) >= minW
               Order By hsp.score Descending
               Take top
    End Function
End Module
