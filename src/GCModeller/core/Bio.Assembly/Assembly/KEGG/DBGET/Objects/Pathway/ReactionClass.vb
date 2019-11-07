Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class ReactionClass

        Public Property entryId As String
        Public Property definition As String
        Public Property reactantPairs As NamedValue()
        Public Property reactions As NamedValue()
        Public Property enzymes As NamedValue()
        Public Property pathways As NamedValue()
        Public Property orthology As NamedValue()

    End Class
End Namespace