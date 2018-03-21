Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Parser
Imports SMRUCC.WebCloud.JavaScript.FontAwesome

Module Module1

    Sub Main()
        Dim cssFile = "C:\Users\administrator\Desktop\fontawesome.css"
        Dim code = VBScript.FromCSS(fontawesome:=cssFile)

        Call code.SaveTo("D:\GCModeller\src\runtime\httpd\WebCloud\JavaScript\FontAwesome\Icons.vb")

        Pause()
    End Sub
End Module
