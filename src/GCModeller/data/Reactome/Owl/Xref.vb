Imports System.Xml.Serialization
Imports SMRUCC.genomics.DatabaseServices.Reactome.OwlDocument.Abstract

Namespace OwlDocument.XrefNodes

    Public MustInherit Class Xref : Inherits ResourceElement
        Public Property db As String
        Public Property id As String
    End Class

    Public Class UnificationXref : Inherits Xref
        Public Property comment As String
        Public Property idVersion As String
    End Class

    Public Class PublicationXref : Inherits Xref
        Public Property year As String
        Public Property title As String
        <XmlElement> Public Property author As String()
        Public Property source As String
    End Class

    Public Class RelationshipXref : Inherits Xref
        Public Property comment As String
        Public Property relationshipType As RDFresource
    End Class
End Namespace