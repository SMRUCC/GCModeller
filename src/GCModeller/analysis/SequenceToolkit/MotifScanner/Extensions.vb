Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif

Public Module Extensions

    <Extension>
    Public Function CreateModel(pwm As Probability) As MotifPWM
        Dim alphabets As Char() = New Char() {"A"c, "C"c, "G"c, "T"c}
        Dim n As Integer = 100  ' normalized as 100 sequence input
        Dim E As Double = Probability.E(nsize:=100)

        Return New MotifPWM With {
            .name = pwm.name,
            .note = pwm.ToString,
            .pwm = pwm.region _
                .Select(Function(r)
                            Dim hi As Double = Probability.HI(r)

                            Return New ResidueSite(r, alphabets) With {
                                .bits = Probability.CalculatesBits(hi, E, NtMol:=alphabets.Length = 4)
                            }
                        End Function) _
                .ToArray,
            .alphabets = alphabets
        }
    End Function

End Module
