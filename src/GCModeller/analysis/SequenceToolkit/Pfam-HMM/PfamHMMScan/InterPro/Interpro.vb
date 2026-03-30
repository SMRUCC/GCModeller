Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace InterPro.Xml

    <XmlType("interpro")>
    Public Class Interpro : Inherits LLMDocument
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property short_name As String
        <XmlAttribute> Public Property type As String

        Public Property name As String
        Public Property abstract As abstract
        Public Property pub_list As Publication()
        Public Property parent_list As RelRef()
        Public Property contains As RelRef()
        Public Property found_in As RelRef()
        Public Property member_list As db_xref()
        Public Property external_doc_list As db_xref()
        Public Property structure_db_links As db_xref()
        Public Property taxonomy_distribution As TaxonData()
        Public Property sec_list As SecAcc()

        Public Overrides Function ToString() As String
            Return $"[{type}] {short_name}   {name}"
        End Function
    End Class

    Public Class abstract : Inherits LLMDocument

        <XmlText> Public Property html As String

        Public Shared Function CleanText(text As String) As String
            Dim abstract = text.Match("[<]abstract.+[<][/]abstract[>]", RegexICSng)

            If abstract = "" Then
                Return text
            Else
                Return text.Replace(abstract, TrimInternalMarkup(abstract))
            End If
        End Function

        Private Shared Function TrimInternalMarkup(abstract As String) As String
            Dim text As String = abstract.GetStackValue(">", "<")
            Dim escape As String = text.Replace("<", "&lt;")

            Return abstract.Replace(text, escape)
        End Function

    End Class
End Namespace