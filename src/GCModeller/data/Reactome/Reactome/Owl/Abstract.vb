Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace OwlDocument.Abstract

    Public MustInherit Class ResourceElement
        Implements sIdEnumerable

        <XmlAttribute("ID")>
        Public Property ResourceId As String Implements sIdEnumerable.Identifier

        Public Overrides Function ToString() As String
            Return ResourceId
        End Function
    End Class

    Public MustInherit Class Node : Inherits ResourceElement
        <XmlElement("xref")>
        Public Property Xref As RDFresource()
        <XmlElement("dataSource")>
        Public Property DataSource As RDFresource
        <XmlElement("comment")>
        Public Property Comments As String()
    End Class

    <XmlType("resource")>
    Public Class RDFresource
        <XmlAttribute("resource")> Public Property ResourceId As String
            Get
                Return _ResourceId
            End Get
            Set(value As String)
                _ResourceId = value
                If _ResourceId.First = "#"c Then
                    __ResourceId = Mid(value, 2)
                Else
                    __ResourceId = value
                End If
            End Set
        End Property

        Dim __ResourceId As String
        Dim _ResourceId As String

        Public Function GetResourceId() As String
            Return __ResourceId
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("resource:= '{0}'", ResourceId)
        End Function
    End Class
End Namespace
