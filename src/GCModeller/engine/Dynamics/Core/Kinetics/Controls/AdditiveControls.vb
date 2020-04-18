Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    ''' <summary>
    ''' 基于累加效应的动力学模型
    ''' 
    ''' 这个模型可能比较适用于基因表达过程的简略建模
    ''' </summary>
    Public Class AdditiveControls : Inherits Controls

        Public Property activation As Variable() = {}
        ''' <summary>
        ''' 如果抑制的总量大于激活的总量，那么这个调控的反应过程将不会进行
        ''' </summary>
        ''' <returns></returns>
        Public Property inhibition As Variable() = {}

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property coefficient As Double
            Get
                If activation.IsNullOrEmpty AndAlso inhibition.IsNullOrEmpty Then
                    Return baseline
                End If

                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = activation.Sum(Function(v) v.coefficient * v.mass.Value)

                ' 抑制的总量已经大于等于激活的总量的时候，返回零值，
                ' 则反应过程可能不会发生
                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        Public Overrides Function ToString() As String
            If coefficient > 0 Then
                Return $"active by {activation.Select(Function(a) a.mass.ID).GetJson}"
            Else
                Return "No activity!"
            End If
        End Function

        ''' <summary>
        ''' 这个反应在当前方向上是自然发生的，速率为<see cref="baseline"/>
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Public Overloads Shared Widening Operator CType(base As Double) As AdditiveControls
            Return New AdditiveControls With {
                .baseline = base
            }
        End Operator
    End Class
End Namespace