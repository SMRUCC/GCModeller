Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Namespace vcXML

    Module XmlHelper

        Friend emptyNamespace As New XmlSerializerNamespaces()

        Sub New()
            emptyNamespace.Add(String.Empty, String.Empty)
        End Sub

        Public Function getXmlFragment(Of T)(obj As T, xmlConfig As XmlWriterSettings) As String
            Dim serializer As New XmlSerializer(GetType(T))
            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, xmlConfig)

            Call serializer.Serialize(writer, obj, emptyNamespace)

            Return output.ToString
        End Function
    End Module
End Namespace