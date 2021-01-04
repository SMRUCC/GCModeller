Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Probability

Public Class MotifNeedlemanWunsch : Inherits NeedlemanWunsch(Of Residue)

    Sub New(query As Residue(), subject As Residue())
        Call MyBase.New(defaultScoreMatrix, symbolProvider)

        Me.Sequence1 = query
        Me.Sequence2 = subject
    End Sub

    Private Shared Function symbolProvider() As GenericSymbol(Of Residue)
        Return New GenericSymbol(Of Residue)(
            equals:=Function(a, b) a.topChar = b.topChar,
            similarity:=Function(a, b) a.Cos(b),
            toChar:=Function(x) x.topChar,
            empty:=Function()
                       Return New Residue With {
                           .frequency = New Dictionary(Of Char, Double)
                       }
                   End Function
        )
    End Function

    Public Shared Function defaultScoreMatrix() As ScoreMatrix(Of Residue)
        Return New ScoreMatrix(Of Residue)(
            Function(a, b)
                Dim maxA = Residue.Max(a)
                Dim maxB = Residue.Max(b)

                If a.isEmpty OrElse b.isEmpty Then
                    Return False
                End If

                If maxA = maxB Then
                    Return True
                Else
                    ' A是motif模型，所以不一致的时候以A为准
                    Dim freqB = b(maxA)

                    If freqB < 0.3 Then
                        Return False
                    Else
                        Return True
                    End If
                End If
            End Function) With {.MatchScore = 10}
    End Function
End Class