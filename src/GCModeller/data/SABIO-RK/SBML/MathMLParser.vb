Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Namespace SBML

    Public Module MathMLParser

        Const startTag As String = "<listOfFunctionDefinitions"
        Const functionMLPattern As String = "<functionDefinition.+?</functionDefinition>"

        Public Iterator Function ParseMathML(sbmlText As String) As IEnumerable(Of NamedValue(Of XmlElement))
            Dim start = sbmlText.IndexOf(startTag)
            Dim ends = sbmlText.IndexOf("</listOfFunctionDefinitions>")

            sbmlText = sbmlText _
                .Substring(start + startTag.Length + 1, ends - start - startTag.Length - 1) _
                .Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF)

            Dim functions As String() = r _
                .Matches(sbmlText, functionMLPattern, RegexICSng) _
                .ToArray
            Dim xml As XmlElement
            Dim id As String
            Dim term As String
            Dim math As XmlElement

            For Each func As String In functions
                xml = XmlElement.ParseXmlText(func)
                id = xml.attributes("id")
                term = xml.attributes.TryGetValue("sboTerm")
                math = xml.getElementsByTagName("math").First

                Yield New NamedValue(Of XmlElement) With {
                    .Name = id,
                    .Description = term,
                    .Value = math
                }
            Next
        End Function
    End Module
End Namespace