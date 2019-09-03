Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class eSearchResult
    Public Property Count As Integer
    Public Property RetMax As Integer
    Public Property RetStart As Integer
    Public Property IdList As IdList
    Public Property TranslationSet As TranslationSet
    Public Property TranslationStack As TranslationStack
    Public Property QueryTranslation As String
End Class

Public Class IdList : Implements Enumeration(Of String)

    <XmlElement("Id")> Public Property Id As String()

    Public Overrides Function ToString() As String
        Return Id.GetJson
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
        If Not Id Is Nothing Then
            For Each id As String In Me.Id
                Yield id
            Next
        End If
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of String).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class

Public Class TranslationSet
    <XmlElement("Translation")>
    Public Property Translation As Translation()
End Class

Public Class Translation
    Public Property From As String
    Public Property [To] As String
End Class

Public Class TranslationStack
    <XmlElement("TermSet")>
    Public Property TermSet As TermSet()
    <XmlElement("OP")>
    Public Property OP As String()
End Class

Public Class TermSet
    Public Property Term As String
    Public Property Field As String
    Public Property Count As Integer
    Public Property Explode As String
End Class