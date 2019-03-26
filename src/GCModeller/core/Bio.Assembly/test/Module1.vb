Imports r = System.Text.RegularExpressions.Regex

Module Module1

    Sub Main()
        Dim xml = "P:\.iGEM\BBa_K1188002.Xml".ReadAllText

        xml = r.Replace(xml, "<![-][-].*[-][-]>", "")

        Pause()
    End Sub
End Module
