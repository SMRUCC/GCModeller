Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<XmlType("gene")>
Public Class BackgroundGene : Inherits Synonym

    ''' <summary>
    ''' The gene name
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property name As String

    <XmlElement>
    Public Property term_id As String()
    Public Property locus_tag As NamedValue

    Public Overrides Function ToString() As String
        Return $"{MyBase.ToString}  [{locus_tag.text}]"
    End Function

End Class
