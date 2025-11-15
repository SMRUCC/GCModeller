Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class ZERO

    ReadOnly nucleotides As Char()
    ReadOnly cumulativeProbs As IReadOnlyCollection(Of Double)

    Sub New(background As Dictionary(Of Char, Double))
        Dim cumulativeProbs As New List(Of Double)()
        Dim nucleotides As Char() = background.Keys.ToArray
        ' 构建累积概率分布
        Dim cumulative As Double = 0
        For Each NT As Char In background.Keys
            cumulative += background(NT)
            cumulativeProbs.Add(cumulative)
        Next

        Me.nucleotides = nucleotides
        Me.cumulativeProbs = cumulativeProbs
    End Sub

    Public Function NextSequence(length As Integer) As String
        ' 生成随机序列
        Dim sequence As Char() = New Char(length - 1) {}

        For i As Integer = 1 To length
            Dim rndValue As Double = randf.NextDouble()

            ' 选择核苷酸
            For j As Integer = 0 To nucleotides.Length - 1
                If rndValue <= cumulativeProbs(j) Then
                    sequence(i - 1) = nucleotides(j)
                    Exit For
                End If
            Next
        Next

        Return New String(sequence)
    End Function

End Class