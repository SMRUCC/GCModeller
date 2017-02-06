Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.IO

Namespace CytoscapeGraphView.Cyjs.style

    Public Delegate Function GetProperty(name$) As String

    ''' <summary>
    ''' The <see cref="style.css"/> (<see cref="CSSTranslator"/>) value expression
    ''' </summary>
    Public Module Expression

        ''' <summary>
        ''' ``mapData(strength,70,100,2,6)``
        ''' </summary>
        ''' <param name="exp$"></param>
        ''' <returns></returns>
        Public Function ValueMap(exp$) As Func(Of GetProperty, Double)
            Dim params$ = exp.GetStackValue("(", ")")
            Dim t As String() = Tokenizer.CharsParser(params)
            Dim key$ = t(Scan0)
            Dim rangeData As New DoubleRange(Val(t(1)), Val(t(2)))
            Dim rangeValue As New DoubleRange(Val(t(3)), Val(t(4)))

            Return Function(obj As GetProperty) As Double
                       Dim value As Double = Val(obj(name:=key))

                       If value <= rangeData.Min Then
                           Return rangeValue.Min
                       ElseIf value >= rangeData.Max Then
                           Return rangeValue.Max
                       Else
                           Return rangeData.ScaleMapping(value, rangeValue)
                       End If
                   End Function
        End Function
    End Module
End Namespace