Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace MathML

    ''' <summary>
    ''' LaTeX formula string support for the MathML expression object model.
    ''' </summary>
    Public Module LaTeXMath

        Public Function ParseLatex(text As String) As LambdaExpression
            Return New LambdaExpression With {
                .parameters = New String() {},
                .lambda = ParseLatexExpression(text)
            }
        End Function

        Public Function FromLatex(text As String) As LambdaExpression
            Return ParseLatex(text)
        End Function

        Public Function ParseLatexExpression(text As String) As MathExpression
            If text Is Nothing Then Throw New ArgumentNullException(NameOf(text))
            Return New LaTeXParser(text).ParseRoot()
        End Function

        Public Function ToLatex(lambda As LambdaExpression) As String
            If lambda Is Nothing Then Throw New ArgumentNullException(NameOf(lambda))
            If lambda.lambda Is Nothing Then Throw New ArgumentException("The LaTeX expression has no body.", NameOf(lambda))

            Return ToLatex(lambda.lambda)
        End Function

        Public Function ToLatex(expression As MathExpression) As String
            If TypeOf expression Is BinaryExpression Then
                Return WriteBinary(DirectCast(expression, BinaryExpression))
            ElseIf TypeOf expression Is SymbolExpression Then
                Return WriteSymbol(DirectCast(expression, SymbolExpression))
            ElseIf TypeOf expression Is MathFunctionExpression Then
                Return WriteFunction(DirectCast(expression, MathFunctionExpression))
            ElseIf TypeOf expression Is BoundedOperatorExpression Then
                Return WriteBounded(DirectCast(expression, BoundedOperatorExpression))
            ElseIf TypeOf expression Is MatrixExpression Then
                Return WriteMatrix(DirectCast(expression, MatrixExpression))
            Else
                Throw New NotSupportedException($"Unsupported math expression node: {If(expression Is Nothing, "Nothing", expression.GetType.Name)}")
            End If
        End Function

        Private Function WriteBinary(binary As BinaryExpression) As String
            Dim op As String = If(binary.operator, "")
            Dim left As MathExpression = binary.applyleft
            Dim right As MathExpression = binary.applyright

            Select Case op
                Case "divide"
                    Return "\frac{" & ToLatex(left) & "}{" & ToLatex(right) & "}"
                Case "power"
                    Return Operand(left) & "^{" & ToLatex(right) & "}"
                Case "plus"
                    Return Operand(left) & " + " & Operand(right)
                Case "minus"
                    Return Operand(left) & " - " & Operand(right)
                Case "times"
                    Return Operand(left) & " \times " & Operand(right)
                Case "oplus"
                    Return Operand(left) & " \oplus " & Operand(right)
                Case "implicit"
                    Return Operand(left) & " " & Operand(right)
                Case "eq"
                    Return Operand(left) & " = " & Operand(right)
                Case "leq"
                    Return Operand(left) & " \leq " & Operand(right)
                Case "geq"
                    Return Operand(left) & " \geq " & Operand(right)
                Case "neq"
                    Return Operand(left) & " \neq " & Operand(right)
                Case Else
                    Return Operand(left) & " " & op & " " & Operand(right)
            End Select
        End Function

        Private Function WriteSymbol(symbol As SymbolExpression) As String
            Return WriteSymbolText(If(symbol?.text, ""))
        End Function

        Private Function WriteFunction([function] As MathFunctionExpression) As String
            Dim name As String = If([function].name, "f")
            Dim args As String() = If([function].parameters Is Nothing,
                                      New String() {},
                                      [function].parameters.Select(Function(p) ToLatex(p)).ToArray)

            If name = "sqrt" Then
                Return "\sqrt{" & String.Join(", ", args) & "}"
            End If

            Return "\" & name & "{" & String.Join(", ", args) & "}"
        End Function

        Private Function WriteBounded(expression As BoundedOperatorExpression) As String
            Dim op As String = If(expression.[operator], "sum")
            Dim lower As String = If(expression.lower Is Nothing, "", ToLatex(expression.lower))
            Dim upper As String = If(expression.upper Is Nothing, "", ToLatex(expression.upper))
            Dim body As String = Operand(expression.body)

            Select Case op
                Case "limit"
                    Return "\lim_{" & lower & "} " & body
                Case "product"
                    Return "\prod_{" & lower & "}^{" & upper & "} " & body
                Case "int", "integral"
                    Return "\int_{" & lower & "}^{" & upper & "} " & body
                Case "diff", "partialdiff"
                    Return WriteDerivative(expression, op)
                Case Else
                    Return "\sum_{" & lower & "}^{" & upper & "} " & body
            End Select
        End Function

        Private Function WriteDerivative(expression As BoundedOperatorExpression, op As String) As String
            Dim symbol As String = If(op = "partialdiff", "\partial", "d")
            Dim degree As String = If(expression.degree Is Nothing, "", "^{" & ToLatex(expression.degree) & "}")
            Dim variable As String = If(expression.variable, "")
            Dim numerator As String = symbol & degree & Operand(expression.body)
            Dim denominator As String = symbol & variable & degree
            Return "\frac{" & numerator & "}{" & denominator & "}"
        End Function

        Private Function WriteMatrix(matrix As MatrixExpression) As String
            If matrix.rows Is Nothing Then
                Return "\begin{matrix}\end{matrix}"
            End If

            Dim rows As New List(Of String)

            For Each row As MathExpression() In matrix.rows
                If row Is Nothing Then
                    Call rows.Add("")
                Else
                    Call rows.Add(String.Join(" & ", row.Select(Function(cell) ToLatex(cell))))
                End If
            Next

            Return "\begin{matrix}" & String.Join(" \\ ", rows.ToArray()) & "\end{matrix}"
        End Function

        Private Function Operand(expression As MathExpression) As String
            If TypeOf expression Is SymbolExpression OrElse
               TypeOf expression Is MathFunctionExpression OrElse
               TypeOf expression Is BoundedOperatorExpression OrElse
               TypeOf expression Is MatrixExpression Then
                Return ToLatex(expression)
            Else
                Return "{" & ToLatex(expression) & "}"
            End If
        End Function

        Private Function WriteSymbolText(text As String) As String
            Dim subIndex As Integer = text.IndexOf("_"c)
            Dim supIndex As Integer = text.IndexOf("^"c)
            Dim separator As Integer = -1
            Dim isSub As Boolean = False

            If subIndex >= 0 AndAlso (supIndex < 0 OrElse subIndex < supIndex) Then
                separator = subIndex
                isSub = True
            ElseIf supIndex >= 0 Then
                separator = supIndex
            End If

            If separator <= 0 Then
                Return EscapeSymbol(text)
            End If

            Dim result As New StringBuilder(EscapeSymbol(text.Substring(0, separator)))
            Dim remaining As String = text.Substring(separator + 1)

            If isSub Then
                Dim nextSup As Integer = remaining.IndexOf("^"c)

                If nextSup >= 0 Then
                    Call result.Append("_{").Append(EscapeSymbol(remaining.Substring(0, nextSup))).Append("}")
                    remaining = remaining.Substring(nextSup + 1)
                    Call result.Append("^{").Append(EscapeSymbol(remaining)).Append("}")
                Else
                    Call result.Append("_{").Append(EscapeSymbol(remaining)).Append("}")
                End If
            Else
                Dim nextSub As Integer = remaining.IndexOf("_"c)

                If nextSub >= 0 Then
                    Call result.Append("^{").Append(EscapeSymbol(remaining.Substring(0, nextSub))).Append("}")
                    remaining = remaining.Substring(nextSub + 1)
                    Call result.Append("_{").Append(EscapeSymbol(remaining)).Append("}")
                Else
                    Call result.Append("^{").Append(EscapeSymbol(remaining)).Append("}")
                End If
            End If

            Return result.ToString()
        End Function

        Private Function EscapeSymbol(text As String) As String
            Select Case text
                Case "×" : Return "\times"
                Case "·" : Return "\cdot"
                Case "−" : Return "-"
                Case "≤" : Return "\leq"
                Case "≥" : Return "\geq"
                Case "≠" : Return "\neq"
                Case "∞" : Return "\infty"
                Case "π" : Return "\pi"
                Case "Φ" : Return "\Phi"
                Case "→" : Return "\to"
                Case "∂" : Return "\partial"
                Case Else : Return text
            End Select
        End Function

    End Module

    Friend Enum LaTeXTokenKind
        Command
        Symbol
        Text
    End Enum

    Friend NotInheritable Class LaTeXToken

        Public Property Kind As LaTeXTokenKind
        Public Property Value As String
        ''' <summary>start offset in the original latex source, used by text arguments.</summary>
        Public Property Start As Integer

    End Class

    Friend NotInheritable Class LaTeXParser

        Private ReadOnly source As String
        Private ReadOnly tokens As LaTeXToken()
        Private index As Integer

        Friend Sub New(text As String)
            Me.source = text
            Me.tokens = Tokenize(text)
        End Sub

        Friend Function ParseRoot() As MathExpression
            If tokens.Length = 0 Then
                Throw New ArgumentException("The LaTeX formula is empty.")
            End If

            Return ParseExpression()
        End Function

        Private Function ParseExpression() As MathExpression
            Dim left As MathExpression = ParseAdditive()

            Do While Current IsNot Nothing AndAlso Current.Kind = LaTeXTokenKind.Command
                Select Case Current.Value
                    Case "\leq" : left = Binary("leq", left, ParseAdditive())
                    Case "\geq" : left = Binary("geq", left, ParseAdditive())
                    Case "\neq" : left = Binary("neq", left, ParseAdditive())
                    Case Else : Exit Do
                End Select
            Loop

            Return left
        End Function

        Private Function ParseAdditive() As MathExpression
            Dim left As MathExpression = ParseMultiplicative()

            Do While Current IsNot Nothing AndAlso Not IsStop(Current)
                If Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "=" Then
                    Call Consume()
                    left = Binary("eq", left, ParseAdditive())
                ElseIf Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "+" Then
                    Call Consume()
                    left = Binary("plus", left, ParseMultiplicative())
                ElseIf Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "-" Then
                    Call Consume()
                    left = Binary("minus", left, ParseMultiplicative())
                ElseIf Current.Kind = LaTeXTokenKind.Command AndAlso Current.Value = "\oplus" Then
                    Call Consume()
                    left = Binary("oplus", left, ParseMultiplicative())
                Else
                    Exit Do
                End If
            Loop

            Return left
        End Function

        Private Function ParseMultiplicative() As MathExpression
            Dim left As MathExpression = ParsePrimary()

            Do While Current IsNot Nothing AndAlso Not IsStop(Current)
                If Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "*" Then
                    Call Consume()
                    left = Binary("times", left, ParsePrimary())
                ElseIf Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "/" Then
                    Call Consume()
                    left = Binary("divide", left, ParsePrimary())
                ElseIf Current.Kind = LaTeXTokenKind.Command AndAlso (Current.Value = "\times" OrElse Current.Value = "\cdot") Then
                    Call Consume()
                    left = Binary("times", left, ParsePrimary())
                ElseIf Current.Kind = LaTeXTokenKind.Command AndAlso (Current.Value = "\to" OrElse Current.Value = "\infty") AndAlso TypeOf left Is SymbolExpression Then
                    Dim symbol As SymbolExpression = DirectCast(left, SymbolExpression)
                    symbol.text &= If(Current.Value = "\to", "→", "∞")
                    Call Consume()
                ElseIf Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "," Then
                    ' commas inside subscripts (e.g. A_{r,i}) are literal text, not expression stops
                    Call Consume()
                    Dim tail As MathExpression = ParsePrimary()
                    left = New SymbolExpression With {.text = LaTeXMath.ToLatex(left) & "," & LaTeXMath.ToLatex(tail)}
                ElseIf CanStartPrimary(Current) Then
                    ' juxtaposition is multiplication without a visible times sign
                    left = Binary("implicit", left, ParsePrimary())
                Else
                    Exit Do
                End If
            Loop

            Return left
        End Function

        Private Function ParsePrimary() As MathExpression
            Do While Current IsNot Nothing AndAlso IsSpaceCommand(Current)
                Call Consume()
            Loop

            Dim value As MathExpression = ParseAtom()

            Do While Current IsNot Nothing AndAlso
                     (Current.Value = "^" OrElse Current.Value = "_")
                Dim separator As String = Current.Value
                Call Consume()
                value = ApplyScript(value, separator, ParseAtom())
            Loop

            Return value
        End Function

        Private Function ParseAtom() As MathExpression
            Dim token As LaTeXToken = Current

            If token Is Nothing Then
                Throw New ArgumentException("Unexpected end of LaTeX formula.")
            End If

            If token.Kind = LaTeXTokenKind.Text Then
                Call Consume()
                Return New SymbolExpression With {.text = token.Value}
            End If

            If token.Kind = LaTeXTokenKind.Command Then
                Return ParseCommand()
            End If

            Select Case token.Value
                Case "{"
                    Call Consume()

                    If Current IsNot Nothing AndAlso Current.Value = "}" Then
                        Call Consume()
                        Return New SymbolExpression With {.text = ""}
                    End If

                    Dim group As MathExpression = ParseExpression()
                    Call ExpectSymbol("}")
                    Return group
                Case "("
                    Call Consume()
                    Dim group As MathExpression = ParseExpression()
                    Call ExpectSymbol(")")
                    Return group
                Case "["
                    Call Consume()
                    Dim group As MathExpression = ParseExpression()
                    Call ExpectSymbol("]")
                    Return group
                Case "-"
                    Call Consume()
                    Return Binary("minus",
                                  New SymbolExpression With {.text = "0", .isNumericLiteral = True},
                                  ParsePrimary())
                Case Else
                    Call Consume()
                    Return New SymbolExpression With {.text = token.Value}
            End Select
        End Function

        Private Function ParseCommand() As MathExpression
            Dim command As String = Current.Value
            Call Consume()

            Select Case command
                Case "\frac", "\dfrac", "\tfrac"
                    Return Binary("divide", ParseGroupOrAtom(), ParseGroupOrAtom())
                Case "\sqrt"
                    If Current IsNot Nothing AndAlso Current.Value = "[" Then
                        Call Consume()

                        Do While Current IsNot Nothing AndAlso Current.Value <> "]"
                            Call Consume()
                        Loop

                        Call ExpectSymbol("]")
                    End If

                    Return New MathFunctionExpression With {
                        .name = "sqrt",
                        .parameters = New MathExpression() {ParseGroupOrAtom()}
                    }
                Case "\sum", "\prod", "\int", "\lim"
                    Return ParseBounded(command)
                Case "\begin"
                    Return ParseMatrix()
                Case "\sin", "\cos", "\tan", "\ln", "\log", "\exp"
                    Return New MathFunctionExpression With {
                        .name = command.Substring(1),
                        .parameters = New MathExpression() {ParseGroupOrAtom()}
                    }
                Case "\to" : Return New SymbolExpression With {.text = "→"}
                Case "\infty" : Return New SymbolExpression With {.text = "∞"}
                Case "\pi" : Return New SymbolExpression With {.text = "π"}
                Case "\Phi" : Return New SymbolExpression With {.text = "Φ"}
                Case "\times" : Return New SymbolExpression With {.text = "×"}
                Case "\cdot" : Return New SymbolExpression With {.text = "·"}
                Case "\leq" : Return New SymbolExpression With {.text = "≤"}
                Case "\geq" : Return New SymbolExpression With {.text = "≥"}
                Case "\neq" : Return New SymbolExpression With {.text = "≠"}
                Case "\partial" : Return New SymbolExpression With {.text = "∂"}
                Case "\oplus" : Return New SymbolExpression With {.text = "⊕"}
                Case "\mu" : Return New SymbolExpression With {.text = "μ"}
                Case "\alpha" : Return New SymbolExpression With {.text = "α"}
                Case "\beta" : Return New SymbolExpression With {.text = "β"}
                Case "\gamma" : Return New SymbolExpression With {.text = "γ"}
                Case "\delta" : Return New SymbolExpression With {.text = "δ"}
                Case "\epsilon" : Return New SymbolExpression With {.text = "ε"}
                Case "\theta" : Return New SymbolExpression With {.text = "θ"}
                Case "\lambda" : Return New SymbolExpression With {.text = "λ"}
                Case "\sigma" : Return New SymbolExpression With {.text = "σ"}
                Case "\omega" : Return New SymbolExpression With {.text = "ω"}
                Case "\Gamma" : Return New SymbolExpression With {.text = "Γ"}
                Case "\Delta" : Return New SymbolExpression With {.text = "Δ"}
                Case "\Theta" : Return New SymbolExpression With {.text = "Θ"}
                Case "\Lambda" : Return New SymbolExpression With {.text = "Λ"}
                Case "\Sigma" : Return New SymbolExpression With {.text = "Σ"}
                Case "\Omega" : Return New SymbolExpression With {.text = "Ω"}
                Case "\text", "\mathrm", "\textbf", "\mathit"
                    Dim text As String = ReadBracedText()

                    If text Is Nothing Then
                        Return New SymbolExpression With {.text = command.Substring(1)}
                    End If

                    Return New SymbolExpression With {.text = text.Trim()}
                Case "\end"
                    Return New SymbolExpression With {.text = ""}
                Case Else
                    Return New SymbolExpression With {.text = command.Substring(1)}
            End Select
        End Function

        Private Function ParseBounded(command As String) As MathExpression
            Dim op As String = If(command = "\lim", "limit", If(command = "\prod", "product", command.Substring(1)))
            Dim lower As MathExpression = Nothing
            Dim upper As MathExpression = Nothing

            If MatchSymbol("_") Then
                lower = ParseBoundAtom()
            End If

            If op <> "limit" AndAlso MatchSymbol("^") Then
                upper = ParseBoundAtom()
            End If

            Return New BoundedOperatorExpression With {
                .[operator] = op,
                .variable = "",
                .lower = lower,
                .upper = upper,
                .body = ParsePrimary()
            }
        End Function

        Private Function ParseMatrix() As MathExpression
            Call ExpectSymbol("{")
            Call Consume()
            Call ExpectSymbol("}")

            Dim rows As New List(Of MathExpression())
            Dim cells As New List(Of MathExpression)

            Do While Current IsNot Nothing
                If Current.Kind = LaTeXTokenKind.Command AndAlso Current.Value = "\end" Then
                    Call Consume()
                    Call ExpectSymbol("{")
                    Call Consume()
                    Call ExpectSymbol("}")
                    Exit Do
                End If

                Call cells.Add(ParseExpression())

                If MatchSymbol("&") Then
                    Continue Do
                End If

                If Current IsNot Nothing AndAlso Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = "\\" Then
                    Call Consume()
                    Call rows.Add(cells.ToArray())
                    cells.Clear()
                End If
            Loop

            If cells.Count > 0 Then
                Call rows.Add(cells.ToArray())
            End If

            Return New MatrixExpression With {.rows = rows.ToArray()}
        End Function

        Private Function ParseGroupOrAtom() As MathExpression
            If Current IsNot Nothing AndAlso Current.Value = "{" Then
                Call Consume()

                If Current IsNot Nothing AndAlso Current.Value = "}" Then
                    Call Consume()
                    Return New SymbolExpression With {.text = ""}
                End If

                Dim group As MathExpression = ParseExpression()
                Call ExpectSymbol("}")
                Return group
            End If

            Return ParsePrimary()
        End Function

        Private Function ParseBoundAtom() As MathExpression
            If Current IsNot Nothing AndAlso Current.Value = "{" Then
                Return ParseGroupOrAtom()
            End If

            Return ParseAtom()
        End Function

        ''' <summary>
        ''' Read the raw contents of the current braced group. Unlike the math expression
        ''' parser this keeps whitespace, CJK characters and operators such as ``+``
        ''' verbatim, which is required by ``\text{...}`` arguments.
        ''' </summary>
        Private Function ReadBracedText() As String
            If Current Is Nothing OrElse Current.Value <> "{" Then
                Return Nothing
            End If

            Dim open As LaTeXToken = Consume()
            Dim depth As Integer = 1
            Dim closeIndex As Integer = -1

            While index < tokens.Length
                Dim token As LaTeXToken = tokens(index)

                If token.Value = "{" Then
                    depth += 1
                ElseIf token.Value = "}" Then
                    depth -= 1

                    If depth = 0 Then
                        closeIndex = token.Start
                        index += 1
                        Exit While
                    End If
                End If

                index += 1
            End While

            If closeIndex < 0 Then
                Throw New ArgumentException("Unclosed text argument in LaTeX formula.")
            End If

            Return source.Substring(open.Start + 1, closeIndex - open.Start - 1)
        End Function

        Private Function ApplyScript(value As MathExpression, separator As String, script As MathExpression) As MathExpression
            Dim scriptText As String = LaTeXMath.ToLatex(script)
            Dim scriptToken As String = If(scriptText.Length = 1, scriptText, "{" & scriptText & "}")

            If TypeOf value Is SymbolExpression Then
                Dim symbol As SymbolExpression = DirectCast(value, SymbolExpression)
                symbol.text &= separator & scriptText
                Return symbol
            ElseIf separator = "^" Then
                Return Binary("power", value, script)
            Else
                Return New SymbolExpression With {.text = "{" & LaTeXMath.ToLatex(value) & "}_" & scriptToken}
            End If
        End Function

        Private Shared Function Binary(op As String, left As MathExpression, right As MathExpression) As BinaryExpression
            Return New BinaryExpression With {
                .[operator] = op,
                .applyleft = left,
                .applyright = right
            }
        End Function

        Private ReadOnly Property Current As LaTeXToken
            Get
                If index >= tokens.Length Then
                    Return Nothing
                End If

                Return tokens(index)
            End Get
        End Property

        Private Function Consume() As LaTeXToken
            Dim token As LaTeXToken = Current
            index += 1
            Return token
        End Function

        Private Function MatchSymbol(value As String) As Boolean
            If Current IsNot Nothing AndAlso Current.Kind = LaTeXTokenKind.Symbol AndAlso Current.Value = value Then
                Call Consume()
                Return True
            End If

            Return False
        End Function

        Private Sub ExpectSymbol(value As String)
            If Not MatchSymbol(value) Then
                Throw New ArgumentException($"Expected '{value}' in LaTeX formula.")
            End If
        End Sub

        Private Shared Function IsStop(token As LaTeXToken) As Boolean
            If token.Kind = LaTeXTokenKind.Symbol Then
                Return "}&])".IndexOf(token.Value) >= 0 OrElse token.Value = "\\"
            End If

            Return token.Kind = LaTeXTokenKind.Command AndAlso token.Value = "\end"
        End Function

        Private Shared Function CanStartPrimary(token As LaTeXToken) As Boolean
            If token.Kind = LaTeXTokenKind.Text Then
                Return True
            End If

            If token.Kind = LaTeXTokenKind.Command Then
                Return token.Value <> "\end" AndAlso token.Value <> "\oplus"
            End If

            Return token.Value = "{" OrElse token.Value = "(" OrElse token.Value = "["
        End Function

        Private Shared Function IsSpaceCommand(token As LaTeXToken) As Boolean
            Return token.Kind = LaTeXTokenKind.Command AndAlso
                   (token.Value = "\ " OrElse token.Value = "\," OrElse token.Value = "\;" OrElse
                    token.Value = "\:" OrElse token.Value = "\!" OrElse token.Value = "\quad")
        End Function

        Private Shared Function IsDelimiter(ch As Char) As Boolean
            Return ch = "\"c OrElse "{}[]()+-*/=^_&,".IndexOf(ch) >= 0
        End Function

        Private Shared Function Tokenize(text As String) As LaTeXToken()
            Dim result As New List(Of LaTeXToken)
            Dim i As Integer = 0

            Do While i < text.Length
                Dim ch As Char = text(i)
                Dim start As Integer = i

                If Char.IsWhiteSpace(ch) Then
                    i += 1
                    Continue Do
                End If

                If ch = "\"c Then
                    If i + 1 < text.Length AndAlso text(i + 1) = "\"c Then
                        Call result.Add(New LaTeXToken With {.Kind = LaTeXTokenKind.Symbol, .Value = "\\", .Start = start})
                        i += 2
                    ElseIf i + 1 < text.Length AndAlso Not Char.IsLetter(text(i + 1)) Then
                        Call result.Add(New LaTeXToken With {.Kind = LaTeXTokenKind.Command, .Value = text.Substring(i, 2), .Start = start})
                        i += 2
                    Else
                        Dim j As Integer = i + 1

                        Do While j < text.Length AndAlso Char.IsLetter(text(j))
                            j += 1
                        Loop

                        Call result.Add(New LaTeXToken With {.Kind = LaTeXTokenKind.Command, .Value = text.Substring(i, j - i), .Start = start})
                        i = j
                    End If
                ElseIf IsDelimiter(ch) Then
                    Call result.Add(New LaTeXToken With {.Kind = LaTeXTokenKind.Symbol, .Value = ch.ToString, .Start = start})
                    i += 1
                Else
                    Dim j As Integer = i

                    Do While j < text.Length AndAlso Not Char.IsWhiteSpace(text(j)) AndAlso Not IsDelimiter(text(j))
                        j += 1
                    Loop

                    If j = i Then
                        j += 1
                    End If

                    Call result.Add(New LaTeXToken With {.Kind = LaTeXTokenKind.Text, .Value = text.Substring(i, j - i), .Start = start})
                    i = j
                End If
            Loop

            Return result.ToArray()
        End Function

    End Class
End Namespace
