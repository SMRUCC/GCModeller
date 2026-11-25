Imports System.Collections.Generic
Imports System.Drawing
Imports System.Globalization
Imports System.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports FormulaFont = Microsoft.VisualBasic.Imaging.Font
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    Friend MustInherit Class FormulaBox

        Friend Property Width As Single
        Friend Property Height As Single
        Friend Property Ascent As Single
        Friend Property Descent As Single

        Friend MustOverride Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)

        Friend Sub SetMetrics(width As Single, height As Single, ascent As Single)
            Me.Width = System.Math.Max(0F, width)
            Me.Ascent = System.Math.Max(0F, System.Math.Min(ascent, height))
            Me.Height = System.Math.Max(0F, height)
            Me.Descent = Me.Height - Me.Ascent
        End Sub

        Friend Function GetSize() As SizeF
            Return New SizeF(Width, Height)
        End Function

    End Class

    Friend NotInheritable Class TextFormulaBox : Inherits FormulaBox

        Friend ReadOnly Value As String
        Friend ReadOnly TextFont As FormulaFont

        Friend Sub New(value As String, font As FormulaFont, size As SizeF)
            Me.Value = If(value, "")
            Me.TextFont = font
            Call SetMetrics(size.Width, size.Height, font.Size * 0.82F)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Call device.DrawString(Value, TextFont, style.Foreground, x, y)
        End Sub

    End Class

    Friend NotInheritable Class HorizontalFormulaBox : Inherits FormulaBox

        Private ReadOnly items As FormulaBox()
        Private ReadOnly gap As Single

        Friend Sub New(items As IEnumerable(Of FormulaBox), gap As Single)
            Me.items = items.ToArray()
            Me.gap = System.Math.Max(0F, gap)

            Dim totalWidth As Single = 0F
            Dim maxAscent As Single = 0F
            Dim maxDescent As Single = 0F

            For Each item As FormulaBox In Me.items
                totalWidth += item.Width
                maxAscent = System.Math.Max(maxAscent, item.Ascent)
                maxDescent = System.Math.Max(maxDescent, item.Descent)
            Next

            If Me.items.Length > 1 Then
                totalWidth += gap * (Me.items.Length - 1)
            End If

            Call SetMetrics(totalWidth, maxAscent + maxDescent, maxAscent)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Dim cursor As Single = x

            For i As Integer = 0 To items.Length - 1
                Dim item As FormulaBox = items(i)
                Call item.Render(device, style, cursor, y + Ascent - item.Ascent)
                cursor += item.Width

                If i < items.Length - 1 Then
                    cursor += gap
                End If
            Next
        End Sub

    End Class

    Friend NotInheritable Class FractionFormulaBox : Inherits FormulaBox

        Private ReadOnly numerator As FormulaBox
        Private ReadOnly denominator As FormulaBox
        Private ReadOnly ruleWidth As Single

        Friend Sub New(numerator As FormulaBox, denominator As FormulaBox, style As FormulaRenderStyle)
            Me.numerator = numerator
            Me.denominator = denominator
            Me.ruleWidth = System.Math.Max(0.5F, style.FractionRule.Width)

            Dim barWidth As Single = System.Math.Max(numerator.Width, denominator.Width) + style.FractionPadding * 2F
            Dim barTop As Single = numerator.Height + style.FractionGap
            Dim totalHeight As Single = barTop + ruleWidth + style.FractionGap + denominator.Height

            Call SetMetrics(barWidth, totalHeight, barTop)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Call numerator.Render(device, style,
                                  x + (Width - numerator.Width) / 2F,
                                  y)

            Dim ruleY As Single = y + Ascent + ruleWidth / 2F
            Call device.DrawLine(style.FractionRule, x, ruleY, x + Width, ruleY)

            Call denominator.Render(device, style,
                                    x + (Width - denominator.Width) / 2F,
                                    y + Ascent + ruleWidth + style.FractionGap)
        End Sub

    End Class

    Friend NotInheritable Class ScriptFormulaBox : Inherits FormulaBox

        Private ReadOnly baseBox As FormulaBox
        Private ReadOnly scriptBox As FormulaBox
        Private ReadOnly baseY As Single
        Private ReadOnly scriptY As Single

        Friend Sub New(baseBox As FormulaBox, scriptBox As FormulaBox, isSubscript As Boolean, style As FormulaRenderStyle)
            Me.baseBox = baseBox
            Me.scriptBox = scriptBox

            Dim baselineOffset As Single

            If isSubscript Then
                baselineOffset = baseBox.Ascent * 1.45F
            Else
                baselineOffset = baseBox.Ascent * 0.45F
            End If

            Dim rawScriptY As Single = baselineOffset - scriptBox.Ascent
            Dim shift As Single = If(rawScriptY < 0F, -rawScriptY, 0F)

            Me.baseY = shift
            Me.scriptY = shift + rawScriptY

            Dim maxAscent As Single = baseBox.Ascent + shift
            Dim maxDescent As Single = System.Math.Max(baseBox.Descent, scriptY + scriptBox.Height)

            Call SetMetrics(baseBox.Width + style.ScriptGap + scriptBox.Width,
                            maxAscent + maxDescent,
                            maxAscent)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Call baseBox.Render(device, style, x, y + baseY)
            Call scriptBox.Render(device, style, x + baseBox.Width + style.ScriptGap, y + scriptY)
        End Sub

    End Class

    Friend NotInheritable Class GroupFormulaBox : Inherits FormulaBox

        Private ReadOnly leftBracket As TextFormulaBox
        Private ReadOnly content As FormulaBox
        Private ReadOnly rightBracket As TextFormulaBox
        Private ReadOnly gap As Single

        Friend Sub New(content As FormulaBox, context As FormulaLayoutContext)
            Me.content = content
            Me.gap = context.Style.BracketGap
            Me.leftBracket = context.CreateText("(")
            Me.rightBracket = context.CreateText(")")

            Dim maxAscent As Single = System.Math.Max(System.Math.Max(leftBracket.Ascent, rightBracket.Ascent), content.Ascent)
            Dim maxDescent As Single = System.Math.Max(System.Math.Max(leftBracket.Descent, rightBracket.Descent), content.Descent)

            Call SetMetrics(leftBracket.Width + content.Width + rightBracket.Width + gap * 2F,
                            maxAscent + maxDescent,
                            maxAscent)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Call leftBracket.Render(device, style, x, y + Ascent - leftBracket.Ascent)

            Dim contentX As Single = x + leftBracket.Width + gap
            Call content.Render(device, style, contentX, y + Ascent - content.Ascent)

            Dim rightX As Single = contentX + content.Width + gap
            Call rightBracket.Render(device, style, rightX, y + Ascent - rightBracket.Ascent)
        End Sub

    End Class

    ''' <summary>
    ''' Converts MathML expression objects into a drawable layout tree.
    ''' </summary>
    Friend NotInheritable Partial Class FormulaLayoutContext

        Private ReadOnly device As IGraphics
        Private ReadOnly textSizes As New Dictionary(Of String, SizeF)

        Friend ReadOnly Property Style As FormulaRenderStyle

        Friend Sub New(device As IGraphics, style As FormulaRenderStyle)
            If device Is Nothing Then Throw New ArgumentNullException(NameOf(device))
            If style Is Nothing Then Throw New ArgumentNullException(NameOf(style))

            Me.device = device
            Me.Style = style
        End Sub

        Friend Function CreateText(value As String, Optional font As FormulaFont = Nothing) As TextFormulaBox
            Dim text As String = If(value, "")
            Dim textFont As FormulaFont = If(font, Style.Font)
            Dim key As String = textFont.Name & "|" &
                                textFont.Size.ToString("0.###", CultureInfo.InvariantCulture) & "|" &
                                CInt(textFont.Style).ToString(CultureInfo.InvariantCulture) & "|" &
                                CInt(textFont.Unit).ToString(CultureInfo.InvariantCulture) & "|" & text

            Dim size As SizeF

            If Not textSizes.TryGetValue(key, size) Then
                size = device.MeasureString(text, textFont)
                textSizes(key) = size
            End If

            Return New TextFormulaBox(text, textFont, size)
        End Function

        Friend Function Layout(lambda As LambdaExpression) As FormulaBox
            If lambda Is Nothing Then Throw New ArgumentNullException(NameOf(lambda))
            If lambda.lambda Is Nothing Then
                Throw New ArgumentException("The MathML content has no apply body.", NameOf(lambda))
            End If

            Return Layout(lambda.lambda)
        End Function

        Friend Function Layout(expression As MathExpression) As FormulaBox
            If TypeOf expression Is BinaryExpression Then
                Return LayoutBinary(DirectCast(expression, BinaryExpression))
            ElseIf TypeOf expression Is SymbolExpression Then
                Return LayoutSymbol(DirectCast(expression, SymbolExpression))
            ElseIf TypeOf expression Is MathFunctionExpression Then
                Return LayoutFunction(DirectCast(expression, MathFunctionExpression))
            ElseIf TypeOf expression Is BoundedOperatorExpression Then
                Return LayoutBoundedOperator(DirectCast(expression, BoundedOperatorExpression))
            ElseIf TypeOf expression Is MatrixExpression Then
                Return LayoutMatrix(DirectCast(expression, MatrixExpression))
            Else
                Throw New NotSupportedException($"Unsupported math expression node: {If(expression Is Nothing, "Nothing", expression.GetType.Name)}")
            End If
        End Function

        Private Function LayoutBinary(binary As BinaryExpression) As FormulaBox
            Dim op As String = If(binary.operator, "")

            If op = "divide" Then
                Return New FractionFormulaBox(Layout(binary.applyleft),
                                              Layout(binary.applyright),
                                              Style)
            End If

            If op = "power" Then
                Dim baseBox As FormulaBox = LayoutWithParentheses(binary.applyleft, op, False)
                Dim exponentBox As FormulaBox = LayoutScript(binary.applyright)
                Return New ScriptFormulaBox(baseBox, exponentBox, False, Style)
            End If

            Dim leftBox As FormulaBox = LayoutWithParentheses(binary.applyleft, op, False)
            Dim isImplicit As Boolean = (op = "implicit")
            Dim operatorBox As TextFormulaBox = CreateText(If(isImplicit, "", OperatorSymbol(op)))
            Dim rightBox As FormulaBox = LayoutWithParentheses(binary.applyright, op, True)
            Dim gap As Single = If(isImplicit, 0F, Style.OperatorGap)

            Return New HorizontalFormulaBox({leftBox, operatorBox, rightBox}, gap)
        End Function

        Private Function LayoutSymbol(symbol As SymbolExpression) As FormulaBox
            Dim text As String = If(symbol?.text, "")

            If symbol IsNot Nothing AndAlso symbol.isNumericLiteral Then
                Dim slash As Integer = text.IndexOf("/"c)

                If slash > 0 AndAlso slash = text.LastIndexOf("/"c) Then
                    Dim numerator As String = text.Substring(0, slash).Trim
                    Dim denominator As String = text.Substring(slash + 1).Trim

                    If numerator.Length > 0 AndAlso denominator.Length > 0 Then
                        Return New FractionFormulaBox(CreateText(numerator), CreateText(denominator), Style)
                    End If
                End If
            End If

            Dim subIndex As Integer = text.IndexOf("_"c)
            Dim supIndex As Integer = text.IndexOf("^"c)
            Dim separator As Integer = -1
            Dim isSubscript As Boolean = False

            If subIndex >= 0 AndAlso (supIndex < 0 OrElse subIndex < supIndex) Then
                separator = subIndex
                isSubscript = True
            ElseIf supIndex >= 0 Then
                separator = supIndex
            End If

            If separator <= 0 OrElse separator = text.Length - 1 Then
                Return CreateText(text)
            End If

            Dim baseText As String = text.Substring(0, separator)
            Dim scriptText As String = text.Substring(separator + 1)
            Dim scriptFont As FormulaFont = Style.CreateScriptFont(Style.Font)
            Dim scriptBox As FormulaBox

            Try
                Dim scriptExpression As MathExpression = LaTeXMath.ParseLatexExpression(scriptText)
                Dim scriptContext As New FormulaLayoutContext(device, Style.CreateScriptStyle())
                scriptBox = scriptContext.Layout(scriptExpression)
            Catch
                scriptBox = CreateText(scriptText, scriptFont)
            End Try

            Return New ScriptFormulaBox(CreateText(baseText), scriptBox, isSubscript, Style)
        End Function

        Private Function LayoutFunction([function] As MathFunctionExpression) As FormulaBox
            Dim items As New List(Of FormulaBox) From {
                CreateText(If([function].name, "f")),
                CreateText("(")
            }

            If [function].parameters IsNot Nothing Then
                For i As Integer = 0 To [function].parameters.Length - 1
                    If i > 0 Then
                        items.Add(CreateText(","))
                    End If

                    items.Add(Layout([function].parameters(i)))
                Next
            End If

            items.Add(CreateText(")"))
            Return New HorizontalFormulaBox(items, Style.OperatorGap * 0.45F)
        End Function

        Private Function LayoutWithParentheses(expression As MathExpression, parentOperator As String, isRightOperand As Boolean) As FormulaBox
            Dim box As FormulaBox = Layout(expression)

            If NeedsParentheses(expression, parentOperator, isRightOperand) Then
                Return New GroupFormulaBox(box, Me)
            End If

            Return box
        End Function

        Private Function LayoutScript(expression As MathExpression) As FormulaBox
            Dim scriptContext As New FormulaLayoutContext(device, Style.CreateScriptStyle())
            Return scriptContext.Layout(expression)
        End Function

        Private Shared Function Precedence(op As String) As Integer
            Select Case op
                Case "eq", "leq", "geq", "neq" : Return 0
                Case "plus", "minus" : Return 1
                Case "times", "divide", "implicit" : Return 2
                Case "power" : Return 3
                Case Else : Return 1
            End Select
        End Function

        Private Shared Function OperatorSymbol(op As String) As String
            Select Case op
                Case "plus" : Return "+"
                Case "minus" : Return "−"
                Case "times" : Return "×"
                Case "oplus" : Return "⊕"
                Case "implicit" : Return ""
                Case "divide" : Return "∕"
                Case "power" : Return "^"
                Case "eq" : Return "="
                Case "leq" : Return "≤"
                Case "geq" : Return "≥"
                Case "neq" : Return "≠"
                Case Else : Return If(op, "")
            End Select
        End Function

        Private Shared Function NeedsParentheses(expression As MathExpression, parentOperator As String, isRightOperand As Boolean) As Boolean
            Dim child As BinaryExpression = TryCast(expression, BinaryExpression)

            If child Is Nothing Then
                Return False
            End If

            Dim childPrec As Integer = Precedence(child.operator)
            Dim parentPrec As Integer = Precedence(parentOperator)

            If childPrec < parentPrec Then
                Return True
            End If

            If childPrec = parentPrec Then
                If parentOperator = "minus" OrElse parentOperator = "divide" Then
                    Return isRightOperand
                ElseIf parentOperator = "power" Then
                    Return True
                End If
            End If

            Return False
        End Function

    End Class
End Namespace
