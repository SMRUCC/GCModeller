Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.Uniprot.XML.UniRef

    Public Class entry : Implements INamedValue

        <XmlAttribute>
        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property representativeMember As representativeMember

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class representativeMember
        Public Property dbReference As dbReference
        Public Property sequence As sequence
    End Class
End Namespace