Imports Microsoft.VisualBasic.ComponentModel.Collection

Module TypeScriptSymbols

    ''' <summary>
    ''' The typescript keywords
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Keywords As Index(Of String) = {
        "declare", "namespace", "module", "function", "void",
        "interface", "protected", "constructor", "extends",
        "private", "public", "export", "static"
    }

End Module
