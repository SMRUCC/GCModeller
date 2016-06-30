Imports System.Xml.Serialization

Namespace SiteSearch

    ''' <summary>
    ''' ftp://ftp.ebi.ac.uk/pub/databases/Pfam/sitesearch/PfamFamily.xml.gz
    ''' </summary>
    ''' 
    <XmlType("database")>
    Public Class PfamFamily : Inherits PfamObject
        <XmlElement("release")> Public Property Release As String
        <XmlElement("release_date")> Public Property ReleaseDate As String
        <XmlElement("entry_count")> Public Property EntryCount As Integer
        <XmlArray("entries")> Public Property Entries As Entry()
    End Class

    Public MustInherit Class PfamObject
        <XmlElement("name")> Public Property Name As String
        <XmlElement("description")> Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{Name}:  {Description}"
        End Function
    End Class

    <XmlType("entry")>
    Public Class Entry : Inherits PfamObject
        <XmlAttribute("id")> Public Property ID As String
        <XmlAttribute("acc")> Public Property AccID As String
        <XmlAttribute("authors")> Public Property Authors As String
        <XmlArray("dates")> Public Property Dates As [Date]()
        <XmlArray("additional_fields")> Public Property AdditionalFields As Field()
        <XmlArray("cross_references")> Public Property Xrefs As CrossRef()
    End Class

    <XmlType("date")>
    Public Class [Date]
        <XmlAttribute("type")> Public Property Type As String
        <XmlAttribute("value")> Public Property value As String

        Public Overrides Function ToString() As String
            Return $"({Type})  {value}"
        End Function
    End Class

    <XmlType("field")>
    Public Class Field
        <XmlAttribute("name")> Public Property Name As String
        <XmlText> Public Property value As String

        Public Overrides Function ToString() As String
            Return $"{Name} = ""{value}"""
        End Function
    End Class

    <XmlType("ref")>
    Public Class CrossRef
        <XmlAttribute("dbname")> Public Property DbName As String
        <XmlAttribute("dbkey")> Public Property DbKey As String

        Public Overrides Function ToString() As String
            Return $"{DbName} => {DbKey}"
        End Function
    End Class
End Namespace