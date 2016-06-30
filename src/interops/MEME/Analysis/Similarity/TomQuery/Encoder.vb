Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic

Namespace Analysis.Similarity.TOMQuery

    Module Encoder

        ''' <summary>
        ''' 对残基在列之中出现的概率进行分级
        ''' </summary>
        ''' <param name="p">0-1之间的概率值</param>
        ''' <returns>ABCDE</returns>
        <Extension> Public Function Level(p As Double) As Char
            If p >= 0 AndAlso p < 0.2 Then
                Return "1"c
            ElseIf p >= 0.2 AndAlso p < 0.4 Then
                Return "2"c
            ElseIf p >= 0.4 AndAlso p < 0.6 Then
                Return "3"c
            ElseIf p >= 0.6 AndAlso p < 0.8 Then
                Return "4"c
            Else
                Return "5"c
            End If
        End Function

        Public Function ToString(model As Analysis.MotifScans.AnnotationModel) As String
            Dim chars As New List(Of Char)

            For Each column As MotifScans.ResidueSite In model.PWM  ' ATGC
                Call chars.Add("A"c, column.PWM(0).Level)
                Call chars.Add("T"c, column.PWM(1).Level)
                Call chars.Add("G"c, column.PWM(2).Level)
                Call chars.Add("C"c, column.PWM(3).Level)
            Next

            Dim s As String = New String(chars.ToArray)
            Return s
        End Function
    End Module
End Namespace