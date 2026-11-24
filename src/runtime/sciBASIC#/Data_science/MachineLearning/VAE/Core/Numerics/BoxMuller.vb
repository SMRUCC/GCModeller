' ============================================================================
' BoxMuller.vb - 成对正态采样
'
' 修正 MathUtils.Randn（MathUtils.vb L189）每次只取 sin 分支、丢弃 cos 分支
' 造成的一倍浪费：Box-Muller 变换一次可以产生两个相互独立的标准正态样本。
' ============================================================================

Imports std = System.Math

Namespace Core.Numerics

    ''' <summary>
    ''' Box-Muller 正态采样。
    ''' </summary>
    Public Module BoxMuller

        ''' <summary>生成一个标准正态样本。</summary>
        Public Function Sample(rng As Random) As Double
            Dim u1 As Double = 1.0 - rng.NextDouble()
            Dim u2 As Double = 1.0 - rng.NextDouble()

            Return std.Sqrt(-2.0 * std.Log(u1)) * std.Cos(2.0 * std.PI * u2)
        End Function

        ''' <summary>
        ''' 一次生成两个相互独立的标准正态样本（同时使用极坐标的 cos 与 sin 分支）。
        ''' </summary>
        Public Sub NextPair(rng As Random, ByRef z0 As Double, ByRef z1 As Double)
            Dim u1 As Double = 1.0 - rng.NextDouble()
            Dim u2 As Double = 1.0 - rng.NextDouble()
            Dim radius As Double = std.Sqrt(-2.0 * std.Log(u1))
            Dim theta As Double = 2.0 * std.PI * u2

            z0 = radius * std.Cos(theta)
            z1 = radius * std.Sin(theta)
        End Sub

        ''' <summary>
        ''' 用标准正态样本填充 <paramref name="buffer"/> 的指定区间，成对生成。
        ''' </summary>
        Public Sub Fill(rng As Random, buffer As Double(), start As Integer, count As Integer)
            If buffer Is Nothing OrElse count <= 0 Then
                Return
            End If

            Dim finish As Integer = std.Min(start + count, buffer.Length)
            Dim i As Integer = start

            While i + 1 < finish
                Call NextPair(rng, buffer(i), buffer(i + 1))
                i += 2
            End While

            If i < finish Then
                buffer(i) = Sample(rng)
            End If
        End Sub

        ''' <summary>生成指定长度的标准正态样本数组。</summary>
        Public Function Fill(rng As Random, count As Integer) As Double()
            Dim buffer As Double() = New Double(count - 1) {}

            Call Fill(rng, buffer, 0, count)

            Return buffer
        End Function

    End Module

End Namespace
