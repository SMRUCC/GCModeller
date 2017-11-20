Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class PMID

    <XmlAttribute>
    Public Property Version As String
    <XmlText>
    Public Property ID As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class Article
    <XmlAttribute>
    Public Property PubModel As String
    Public Property Journal As Journal
    Public Property ArticleTitle As String
    Public Property Pagination As Pagination
    Public Property ELocationID As ELocationID
    Public Property Abstract As Abstract
    Public Property AuthorList As AuthorList
    Public Property Language As String
    Public Property PublicationTypeList As PublicationTypeList
    Public Property ArticleDate As PubDate

    Public Overrides Function ToString() As String
        Return ArticleTitle
    End Function
End Class

Public Class PublicationTypeList
    <XmlElement("PublicationType")> Public Property PublicationType As RegisterObject()
End Class

Public Class AuthorList
    <XmlAttribute>
    Public Property CompleteYN As String
    <XmlElement(NameOf(Author))>
    Public Property Authors As Author()
End Class

Public Class Author
    <XmlAttribute>
    Public Property ValidYN As String
    Public Property LastName As String
    Public Property ForeName As String
    Public Property Initials As String
    Public Property AffiliationInfo As AffiliationInfo

    Public Overrides Function ToString() As String
        Dim disp$ = $"{Initials} {ForeName} {LastName}"
        If AffiliationInfo Is Nothing Then
            disp &= $" ({AffiliationInfo.Affiliation})"
        End If
        Return disp
    End Function
End Class

Public Class AffiliationInfo
    Public Property Affiliation As String

    Public Overrides Function ToString() As String
        Return Affiliation
    End Function
End Class

Public Class Abstract
    Public Property AbstractText As String

    Public Overrides Function ToString() As String
        Return AbstractText
    End Function
End Class

Public Class ELocationID
    <XmlAttribute> Public Property EIdType As String
    <XmlAttribute> Public Property ValidYN As String

    <XmlText>
    Public Property Value As String

    Public Overrides Function ToString() As String
        Return EIdType & ": " & Value
    End Function
End Class

Public Class Pagination
    Public Property MedlinePgn As String
End Class

Public Class Journal
    Public Property JournalIssue As JournalIssue
    Public Property Title As String
    Public Property ISOAbbreviation As String
End Class

Public Class JournalIssue
    <XmlAttribute>
    Public Property CitedMedium As String
    Public Property Volume As String
    Public Property Issue As String
    Public Property PubDate As PubDate
End Class

Public Class PubDate
    <XmlAttribute>
    Public Property DateType As String
    Public Property Year As String
    Public Property Month As String
    Public Property Day As String

    Public Overrides Function ToString() As String
        Return CType(Me, Date).ToString
    End Function

    Public Overloads Shared Narrowing Operator CType(d As PubDate) As Date
        Return New Date(d.Year, ValueTypes.GetMonthInteger(d.Month), d.Day)
    End Operator
End Class