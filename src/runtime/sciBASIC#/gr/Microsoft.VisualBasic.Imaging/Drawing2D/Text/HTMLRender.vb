Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.Markup.HTML
Imports Microsoft.VisualBasic.Text

Namespace Drawing2D.Text

    ''' <summary>
    ''' 进行简单的HTML片段的渲染
    ''' </summary>
    Public Module HTMLRender

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="html"></param>
        ''' <param name="topleft">程序会以这个位置为原点进行布局的计算操作</param>
        <Extension>
        Public Sub RenderHTML(g As IGraphics, html As TextString(), Optional topleft As Point = Nothing)
            Dim size As SizeF

            For Each fragment As TextString In html

                ' 已经在这里处理了topleft的更新了
                Select Case fragment.weight
                    Case TextString.WeightStyles.sub
                        size = g.drawSub(fragment, topleft)
                    Case TextString.WeightStyles.sup
                        size = g.drawSup(fragment, topleft)
                    Case Else
                        size = g.drawNormal(fragment, topleft)
                End Select
            Next
        End Sub

        <Extension>
        Private Function drawNormal(g As IGraphics, str As TextString, ByRef topleft As Point) As SizeF
            Dim font As Font = str.font
            Dim color As New SolidBrush(str.color.TranslateColor)
            Dim size As SizeF
            Dim hasLines As Integer = 0
            Dim offsetY%
            Dim p As Point

            For Each line As String In str.text.LineTokens
                size = g.MeasureString(line, font)
                p = New Point With {
                    .X = topleft.X,
                    .Y = topleft.Y + offsetY
                }
                g.DrawString(line, font, color, p)
                hasLines += 1
                offsetY += size.Height
            Next

            If hasLines = 1 Then
                ' 没有换行, 则
                ' X前进字符串的宽度
                ' Y不变
                topleft = New Point With {
                    .X = topleft.X + size.Width,
                    .Y = topleft.Y
                }
            End If

            Return g.MeasureString(str, font)
        End Function

        <Extension>
        Private Function drawSub(g As IGraphics, text As TextString, ByRef topleft As Point) As SizeF
            Dim font As New Font(text.font.Name, text.font.Size / 3)
            Dim size As SizeF = g.MeasureString(text.text, font)
            Dim color As New SolidBrush(text.color.TranslateColor)

            g.DrawString(
                text, font, color, New Point With {
                    .X = topleft.X,
                    .Y = topleft.Y + size.Height * 2.5
            })
            topleft = New Point With {
                .X = topleft.X + size.Width,
                .Y = topleft.Y
            }

            Return size
        End Function

        <Extension>
        Private Function drawSup(g As IGraphics, text As TextString, ByRef topleft As Point) As SizeF
            Dim font As New Font(text.font.Name, text.font.Size / 3)
            Dim size As SizeF = g.MeasureString(text.text, font)
            Dim color As New SolidBrush(text.color.TranslateColor)

            g.DrawString(
                text, font, color, New Point With {
                    .X = topleft.X,
                    .Y = topleft.Y - size.Height / 2
            })
            topleft = New Point With {
                .X = topleft.X + size.Width,
                .Y = topleft.Y
            }

            Return size
        End Function
    End Module
End Namespace