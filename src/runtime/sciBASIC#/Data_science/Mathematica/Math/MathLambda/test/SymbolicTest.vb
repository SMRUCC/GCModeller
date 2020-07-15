Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Lambda
Imports Microsoft.VisualBasic.Math.Scripting

Module SymbolicTest

    Sub Main()
        Dim symbols = ScriptEngine.ParseExpression("(5+5) * (2*x + x / 5 + 1)").DoCall(AddressOf Symbolic.Simplify)

        Console.WriteLine(symbols)

        Pause()
    End Sub
End Module
