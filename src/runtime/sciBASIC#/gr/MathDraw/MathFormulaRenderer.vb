Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    ''' <summary>
    ''' Renders MathML expression objects through the abstract <see cref="IGraphics"/> device.
    ''' </summary>
    Public Module MathFormulaRenderer

        Public Function MeasureMathML(device As IGraphics, xmlText As String, Optional style As FormulaRenderStyle = Nothing) As SizeF
            If xmlText Is Nothing Then Throw New ArgumentNullException(NameOf(xmlText))
            Return Measure(device, LambdaExpression.FromMathML(xmlText), style)
        End Function

        Public Function DrawMathML(device As IGraphics, xmlText As String, origin As PointF, Optional style As FormulaRenderStyle = Nothing) As SizeF
            If xmlText Is Nothing Then Throw New ArgumentNullException(NameOf(xmlText))
            Return Draw(device, LambdaExpression.FromMathML(xmlText), origin, style)
        End Function

        Public Function Measure(device As IGraphics, lambda As LambdaExpression, Optional style As FormulaRenderStyle = Nothing) As SizeF
            Dim layoutStyle As FormulaRenderStyle = ResolveStyle(style)
            Dim context As New FormulaLayoutContext(device, layoutStyle)
            Return context.Layout(lambda).GetSize()
        End Function

        Public Function Draw(device As IGraphics, lambda As LambdaExpression, origin As PointF, Optional style As FormulaRenderStyle = Nothing) As SizeF
            Dim layoutStyle As FormulaRenderStyle = ResolveStyle(style)
            Dim context As New FormulaLayoutContext(device, layoutStyle)
            Dim box As FormulaBox = context.Layout(lambda)

            Call box.Render(device, layoutStyle, origin.X, origin.Y)
            Return box.GetSize()
        End Function

        Public Function Measure(device As IGraphics, expression As MathExpression, Optional style As FormulaRenderStyle = Nothing) As SizeF
            Dim layoutStyle As FormulaRenderStyle = ResolveStyle(style)
            Dim context As New FormulaLayoutContext(device, layoutStyle)
            Return context.Layout(expression).GetSize()
        End Function

        Public Function Draw(device As IGraphics, expression As MathExpression, origin As PointF, Optional style As FormulaRenderStyle = Nothing) As SizeF
            Dim layoutStyle As FormulaRenderStyle = ResolveStyle(style)
            Dim context As New FormulaLayoutContext(device, layoutStyle)
            Dim box As FormulaBox = context.Layout(expression)

            Call box.Render(device, layoutStyle, origin.X, origin.Y)
            Return box.GetSize()
        End Function

        Private Function ResolveStyle(style As FormulaRenderStyle) As FormulaRenderStyle
            Return If(style, New FormulaRenderStyle())
        End Function

    End Module
End Namespace
