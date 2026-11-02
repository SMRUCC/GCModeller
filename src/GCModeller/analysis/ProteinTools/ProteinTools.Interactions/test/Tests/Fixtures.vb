Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace test.Tests

    ''' <summary>
    ''' 测试数据构造工具。
    ''' </summary>
    ''' <remarks>
    ''' 这里的随机数使用自己实现的线性同余发生器，而不是 <see cref="Random"/>，
    ''' 这样可以保证测试数据在任何 .NET 版本之上都完全一致，
    ''' 因此断言里使用的常数永远不会因为运行环境变化而失效。
    ''' </remarks>
    Module Fixtures

        ''' <summary>
        ''' 一个确定性的线性同余随机数发生器
        ''' </summary>
        Public Class Lcg

            Private _state As ULong

            Public Sub New(seed As ULong)
                _state = If(seed = 0UL, 1UL, seed)
            End Sub

            ''' <summary>
            ''' 返回 <c>[0, bound)</c> 区间之内的整数
            ''' </summary>
            Public Function [Next](bound As Integer) As Integer
                _state = _state * 6364136223846793005UL + 1442695040888963407UL

                Return CInt((_state >> 33) Mod CULng(bound))
            End Function

            ''' <summary>
            ''' 返回 <c>[0, 1)</c> 区间之内的浮点数
            ''' </summary>
            Public Function NextDouble() As Double
                _state = _state * 6364136223846793005UL + 1442695040888963407UL

                Return (_state >> 11) / CDbl(1UL << 53)
            End Function
        End Class

        ''' <summary>
        ''' 由文本行构造联合比对，每行的长度必须一致
        ''' </summary>
        Public Function FromRows(rows As String(), lengthA As Integer, Optional names As String() = Nothing) As JointAlignment
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21
            Dim data As Integer(,) = New Integer(rows.Length - 1, rows(0).Length - 1) {}

            For r As Integer = 0 To rows.Length - 1
                Dim codes As Integer() = alphabet.Encode(rows(r), "row" & r)

                For c As Integer = 0 To rows(r).Length - 1
                    data(r, c) = codes(c)
                Next
            Next

            Return New JointAlignment(data, names, lengthA, rows(0).Length - lengthA, alphabet)
        End Function

        ''' <summary>
        ''' 构造一个完全随机的联合比对(各列之间相互独立)
        ''' </summary>
        Public Function RandomAlignment(rows As Integer, lengthA As Integer, lengthB As Integer, seed As ULong, Optional distinctResidues As Integer = 4) As JointAlignment
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21
            Dim rng As New Lcg(seed)
            Dim data As Integer(,) = New Integer(rows - 1, lengthA + lengthB - 1) {}
            Dim names As String() = New String(rows - 1) {}

            For r As Integer = 0 To rows - 1
                names(r) = "genome_" & r.ToString("000")

                For c As Integer = 0 To lengthA + lengthB - 1
                    data(r, c) = rng.Next(distinctResidues)
                Next
            Next

            Return New JointAlignment(data, names, lengthA, lengthB, alphabet)
        End Function

        ''' <summary>
        ''' 构造两列相互独立的比对：列 <paramref name="independentColumn"/> 与第 0 列无关，
        ''' 列 <paramref name="coupledColumn"/> 与第 0 列完全一致。
        ''' </summary>
        Public Function IndependentVersusCoupled(rows As Integer, independentColumn As Integer, coupledColumn As Integer, seed As ULong) As JointAlignment
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21
            Dim rng As New Lcg(seed)
            Dim columns As Integer = Math.Max(independentColumn, coupledColumn) + 1
            Dim lengthA As Integer = columns
            Dim data As Integer(,) = New Integer(rows - 1, columns - 1) {}
            Dim names As String() = New String(rows - 1) {}

            For r As Integer = 0 To rows - 1
                names(r) = "row_" & r
                Dim first As Integer = rng.Next(4)

                data(r, 0) = first

                For c As Integer = 1 To columns - 1
                    data(r, c) = rng.Next(4)
                Next

                data(r, independentColumn) = rng.Next(4)
                data(r, coupledColumn) = first
            Next

            Return New JointAlignment(data, names, lengthA, 0, alphabet)
        End Function
    End Module
End Namespace
