Imports System.Xml.Serialization

Namespace BITS

    ''' <summary>
    ''' the xml definition of the ncbi book data
    ''' </summary>
    ''' <remarks>
    ''' A set of Book Interchange Tag Suite (BITS)
    ''' DTD modules was written as the basis for
    ''' publishing, interchange, and repository
    ''' book DTDs, with the intention that DTDs for
    ''' specific purposes, such as this Book
    ''' DTD, would be developed based on them.
    ''' 
    ''' This Book Interchange DTD has been developed
    ''' from the ANSI/NISO JATS Z39.96 DTD modules,
    ''' in the approved manner, making changes to the
    ''' declarations in those modules by over-riding
    ''' Parameter Entity contents. These overrides
    ''' are defined in the three BITS book
    ''' customization modules:
    ''' 
    ''' ```
    '''     %bookcustom-classes.ent;
    '''     %bookcustom-mixes.ent;
    '''     %bookcustom-models.ent;
    ''' ```
    ''' 
    ''' which are called from this DTD file.
    ''' </remarks>
    <XmlType("book-part-wrapper")>
    Public Class BookPartWrapper

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("content-type")> Public Property content_type As String
        <XmlAttribute("from-where")> Public Property from_where As String
        <XmlAttribute("dtd-version")> Public Property dtd_version As String

        <XmlElement("book-part")> Public Property book_part As BookPart

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace