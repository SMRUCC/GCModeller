
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace Level3

    Public Class speciesAnnotation : Inherits Description

        <XmlElement("is", [Namespace]:=annotation.AnnotationInfo.bqbiol)>
        Public Property [is] As [is]

        Sub New()
            Call MyBase.New
        End Sub
    End Class

    Public Class [is]

        <XmlElement("Bag", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property Bag As Array
    End Class

    Public Class annotation

        <XmlElement("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property RDF As AnnotationInfo

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
        End Sub

        <XmlType("annoinfo", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Class AnnotationInfo : Inherits RDF(Of speciesAnnotation)

            Public Const bqbiol As String = "http://biomodels.net/biology-qualifiers/"
            Public Const bqmodel As String = "http://biomodels.net/model-qualifiers/"

            Sub New()
                Call MyBase.New

                Call MyBase.xmlns.Add("bqbiol", "http://biomodels.net/biology-qualifiers/")
                Call MyBase.xmlns.Add("bqmodel", "http://biomodels.net/model-qualifiers/")
            End Sub
        End Class

    End Class
End Namespace