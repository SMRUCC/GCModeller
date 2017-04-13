Imports System.Xml.Serialization

Namespace Assembly.EBI.ChEBI.WebServices

    <XmlRoot("getCompleteEntityResponse", [Namespace]:="http://www.ebi.ac.uk/webservices/chebi")>
    Public Structure REST

        <XmlElement>
        Public Property [return] As ChEBIEntity()

        Public Shared Function ParsingRESTData(result$) As ChEBIEntity()
            Throw New Exception
        End Function
    End Structure
End Namespace