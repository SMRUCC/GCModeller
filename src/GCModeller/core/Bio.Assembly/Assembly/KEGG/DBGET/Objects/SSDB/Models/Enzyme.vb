Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    Public Class Enzyme

        ''' <summary>
        ''' The ``EC`` number
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As String
        Public Property commonNames As String()
        Public Property [class] As String()
        Public Property sysname As String
        Public Property IUBMB As String
        ''' <summary>
        ''' The kegg reaction id list
        ''' </summary>
        ''' <returns></returns>
        Public Property reactions As String()
        Public Property substrate As String()
        Public Property product As String()
        Public Property comment As String
        Public Property orthology As OrthologyTerms
        Public Property genes As NamedValue()

    End Class
End Namespace