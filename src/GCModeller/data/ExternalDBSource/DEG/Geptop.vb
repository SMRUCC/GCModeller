Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Data

Public Class Geptop : Inherits XmlDataModel
    Implements Enumeration(Of Gene)

    Public Class Gene
        <XmlAttribute> Public Property [class] As Integer
        <XmlAttribute> Public Property essentialityScore As Double
        <XmlAttribute> Public Property protein As String
    End Class

    Public Property genome As String

    <XmlElement("gene")>
    Public Property genes As Gene()

    Public Shared Function ParseFromOutput(output As String) As Geptop
        Dim lines = output.SolveStream _
            .LineTokens _
            .Where(Function(l) Strings.Trim(l).First <> "#"c) _
            .Skip(1) _
            .ToArray

        Return lines _
            .Select(Function(line)
                        Dim tokens = line.Split(ASCII.TAB)

                        Return New Geptop.Gene With {
                            .[class] = Val(tokens(0)),
                            .essentialityScore = Val(tokens(1)),
                            .protein = tokens(2)
                        }
                    End Function) _
            .ToArray
    End Function

    Const listAPI$ = "http://cefg.uestc.cn/geptop/list.html"

    Public Shared Function GetGenomeList() As NamedValue(Of String)()
        Dim html As String = listAPI.GET
        Dim list = html.Matches("<li.+?</li>").ToArray
        Dim genomes = list _
            .Select(Function(li)
                        Dim link = li.href
                        Dim name = li.StripHTMLTags

                        Return New NamedValue(Of String) With {
                            .Name = name,
                            .Value = link
                        }
                    End Function) _
            .ToArray

        Return genomes
    End Function

    Public Shared Sub FetchData(save$)
        Dim web As New WebQuery(Of NamedValue(Of String))(Function(li) li.Value, Function(li) li.Name, Function(s, t) Geptop.ParseFromOutput(s), $"{save}/.geptop")

        For Each genome As NamedValue(Of String) In GetGenomeList()
            Dim result As Geptop = web.Query(Of Geptop)(genome, "*.html")
            result.genome = genome.Name
            result.GetXml.SaveTo($"{save}/{genome.Name.NormalizePathString}.Xml")
        Next
    End Sub

    Public Shared Widening Operator CType(genes As Gene()) As Geptop
        Return New Geptop With {.genes = genes}
    End Operator

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Gene) Implements Enumeration(Of Gene).GenericEnumerator
        For Each gene As Gene In genes
            Yield gene
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Gene).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
