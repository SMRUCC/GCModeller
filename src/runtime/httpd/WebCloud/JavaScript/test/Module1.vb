Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Parser

Module Module1

    Sub Main()
        Dim cssFile = "C:\Users\administrator\Desktop\fontawesome.css"
        Dim css = CssParser.GetTagWithCSS(cssFile.ReadAllText)


        Pause()
    End Sub
End Module
