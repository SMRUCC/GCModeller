Imports System.Drawing
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports GraphicsUnit = Microsoft.VisualBasic.Imaging.GraphicsUnit
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    ''' <summary>
    ''' Style settings shared by the MathML formula layout and rendering code.
    ''' </summary>
    Public NotInheritable Class FormulaRenderStyle

        Public Property Font As Font
        Public Property Foreground As Brush
        Public Property FractionRule As Pen
        Public Property ScriptScale As Single = 0.68F
        Public Property BigOperatorScale As Single = 1.45F
        Public Property OperatorGap As Single = 4F
        Public Property ScriptGap As Single = 2F
        Public Property FractionGap As Single = 5F
        Public Property FractionPadding As Single = 6F
        Public Property BracketGap As Single = 2F
        Public Property MatrixCellGap As Single = 8F
        Public Property MatrixBracketGap As Single = 6F
        Public Property MatrixBracketWidth As Single = 8F

        Public Sub New()
            Me.New(New Font("Cambria Math", 36),
                   New SolidBrush(Color.Black),
                   New Pen(Color.Black, 1.5F))
        End Sub

        Public Sub New(font As Font, foreground As Brush, Optional fractionRule As Pen = Nothing)
            If font Is Nothing Then Throw New ArgumentNullException(NameOf(font))
            If foreground Is Nothing Then Throw New ArgumentNullException(NameOf(foreground))

            Me.Font = font
            Me.Foreground = foreground
            Me.FractionRule = If(fractionRule, New Pen(Color.Black, 1.5F))
        End Sub

        Friend Function CreateScriptFont(base As Font) As Font
            Dim scriptSize As Single = System.Math.Max(6F, base.Size * ScriptScale)
            Return New Font(base.Name, scriptSize, base.Style, base.Unit)
        End Function

        Friend Function CreateBigOperatorFont() As Font
            Dim symbolSize As Single = System.Math.Max(12F, Font.Size * BigOperatorScale)
            Return New Font(Font.Name, symbolSize, Font.Style, Font.Unit)
        End Function

        Friend Function CreateScriptStyle() As FormulaRenderStyle
            Return New FormulaRenderStyle(CreateScriptFont(Font), Foreground, FractionRule) With {
                .ScriptScale = ScriptScale,
                .BigOperatorScale = BigOperatorScale,
                .OperatorGap = OperatorGap * ScriptScale,
                .ScriptGap = ScriptGap * ScriptScale,
                .FractionGap = FractionGap * ScriptScale,
                .FractionPadding = FractionPadding * ScriptScale,
                .BracketGap = BracketGap * ScriptScale,
                .MatrixCellGap = MatrixCellGap * ScriptScale,
                .MatrixBracketGap = MatrixBracketGap * ScriptScale,
                .MatrixBracketWidth = MatrixBracketWidth * ScriptScale
            }
        End Function

    End Class
End Namespace
