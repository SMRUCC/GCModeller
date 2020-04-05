Namespace MarkupCompiler

    Public MustInherit Class CompilerWorkflow

        Protected ReadOnly compiler As v2MarkupCompiler

        Sub New(compiler As v2MarkupCompiler)
            Me.compiler = compiler
        End Sub
    End Class
End Namespace