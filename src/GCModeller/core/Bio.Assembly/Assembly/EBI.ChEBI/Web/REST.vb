Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Assembly.EBI.ChEBI.WebServices

    <XmlRoot("getCompleteEntityResponse", [Namespace]:="http://www.ebi.ac.uk/webservices/chebi")>
    Public Structure REST

        <XmlElement>
        Public Property [return] As ChEBIEntity()

        Public Shared Function ParsingRESTData(result$) As ChEBIEntity()
            Dim xml As XmlDocument = result.LoadXmlDocument
            Dim nodes = xml.GetElementsByTagName("S:Body")
            Dim out As New List(Of ChEBIEntity)

            For Each node As XmlNode In nodes
                result = node.InnerXml
                out += result.CreateObjectFromXmlFragment(Of REST).return
            Next

            Return out
        End Function
    End Structure
End Namespace