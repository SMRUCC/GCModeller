Imports MathLambda
Imports Microsoft.VisualBasic.MIME.application.xml.MathML

Module Module1

    Sub Main()
        Dim exp As LambdaExpression = LambdaExpression.FromMathML("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Mathematica\Math\MathLambda\mathML.xml".ReadAllText)
        Dim func = Compiler.CreateLambda(exp)
        Dim del = func.Compile

        Pause()
    End Sub

End Module
