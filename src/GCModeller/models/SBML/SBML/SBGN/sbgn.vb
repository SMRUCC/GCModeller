Imports System.Xml.Serialization

Namespace SBGN

    ''' <summary>
    ''' Systems Biology Graphical Notation
    ''' </summary>
    ''' <remarks>
    ''' Systems Biology Graphical Notation (SBGN) project, an effort to standardise 
    ''' the graphical notation used in maps of biological processes.
    ''' </remarks>
    ''' 
    <XmlRoot("sbgn", [Namespace]:="http://sbgn.org/libsbgn/0.2")>
    <XmlType("sbgn", [Namespace]:="http://sbgn.org/libsbgn/0.2")>
    Public Class sbgn

        Public Property map As map

        Public Shared Function ReadXml(file As String) As sbgn
            Return file.SolveStream.LoadFromXml(GetType(sbgn))
        End Function

    End Class

    Public Class map

        <XmlElement("glyph")>
        Public Property glyph As glyph()

    End Class

End Namespace