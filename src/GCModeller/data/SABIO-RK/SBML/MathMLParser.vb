Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace SBML

    Public Module MathMLParser

        Public Iterator Function ParseMathML(sbmlText As String) As IEnumerable(Of NamedValue(Of XmlElement))
            Dim start = sbmlText.IndexOf("<listOfFunctionDefinitions")
            Dim ends = sbmlText.IndexOf("</listOfFunctionDefinitions>")

            sbmlText = sbmlText.Substring(start + 27, ends - start)
        End Function
    End Module
End Namespace