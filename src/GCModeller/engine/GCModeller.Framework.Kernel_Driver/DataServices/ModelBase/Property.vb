Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace LDM

    <XmlType("GCML_Document_PropertyValue", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
    Public Class [Property]

        <XmlAttribute> Public Property Name As String
        <XmlElement> Public Property Authors As List(Of String)
        <XmlElement> Public Property Comment As String
        <XmlAttribute> Public Property [CompiledDate] As String
        <XmlElement> Public Property SpecieId As String
        <XmlElement> Public Property Title As String
        <XmlElement> Public Property Emails As List(Of String)
        <XmlAttribute> Public Property Reversion As Integer
        <XmlElement("I_GUID", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
        Public Property GUID As String
        <XmlElement> Public Property Publications As List(Of String)
        <XmlElement> Public Property URLs As List(Of String)
        <XmlElement("DB-xRefLinks", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
        Public Property DBLinks As String()

        Sub New()
            Authors = New List(Of String) From {My.Computer.Name}
            CompiledDate = Now.ToString
            Emails = New List(Of String)
            GUID = System.Guid.NewGuid.ToString
            Publications = New List(Of String)
            URLs = New List(Of String) From {"http://code.google.com/p/genome-in-code"}
            DBLinks = New String() {}
        End Sub

        Public Overrides Function ToString() As String
            Return SpecieId
        End Function
    End Class
End Namespace