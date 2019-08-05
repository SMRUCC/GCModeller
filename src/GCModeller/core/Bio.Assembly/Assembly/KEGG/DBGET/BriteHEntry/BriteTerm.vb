Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' A general brite term
    ''' </summary>
    Public Class BriteTerm

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        Public Property subcategory As String
        ''' <summary>
        ''' D
        ''' </summary>
        ''' <returns></returns>
        Public Property order As String
        ''' <summary>
        ''' ``{compoundID => name}``
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As KeyValuePair

        Public Overrides Function ToString() As String
            Return entry.ToString
        End Function
    End Class
End Namespace