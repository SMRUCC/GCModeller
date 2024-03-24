Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MeSH

    ''' <summary>
    ''' the mesh Descriptor Record Set xml file
    ''' </summary>
    ''' <remarks>
    ''' which could be download from the ncbi ftp website: 
    ''' 
    ''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/?_gl=1*jikpoo*_ga*MTQ4NzExODI0OS4xNjg3NDAyOTQ4*_ga_7147EPK006*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..*_ga_P1FPTH9PL4*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..
    ''' </remarks>
    Public Class DescriptorRecordSet

        <XmlAttribute> Public Property LanguageCode As String

        <XmlElement>
        Public Property DescriptorRecord As DescriptorRecord()

        Public Shared Function ReadTerms(file As String) As IEnumerable(Of DescriptorRecord)
            Return file.LoadUltraLargeXMLDataSet(Of DescriptorRecord)()
        End Function

        ''' <summary>
        ''' create term index by name
        ''' </summary>
        ''' <param name="terms"></param>
        ''' <returns></returns>
        Public Shared Function TreeTermIndex(terms As IEnumerable(Of DescriptorRecord)) As Dictionary(Of String, DescriptorRecord)
            Return terms.SafeQuery.ToDictionary(Function(term) term.DescriptorName.String)
        End Function

    End Class
End Namespace