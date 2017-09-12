Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Assembly.Uniprot.XML.UniRef

    ''' <summary>
    ''' http://uniprot.org/uniref
    ''' </summary>
    Public Class entry : Implements INamedValue

        <XmlAttribute>
        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property representativeMember As representativeMember
        <XmlElement("member")>
        Public Property members As representativeMember()

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class representativeMember

        Public Property dbReference As dbReference
        Public Property sequence As sequence

        Public ReadOnly Property UniProtKB_accession As String
            Get
                Return dbReference("UniProtKB accession")
            End Get
        End Property

        Public ReadOnly Property source_organism As String
            Get
                Return dbReference("source organism")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return UniProtKB_accession
        End Function
    End Class

    Public Module Extensions

        Public Function PopulateALL(path$) As IEnumerable(Of entry)
            Return path.LoadXmlDataSet(Of entry)(NameOf(entry), xmlns:="http://uniprot.org/uniref")
        End Function
    End Module
End Namespace