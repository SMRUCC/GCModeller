Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.Markup.HTML

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

        End Sub
    End Module
End Namespace