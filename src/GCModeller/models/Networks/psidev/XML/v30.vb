Imports System.Xml.Serialization

Namespace XML

    ''' <summary>
    ''' PSI-MI XML v3.0 data interchange format
    ''' </summary>
    ''' <remarks>
    ''' Example:
    ''' 
    ''' ```xml
    ''' &lt;mif:entrySet level="3" minorVersion="0" version="0" xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''     &lt;mif:entry>{1,unbounded}&lt;/mif:entry>
    ''' &lt;/mif:entrySet>
    ''' ```
    ''' </remarks>
    <XmlRoot("entrySet", [Namespace]:="http://psi.hupo.org/mi/mif300")>
    Public Class v30

        Public Property minorVersion As Integer

    End Class
End Namespace