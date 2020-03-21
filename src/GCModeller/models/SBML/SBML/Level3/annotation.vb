
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
End Namespace