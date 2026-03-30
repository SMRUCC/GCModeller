Imports System.Xml.Serialization
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.Interpro.Xml

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

        <XmlElement("p")> Public Property p As String()

        Public Shared Function CleanText(text As String) As String

        End Function

    End Class
End Namespace