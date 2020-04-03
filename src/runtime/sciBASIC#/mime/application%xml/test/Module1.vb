Imports Microsoft.VisualBasic.MIME.application.xml.MathML

Module Module1

    Sub Main()
        Dim test = "D:\GCModeller\src\runtime\sciBASIC#\Data_science\Mathematica\Math\MathLambda\mathML.xml"
        Dim xml = Microsoft.VisualBasic.MIME.application.xml.XmlParser.ParseXml(test.ReadAllText)
        Dim exp As BinaryExpression = BinaryExpression.FromMathML(test.ReadAllText)

        Pause()
    End Sub

End Module
