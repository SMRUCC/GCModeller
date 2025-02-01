Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace PostScript

    ''' <summary>
    ''' abstract model of the painting elements
    ''' </summary>
    Public MustInherit Class PSElement

        Friend MustOverride Sub WriteAscii(ps As Writer)
        Friend MustOverride Sub Paint(g As IGraphics)

    End Class

    ''' <summary>
    ''' An postscript graphics element
    ''' </summary>
    Public MustInherit Class PSElement(Of S As Shape) : Inherits PSElement

        Public Property shape As S

    End Class

    Public Class Writer : Implements IDisposable

        ReadOnly fp As StreamWriter
        ReadOnly css As CSSEnvirnment

        Private disposedValue As Boolean

        Sub New(fp As StreamWriter, css As CSSEnvirnment)
            Me.fp = fp
            Me.css = css
        End Sub

        Public Function pen(stroke As Stroke) As Pen
            Return css.GetPen(stroke)
        End Function

        Public Sub moveto(x!, y!)
            fprintf(fp, "%f %f moveto\n", x, y)
        End Sub

        Public Sub text(s As String, x!, y!)
            fprintf(fp, "%f %f moveto (%s) show\n", x, y, s)
        End Sub

        Public Sub line(x1!, y1!, x2!, y2!)
            fprintf(fp, "newpath\n %f %f moveto\n %f %f lineto\n stroke\n", x1, y1, x2, y2)
        End Sub

        Public Sub circle(center As PointF, radius As Single)
            fprintf(fp, "%f %f %f 0 360 arc closepath\n", center.X, center.Y, radius)
            fprintf(fp, "stroke\n")
        End Sub

        Public Sub rectangle(rect As RectangleF, fill As Boolean, stroke As Boolean)
            Dim x1 = rect.X, y1 = rect.Y
            Dim x2 = x1 + rect.Width
            Dim y2 = y1 + rect.Height

            fprintf(fp, "newpath %f %f moveto ", x1, y1)
            fprintf(fp, "%f %f lineto ", x2, y1)
            fprintf(fp, "%f %f lineto ", x2, y2)
            fprintf(fp, "%f %f lineto ", x1, y2)
            fprintf(fp, "%f %f lineto ", x1, y1)

            If fill Then
                If stroke Then
                    fprintf(fp, "closepath gsave fill grestore stroke\n")
                Else
                    fprintf(fp, "closepath fill\n")
                End If
            End If

            fprintf(fp, "closepath stroke\n")
        End Sub

        Public Sub dash(dash As Integer())
            If dash.IsNullOrEmpty Then
                fprintf(fp, "[] 0 setdash\n")
            Else
                fprintf(fp, "[%s %s] 0 setdash", dash(0), dash(1))
            End If
        End Sub

        Public Sub linewidth(width As Single)
            fprintf(fp, "%f setlinewidth\n", width)
        End Sub

        Public Sub color(r!, g!, b!)
            fprintf(fp, "%3.2f %3.2f %3.2f setrgbcolor\n", r, g, b)
        End Sub

        Public Sub color(color As Color)
            fprintf(fp, "%3.2f %3.2f %3.2f setrgbcolor\n", color.R / 255, color.G / 255, color.B / 255)
        End Sub

        Public Sub transparency(ca As Single)
            fprintf(fp, "<< /CA %f /ca %f >> /TransparentState exch def\n", ca, ca)
        End Sub

        Public Sub beginTransparent()
            fprintf(fp, "TransparentState gs\n")
        End Sub

        Public Sub endTransparent()
            fprintf(fp, "gr\n")
        End Sub

        Public Sub font(font As CSSFont)
            With css.GetFont(font)
                Call Me.font(.Name, .Size)
            End With
        End Sub

        Public Sub font(name As String, fontsize!)
            fprintf(fp, "/%s findfont\n %f scalefont\n setfont\n", name, fontsize)
        End Sub

        Public Sub note(noteText As String)
            For Each line As String In noteText.LineTokens
                Call fprintf(fp, "%% %s\n", line)
            Next
        End Sub

        ''' <summary>
        ''' mark the end of current page
        ''' </summary>
        Public Sub showpage()
            Call fp.WriteLine("showpage")
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call fp.Flush()
                    Call fp.Close()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace

