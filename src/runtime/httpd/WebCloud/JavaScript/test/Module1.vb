Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Parser

Module Module1

    Sub Main()
        Dim cssFile = "C:\Users\administrator\Desktop\fontawesome.css"
        Dim css As CSSFile = CssParser.GetTagWithCSS(cssFile.ReadAllText, selectorFilter:="\.fa[-]")
        Dim icons = css.Selectors.Values.Where(Function(s) s.HasProperty("content")).ToDictionary

        Pause()
    End Sub
End Module
