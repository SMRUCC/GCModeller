Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Language.C

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

        Private disposedValue As Boolean

        Sub New(fp As StreamWriter)
            Me.fp = fp
        End Sub

        Public Sub linewidth(width As Single)
            fprintf(fp, "%f setlinewidth\n", width)
        End Sub

        Public Sub color(r!, g!, b!)
            fprintf(fp, "%3.2f %3.2f %3.2f setrgbcolor\n", r, g, b)
        End Sub

        Public Sub font(name As String, fontsize!)
            fprintf(fp, "/%s findfont %f scalefont setfont\n", name, fontsize)
        End Sub

        Public Sub note(noteText As String)
            fprintf(fp, "%% %s\n", noteText)
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

