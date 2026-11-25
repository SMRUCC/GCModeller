Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports FormulaFont = Microsoft.VisualBasic.Imaging.Font
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    Friend Module FormulaLayoutMath

        Friend Function Max3(a As Single, b As Single, c As Single) As Single
            Return System.Math.Max(System.Math.Max(a, b), c)
        End Function

    End Module

    ''' <summary>
    ''' Large operator symbol with upper/lower limits, either stacked or placed to the side.
    ''' </summary>
    Friend NotInheritable Class LimitsFormulaBox : Inherits FormulaBox

        Private ReadOnly symbol As FormulaBox
        Private ReadOnly upper As FormulaBox
        Private ReadOnly lower As FormulaBox
        Private ReadOnly symbolX As Single
        Private ReadOnly symbolY As Single
        Private ReadOnly upperX As Single
        Private ReadOnly upperY As Single
        Private ReadOnly lowerX As Single
        Private ReadOnly lowerY As Single

        Friend Sub New(symbol As FormulaBox, upper As FormulaBox, lower As FormulaBox, side As Boolean, style As FormulaRenderStyle)
            Me.symbol = symbol
            Me.upper = upper
            Me.lower = lower

            Dim gap As Single = style.ScriptGap
            Dim upperWidth As Single = If(upper Is Nothing, 0F, upper.Width)
            Dim lowerWidth As Single = If(lower Is Nothing, 0F, lower.Width)
            Dim upperHeight As Single = If(upper Is Nothing, 0F, upper.Height)
            Dim lowerHeight As Single = If(lower Is Nothing, 0F, lower.Height)

            If side Then
                Dim scriptWidth As Single = System.Math.Max(upperWidth, lowerWidth)
                Dim scriptHeight As Single = upperHeight +
                                             If(upper IsNot Nothing AndAlso lower IsNot Nothing, gap, 0F) +
                                             lowerHeight
                Dim height As Single = System.Math.Max(symbol.Height, scriptHeight)
                Dim ascent As Single = System.Math.Max(symbol.Ascent, If(upper Is Nothing, 0F, upper.Ascent))
                Dim scriptTop As Single = ascent - upperHeight

                Me.symbolX = 0F
                Me.symbolY = ascent - symbol.Ascent
                Me.upperX = symbol.Width + gap + (scriptWidth - upperWidth) / 2F
                Me.upperY = scriptTop
                Me.lowerX = symbol.Width + gap + (scriptWidth - lowerWidth) / 2F
                Me.lowerY = If(upper Is Nothing, scriptTop, scriptTop + upperHeight + gap)

                Call SetMetrics(symbol.Width + gap + scriptWidth, height, ascent)
            Else
                Dim width As Single = FormulaLayoutMath.Max3(symbol.Width, upperWidth, lowerWidth)
                Dim symbolTop As Single = If(upper Is Nothing, 0F, upperHeight + gap)
                Dim lowerTop As Single = symbolTop + symbol.Height + If(lower Is Nothing, 0F, gap)
                Dim height As Single = lowerTop + lowerHeight
                Dim ascent As Single = symbolTop + symbol.Ascent

                Me.upperX = (width - upperWidth) / 2F
                Me.upperY = 0F
                Me.symbolX = (width - symbol.Width) / 2F
                Me.symbolY = symbolTop
                Me.lowerX = (width - lowerWidth) / 2F
                Me.lowerY = lowerTop

                Call SetMetrics(width, height, ascent)
            End If
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            If upper IsNot Nothing Then
                Call upper.Render(device, style, x + upperX, y + upperY)
            End If

            Call symbol.Render(device, style, x + symbolX, y + symbolY)

            If lower IsNot Nothing Then
                Call lower.Render(device, style, x + lowerX, y + lowerY)
            End If
        End Sub

    End Class

    ''' <summary>
    ''' Matrix layout with bracket delimiters and aligned cells.
    ''' </summary>
    Friend NotInheritable Class MatrixFormulaBox : Inherits FormulaBox

        Private ReadOnly rows As FormulaBox()()
        Private ReadOnly columnWidths As Single()
        Private ReadOnly rowHeights As Single()
        Private ReadOnly cellGap As Single
        Private ReadOnly bracketGap As Single
        Private ReadOnly bracketWidth As Single
        Private ReadOnly contentWidth As Single
        Private ReadOnly contentHeight As Single

        Friend Sub New(rows As FormulaBox()(), style As FormulaRenderStyle)
            Me.rows = rows
            Me.cellGap = style.MatrixCellGap
            Me.bracketGap = style.MatrixBracketGap
            Me.bracketWidth = style.MatrixBracketWidth

            Dim columnCount As Integer = 0

            If rows IsNot Nothing Then
                For Each row As FormulaBox() In rows
                    If row IsNot Nothing Then
                        columnCount = System.Math.Max(columnCount, row.Length)
                    End If
                Next
            End If

            ReDim columnWidths(System.Math.Max(0, columnCount - 1))
            ReDim rowHeights(System.Math.Max(0, If(rows Is Nothing, 0, rows.Length) - 1))

            If rows IsNot Nothing Then
                For i As Integer = 0 To rows.Length - 1
                    Dim row As FormulaBox() = rows(i)

                    If row Is Nothing Then
                        Continue For
                    End If

                    rowHeights(i) = 0F

                    For j As Integer = 0 To row.Length - 1
                        If row(j) IsNot Nothing Then
                            rowHeights(i) = System.Math.Max(rowHeights(i), row(j).Height)
                            columnWidths(j) = System.Math.Max(columnWidths(j), row(j).Width)
                        End If
                    Next
                Next
            End If

            contentWidth = columnWidths.Sum

            If columnWidths.Length > 1 Then
                contentWidth += cellGap * (columnWidths.Length - 1)
            End If

            contentHeight = rowHeights.Sum

            If rowHeights.Length > 1 Then
                contentHeight += cellGap * (rowHeights.Length - 1)
            End If

            Dim padding As Single = 2F
            Dim ascent As Single = contentHeight / 2F + padding
            Call SetMetrics(contentWidth + (bracketGap + bracketWidth) * 2F,
                            ascent + contentHeight / 2F + padding,
                            ascent)
        End Sub

        Friend Overrides Sub Render(device As IGraphics, style As FormulaRenderStyle, x As Single, y As Single)
            Dim top As Single = y + 2F
            Dim bottom As Single = y + Height - 2F
            Dim leftOuter As Single = x
            Dim leftInner As Single = x + bracketWidth
            Dim rightInner As Single = x + Width - bracketWidth
            Dim rightOuter As Single = x + Width

            Call device.DrawLine(style.FractionRule, leftOuter, top, leftOuter, bottom)
            Call device.DrawLine(style.FractionRule, leftOuter, top, leftInner, top)
            Call device.DrawLine(style.FractionRule, leftOuter, bottom, leftInner, bottom)
            Call device.DrawLine(style.FractionRule, rightOuter, top, rightOuter, bottom)
            Call device.DrawLine(style.FractionRule, rightInner, top, rightOuter, top)
            Call device.DrawLine(style.FractionRule, rightInner, bottom, rightOuter, bottom)

            If rows Is Nothing Then
                Return
            End If

            Dim cellTop As Single = y + Ascent - contentHeight / 2F

            For i As Integer = 0 To rows.Length - 1
                Dim row As FormulaBox() = rows(i)
                Dim cellLeft As Single = x + bracketWidth + bracketGap

                For j As Integer = 0 To columnWidths.Length - 1
                    If row IsNot Nothing AndAlso j < row.Length AndAlso row(j) IsNot Nothing Then
                        Dim cell As FormulaBox = row(j)
                        Call cell.Render(device, style,
                                         cellLeft + (columnWidths(j) - cell.Width) / 2F,
                                         cellTop + (rowHeights(i) - cell.Height) / 2F)
                    End If

                    cellLeft += columnWidths(j) + cellGap
                Next

                cellTop += rowHeights(i) + cellGap
            Next
        End Sub

    End Class

    Friend NotInheritable Partial Class FormulaLayoutContext

        Friend Function LayoutBoundedOperator(expression As BoundedOperatorExpression) As FormulaBox
            Dim op As String = If(expression.[operator], "")

            If op = "diff" OrElse op = "partialdiff" Then
                Return LayoutDerivative(expression)
            End If

            Dim symbolText As String = "∑"

            Select Case op
                Case "product" : symbolText = "∏"
                Case "int", "integral" : symbolText = "∫"
                Case "limit" : symbolText = "lim"
                Case Else : symbolText = "∑"
            End Select

            Dim symbolFont As FormulaFont = If(op = "limit", Style.Font, Style.CreateBigOperatorFont())
            Dim symbol As FormulaBox = CreateText(symbolText, symbolFont)
            Dim lower As FormulaBox = If(expression.lower Is Nothing, Nothing, LayoutScript(expression.lower))
            Dim upper As FormulaBox = If(expression.upper Is Nothing, Nothing, LayoutScript(expression.upper))
            Dim side As Boolean = (op = "int" OrElse op = "integral")
            Dim limits As New LimitsFormulaBox(symbol, upper, lower, side, Style)

            Return New HorizontalFormulaBox({limits, Layout(expression.body)}, Style.OperatorGap)
        End Function

        Friend Function LayoutMatrix(expression As MatrixExpression) As FormulaBox
            If expression.rows Is Nothing Then
                Return New MatrixFormulaBox(New FormulaBox()() {}, Style)
            End If

            Dim boxes As New List(Of FormulaBox())

            For Each row As MathExpression() In expression.rows
                If row Is Nothing Then
                    Call boxes.Add(New FormulaBox() {})
                Else
                    Call boxes.Add(row.Select(Function(cell) Layout(cell)).ToArray)
                End If
            Next

            Return New MatrixFormulaBox(boxes.ToArray(), Style)
        End Function

        Private Function LayoutDerivative(expression As BoundedOperatorExpression) As FormulaBox
            Dim op As String = If(expression.[operator], "diff")
            Dim symbolText As String = If(op = "partialdiff", "∂", "d")
            Dim numerator As FormulaBox = CreateText(symbolText)
            Dim variableBox As FormulaBox = CreateText(If(expression.variable, ""))

            If expression.degree IsNot Nothing Then
                numerator = New ScriptFormulaBox(numerator, LayoutScript(expression.degree), False, Style)
                variableBox = New ScriptFormulaBox(variableBox, LayoutScript(expression.degree), False, Style)
            End If

            Dim denominator As New HorizontalFormulaBox({CreateText(symbolText), variableBox}, 0F)
            Dim derivative As New FractionFormulaBox(numerator, denominator, Style)
            Dim body As FormulaBox = Layout(expression.body)

            Return New HorizontalFormulaBox({derivative, body}, Style.OperatorGap)
        End Function

    End Class
End Namespace
