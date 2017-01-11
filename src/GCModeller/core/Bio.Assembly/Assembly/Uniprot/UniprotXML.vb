Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml

Namespace Assembly.Uniprot.XML

    <XmlType("uniprot")> Public Class UniprotXML

        Const ns$ = "xmlns=""http://uniprot.org/uniprot"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://uniprot.org/uniprot http://www.uniprot.org/support/docs/uniprot.xsd"""

        <XmlElement("entry")>
        Public Property entries As entry()
        <XmlElement>
        Public Property copyright As String

        Public Shared Function Load(path$) As UniprotXML
            Dim xml As String = path.ReadAllText.Replace(UniprotXML.ns, Xmlns.DefaultXmlns)
            Dim model As UniprotXML = xml.LoadFromXml(Of UniprotXML)
            Return model
        End Function

        Public Overrides Function ToString() As String
            Return GetXml
        End Function
    End Class

    Public Class entry

        <XmlAttribute> Public Property dataset As String
        <XmlAttribute> Public Property created As String
        <XmlAttribute> Public Property modified As String
        <XmlAttribute> Public Property version As String

        Public Property accession As String
        Public Property name As String

        <XmlElement("dbReference")> Public Property dbReferences As dbReference()
            Get
                Return Xrefs.Values.ToVector
            End Get
            Set(value As dbReference())
                _Xrefs = value _
                    .OrderBy(Function(ref) ref.type) _
                    .GroupBy(Function(ref) ref.type) _
                    .ToDictionary(Function(t) t.Key,
                                  Function(v) v.ToArray)
            End Set
        End Property

        <XmlIgnore>
        Public ReadOnly Property Xrefs As Dictionary(Of String, dbReference())
    End Class

    Public Class value
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        <XmlText> Public Property value As String
    End Class

    Public Class dbReference
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property id As String
        <XmlElement("property")>
        Public Property properties As [property]()
    End Class

    Public Class [property]
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace