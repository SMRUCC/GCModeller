Imports System.Xml.Serialization

Namespace XML.MIF30

    ''' <summary>
    ''' Describes one or more interactions as a self-contained unit. Multiple entries from different files can be
    ''' concatenated into a Single entrySet.
    ''' </summary>
    ''' <remarks>
    ''' Example:
    ''' 
    ''' ```xml
    ''' &lt;mif:entry xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''    &lt;MIF:source release="" releaseDate="">{0,1}&lt;/mif:source>
    '''    &lt;MIF:availabilityList>{0,1}&lt;/mif:availabilityList>
    '''    &lt;MIF:experimentList>{0,1}&lt;/mif:experimentList>
    '''    &lt;MIF:interactorList>{0,1}&lt;/mif:interactorList>
    '''    &lt;MIF:interactionList>{1,1}&lt;/mif:interactionList>
    '''    &lt;MIF:attributeList>{0,1}&lt;/mif:attributeList>
    ''' &lt;/mif:entry>
    ''' ```
    ''' </remarks>
    Public Class entry

        Public Property source As source
        Public Property availabilityList
        Public Property experimentList
        Public Property interactorList
        Public Property interactionList
        Public Property attributeList

    End Class

    ''' <summary>
    ''' Example:
    ''' 
    ''' ```
    ''' &lt;mif:source release="" releaseDate="" xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''    &lt;MIF:names>{0,1}&lt;/mif:names>
    '''    &lt;MIF:bibref>{0,1}&lt;/mif:bibref>
    '''    &lt;MIF:xref>{0,1}&lt;/mif:xref>
    '''    &lt;MIF:attributeList>{0,1}&lt;/mif:attributeList>
    ''' &lt;/mif:source>
    ''' ```
    ''' </summary>
    Public Class source

        <XmlAttribute> Public Property release As String
        <XmlAttribute> Public Property releaseDate As String
        Public Property names As String
        Public Property bibref As String
        Public Property xref As String
        Public Property attributeList
    End Class
End Namespace