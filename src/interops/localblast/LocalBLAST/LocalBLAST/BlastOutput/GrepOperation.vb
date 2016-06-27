Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput

    Public Class GrepOperation

        Dim _query, _subject As TextGrepMethod

        Sub New(Script As String)
            Dim Engine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Script)
            _query = Engine.Method
            _subject = Engine.Method
        End Sub

        Sub New(Query As String, Subject As String)
            _query = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Query).Method
            _subject = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Subject).Method
        End Sub

        Sub New(Query As TextGrepMethod, Subject As TextGrepMethod)
            _query = Query
            _subject = Subject
        End Sub

        Public Function Grep(Of Log As IBlastOutput)(LogOutput As Log) As Log
            Call LogOutput.Grep(_query, _subject)
            Return LogOutput
        End Function
    End Class
End Namespace