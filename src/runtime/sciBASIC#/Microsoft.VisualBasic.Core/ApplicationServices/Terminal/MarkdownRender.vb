Namespace ApplicationServices.Terminal

    ''' <summary>
    ''' A simple markdown render on console
    ''' </summary>
    ''' <remarks>
    ''' 主要是渲染下面的一些元素:
    ''' 
    ''' + code: 红色
    ''' + url: 蓝色
    ''' + blockquote: 灰色背景色
    ''' </remarks>
    Public Module MarkdownRender

        Public Sub Print(markdown As String)

        End Sub
    End Module

    Public Class MarkdownTheme

        Public Property ForeColor As ConsoleColor = ConsoleColor.White
        Public Property BackgroundColor As ConsoleColor = ConsoleColor.Black


    End Class
End Namespace