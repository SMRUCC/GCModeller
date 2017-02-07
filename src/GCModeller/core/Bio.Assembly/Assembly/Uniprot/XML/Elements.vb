Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

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
        Public Property sequence As sequence

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

    Public Class sequence
        <XmlAttribute> Public Property length As Integer
        <XmlAttribute> Public Property mass As String
        <XmlAttribute> Public Property checksum As String
        <XmlAttribute> Public Property modified As String
        <XmlAttribute> Public Property version As String

        <XmlText> Public Property sequence As String

        Public Overrides Function ToString() As String
            Return sequence
        End Function
    End Class

    Public Class comment
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        Public Property text As value
    End Class

    Public Class gene

        <XmlElement("name")> Public Property names As value()
            Get
                Return table.Values _
                    .IteratesALL _
                    .ToArray
            End Get
            Set(value As value())
                If value.IsNullOrEmpty Then
                    table = New Dictionary(Of String, value())
                Else
                    ' 会有多种重复的类型
                    table = value _
                        .GroupBy(Function(name) name.type) _
                        .ToDictionary(Function(n) n.Key,
                                      Function(g) g.ToArray)
                End If
            End Set
        End Property

        Dim table As Dictionary(Of String, value())

        Public Function HaveKey(type$) As Boolean
            Return table.ContainsKey(type)
        End Function

        ''' <summary>
        ''' (primary) 基因名称
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Primary As String()
            Get
                If table.ContainsKey("primary") Then
                    Return table("primary").ToArray(Function(o) o.value)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' (ORF) 基因编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ORF As String()
            Get
                If table.ContainsKey("ORF") Then
                    Return table("ORF").ToArray(Function(o) o.value)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
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
        Public Property submittedName As recommendedName

        Public ReadOnly Property FullName As String
            Get
                If recommendedName Is Nothing OrElse recommendedName.fullName Is Nothing Then
                    If submittedName Is Nothing OrElse submittedName.fullName Is Nothing Then
                        Return Nothing
                    Else
                        Return submittedName.fullName.value
                    End If
                Else
                    Return recommendedName.fullName.value
                End If
            End Get
        End Property
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