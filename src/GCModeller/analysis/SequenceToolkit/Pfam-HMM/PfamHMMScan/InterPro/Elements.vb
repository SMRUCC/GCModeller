Imports System.Xml.Serialization

Namespace InterPro.Xml

    <XmlType("publication")>
    Public Class Publication
        <XmlAttribute> Public Property id As String
        Public Property author_list As String
        Public Property title As String
        Public Property db_xref As db_xref
        Public Property journal As String
        Public Property location As Location
        Public Property year As String

        Public Overrides Function ToString() As String
            Return $"{author_list}, ""{title}"", {journal}, {location.ToString}, {year}"
        End Function
    End Class

    <XmlType("location")>
    Public Class Location
        <XmlAttribute> Public Property issue As String
        <XmlAttribute> Public Property pages As String
        <XmlAttribute> Public Property volume As String

        Public Overrides Function ToString() As String
            Return $"({volume}){issue}: {pages}"
        End Function
    End Class

    <XmlType("db_xref")>
    Public Class db_xref
        <XmlAttribute> Public Property db As String
        <XmlAttribute> Public Property dbkey As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return $"{db}: {dbkey}"
        End Function
    End Class

    <XmlType("rel_ref")>
    Public Class RelRef
        <XmlAttribute> Public Property ipr_ref As String

        Public Overrides Function ToString() As String
            Return ipr_ref
        End Function
    End Class

    <XmlType("taxon_data")>
    Public Class TaxonData
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property proteins_count As Integer

        Public Overrides Function ToString() As String
            Return $"{name}; //{proteins_count}"
        End Function
    End Class

    Public Class SecAcc
        <XmlAttribute> Public Property acc As String

        Public Overrides Function ToString() As String
            Return acc
        End Function
    End Class
End Namespace