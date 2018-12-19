Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language

Namespace NeuralNetwork.StoreProcedure

    ''' <summary>
    ''' 进行所输入的样本数据的归一化的矩阵
    ''' </summary>
    Public Class NormalizeMatrix : Inherits XmlDataModel

        ''' <summary>
        ''' 每一个属性都具有一个归一化区间
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("matrix")>
        Public Property matrix As DoubleRange()
        ''' <summary>
        ''' 属性名称列表,这个序列的长度是和<see cref="matrix"/>的长度一致的,并且元素的顺序一一对应的
        ''' </summary>
        ''' <returns></returns>
        Public Property names As String()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function NormalizeInput(sample As Sample) As Double()
            Static normalRange As DoubleRange = {0, 1}

            Return sample.status _
                .Select(Function(x, i) matrix(i).ScaleMapping(x, normalRange)) _
                .ToArray
        End Function

        Public Function CreateFromSamples(samples As IEnumerable(Of Sample), names As IEnumerable(Of String)) As NormalizeMatrix
            With samples.ToArray
                Dim len% = .First.status.Length
                Dim index%
                Dim matrix As New List(Of DoubleRange)

                For i As Integer = 0 To len - 1
                    index = i
                    matrix += .Select(Function(sample) sample.status(index)).Range
                Next

                Return New NormalizeMatrix With {
                    .matrix = matrix,
                    .names = names.ToArray
                }
            End With
        End Function
    End Class
End Namespace