Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    ''' <summary>
    ''' A small facade around an <see cref="IGraphics"/> device for formula drawing.
    ''' </summary>
    Public Class Canvas

        Public Sub New(device As IGraphics, Optional style As FormulaRenderStyle = Nothing)
            If device Is Nothing Then Throw New ArgumentNullException(NameOf(device))

            Me.Device = device
            Me.Style = If(style, New FormulaRenderStyle())
        End Sub

        Public ReadOnly Property Device As IGraphics
        Public ReadOnly Property Style As FormulaRenderStyle

        Public Function MeasureMathML(xmlText As String) As SizeF
            Return MathFormulaRenderer.MeasureMathML(Device, xmlText, Style)
        End Function

        Public Function DrawMathML(xmlText As String, origin As PointF) As SizeF
            Return MathFormulaRenderer.DrawMathML(Device, xmlText, origin, Style)
        End Function

        Public Function Measure(lambda As LambdaExpression) As SizeF
            Return MathFormulaRenderer.Measure(Device, lambda, Style)
        End Function

        Public Function Draw(lambda As LambdaExpression, origin As PointF) As SizeF
            Return MathFormulaRenderer.Draw(Device, lambda, origin, Style)
        End Function

        Public Function Measure(expression As MathExpression) As SizeF
            Return MathFormulaRenderer.Measure(Device, expression, Style)
        End Function

        Public Function Draw(expression As MathExpression, origin As PointF) As SizeF
            Return MathFormulaRenderer.Draw(Device, expression, origin, Style)
        End Function

        Public Sub Flush()
            Call Device.Flush()
        End Sub

    End Class
End Namespace
