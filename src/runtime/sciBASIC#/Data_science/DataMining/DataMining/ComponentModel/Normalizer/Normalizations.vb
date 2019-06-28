Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Distributions

Namespace ComponentModel.Normalizer

    Public Enum Methods
        ''' <summary>
        ''' 归一化到[0, 1]区间内
        ''' </summary>
        NormalScaler
        ''' <summary>
        ''' 直接 x / max 进行归一化, 当出现极值的时候, 此方法无效, 根据数据分布,可能会归一化到[0, 1] 或者 [-1, 1]区间内
        ''' </summary>
        RelativeScaler
        ''' <summary>
        ''' 通过对数据进行区间离散化来完成归一化
        ''' </summary>
        RangeDiscretizer
    End Enum

    Public Module Normalizations

        ReadOnly normalRange As DoubleRange = {0, 1}

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ScalerNormalize(samples As SampleDistribution, x#) As Double
            Return samples.GetRange.ScaleMapping(x, normalRange)
        End Function

        ''' <summary>
        ''' 正实数和负实数是分开进行归一化的
        ''' </summary>
        ''' <param name="samples"></param>
        ''' <param name="x#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function RelativeNormalize(samples As SampleDistribution, x#) As Double
            If x > 0 Then
                If x > samples.max Then
                    Return 1
                Else
                    Return x / samples.max
                End If
            ElseIf x = 0R Then
                Return 0
            Else
                ' 负实数需要考察一下
                If x < samples.min Then
                    Return -1
                ElseIf Samples.min >= 0 Then
                    Return -1
                Else
                    Return x / Math.Abs(samples.min)
                End If
            End If
        End Function

        Public Function RangeDiscretizer(samples As SampleDistribution, x#) As Double

        End Function

    End Module
End Namespace