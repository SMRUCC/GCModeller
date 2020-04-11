Imports System.Runtime.Serialization
Imports System.Web.Script.Serialization
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace v2

    Public Class ZipComponent(Of T) : Inherits ListOf(Of T)
        Implements XmlDataModel.IXmlType

        <DataMember>
        <IgnoreDataMember>
        <ScriptIgnore>
        <SoapIgnore>
        <XmlAnyElement>
        Public Property TypeComment As XmlComment Implements XmlDataModel.IXmlType.TypeComment
            Get
                Return XmlDataModel.CreateTypeReferenceComment(GetType(T))
            End Get
            Set(value As XmlComment)
                ' do nothing
            End Set
        End Property

        <XmlElement>
        Public Property components As T()

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            Call xmlns.Add("vcellkit", ModelBaseType.GCModellerVCellKit)
            Call xmlns.Add("biocad", VirtualCell.GCMarkupLanguage)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{components.TryCount} components"
        End Function

        Protected Overrides Function getSize() As Integer
            Return components.TryCount
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of T)
            Return components.AsEnumerable
        End Function
    End Class
End Namespace