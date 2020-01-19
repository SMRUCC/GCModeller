Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel

Namespace Arguments

    Public Class Xml : Inherits XmlDataModel

        <Prefix("--header")>
        Public Property header As Decoration
        <Prefix("--footer")>
        Public Property footer As Decoration
        Public Property globalOptions As GlobalOptions
        Public Property outline As Outline
        Public Property page As Page
        Public Property pagesize As PageSize
        Public Property TOC As TOC

    End Class
End Namespace