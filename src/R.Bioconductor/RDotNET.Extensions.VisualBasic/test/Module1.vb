Public Module Module1

    Sub Main()
        Dim type = GetType(Module1)

        test2("3333", 33)

        Pause()
    End Sub

    Public Declare Function test2 Lib "stat" Alias "invoke.test" (args$, int%) As Boolean

End Module
