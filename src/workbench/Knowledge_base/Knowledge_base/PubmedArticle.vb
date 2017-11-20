Imports System.Xml.Serialization

Public Class PubmedArticle
    Public Property MedlineCitation As MedlineCitation
    Public Property PubmedData As PubmedData
End Class

Public Class MedlineCitation
    Public Property Status As String
    Public Property Owner As String
    Public Property PMID As PMID
    Public Property DateCreated As PubDate
    Public Property DateCompleted As PubDate
    Public Property DateRevised As PubDate
    Public Property Article As Article
    Public Property MedlineJournalInfo As MedlineJournalInfo
    Public Property ChemicalList As Chemical()
    Public Property CitationSubset As String
    Public Property MeshHeadingList As MeshHeading()
End Class

Public Class MeshHeading
    Public Property DescriptorName As RegisterObject
    <XmlElement("QualifierName")>
    Public Property QualifierName As RegisterObject()
End Class

Public Class PubmedData
    Public Property History As History
    Public Property PublicationStatus As String
    Public Property ArticleIdList As ArticleId()
End Class