Imports System.Xml.Serialization

Public Class MedlineJournalInfo
    Public Property Country As String
    Public Property MedlineTA As String
    Public Property NlmUniqueID As String
    Public Property ISSNLinking As String
End Class

Public Class Chemical
    Public Property RegistryNumber As String
    Public Property NameOfSubstance As RegisterObject
End Class

Public Class RegisterObject

    <XmlAttribute>
    Public Property UI As String
    <XmlAttribute>
    Public Property MajorTopicYN As String
    <XmlText>
    Public Property Value As String
End Class