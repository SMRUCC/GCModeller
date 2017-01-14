Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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

    Public Class entry : Implements INamedValue

        <XmlAttribute> Public Property dataset As String
        <XmlAttribute> Public Property created As String
        <XmlAttribute> Public Property modified As String
        <XmlAttribute> Public Property version As String

        ''' <summary>
        ''' 蛋白质的唯一标识符，可以用作为字典的键名
        ''' </summary>
        ''' <returns></returns>
        Public Property accession As String Implements INamedValue.Key
        Public Property name As String
        Public Property protein As protein
        <XmlElement("feature")>
        Public Property features As feature()
        Public Property gene As gene
        Public Property proteinExistence As value
        Public Property organism As organism

        <XmlElement("keyword")> Public Property keywords As value()
        <XmlElement("comment")> Public Property comments As comment()
            Get
                Return CommentList.Values.ToVector
            End Get
            Set(value As comment())
                If value Is Nothing Then
                    _CommentList = New Dictionary(Of String, comment())
                Else
                    _CommentList = value _
                        .OrderBy(Function(c) c.type) _
                        .GroupBy(Function(c) c.type) _
                        .ToDictionary(Function(t) t.Key,
                                      Function(v) v.ToArray)
                End If
            End Set
        End Property

        <XmlElement("dbReference")> Public Property dbReferences As dbReference()
            Get
                Return Xrefs.Values.ToVector
            End Get
            Set(value As dbReference())
                If value Is Nothing Then
                    _Xrefs = New Dictionary(Of String, dbReference())
                    Return
                End If

                _Xrefs = value _
                    .OrderBy(Function(ref) ref.type) _
                    .GroupBy(Function(ref) ref.type) _
                    .ToDictionary(Function(t) t.Key,
                                  Function(v) v.ToArray)
            End Set
        End Property

        <XmlIgnore>
        Public ReadOnly Property CommentList As Dictionary(Of String, comment())
        <XmlIgnore>
        Public ReadOnly Property Xrefs As Dictionary(Of String, dbReference())


    End Class

    Public Class comment
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        Public Property text As value
    End Class

    Public Class gene
        Public Property name As value
    End Class

    Public Class organism

        <XmlAttribute> Public Property evidence As String
        <XmlElement("name")> Public Property names As value()
            Get
                Return namesData.Values.ToArray
            End Get
            Set(value As value())
                If value Is Nothing Then
                    _namesData = New Dictionary(Of String, value)
                Else
                    _namesData = value.ToDictionary(Function(x) x.type)
                End If
            End Set
        End Property

        Public Property dbReference As value
        Public Property lineage As lineage

        <XmlIgnore>
        Public ReadOnly Property namesData As Dictionary(Of String, value)

        <XmlIgnore>
        Public ReadOnly Property scientificName As String
            Get
                If namesData.ContainsKey("scientific") Then
                    Return namesData("scientific").value
                Else
                    Return ""
                End If
            End Get
        End Property

        <XmlIgnore>
        Public ReadOnly Property commonName As String
            Get
                If namesData.ContainsKey("common") Then
                    Return namesData("common").value
                Else
                    Return ""
                End If
            End Get
        End Property
    End Class

    Public Class lineage
        <XmlElement("taxon")> Public Property taxonlist As String()
    End Class

    Public Class protein
        Public Property recommendedName As recommendedName
    End Class

    Public Class feature
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        <XmlAttribute> Public Property description As String
        <XmlText> Public Property value As String
        Public Property location As location
    End Class

    Public Class location
        Public Property begin As position
        Public Property [end] As position
    End Class

    Public Class position
        Public Property position As String
    End Class

    Public Class recommendedName
        Public Property fullName As value
        Public Property ecNumber As value
    End Class

    Public Class value

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        <XmlAttribute> Public Property description As String
        <XmlAttribute> Public Property id As String
        <XmlText> Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
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