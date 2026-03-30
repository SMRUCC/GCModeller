Imports System.Xml.Serialization
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.Interpro.Xml

Namespace Interpro


    <XmlType("interpro")>
    Public Class Interpro
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property short_name As String
        <XmlAttribute> Public Property type As String
        Public Property name As String
        Public Property abstract As String
        Public Property pub_list As Publication()
        Public Property parent_list As RelRef()
        Public Property contains As RelRef()
        Public Property found_in As RelRef()
        Public Property member_list As DbXref()
        Public Property external_doc_list As DbXref()
        Public Property structure_db_links As DbXref()
        Public Property taxonomy_distribution As TaxonData()
        Public Property sec_list As SecAcc()

        Public Overrides Function ToString() As String
            Return $"[{type}] {short_name}   {name}"
        End Function
    End Class
End Namespace