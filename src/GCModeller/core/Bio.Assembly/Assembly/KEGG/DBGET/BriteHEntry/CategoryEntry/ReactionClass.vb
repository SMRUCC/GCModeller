
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class ReactionClass : Implements INamedValue

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        Public Property subclass As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' D
        ''' </summary>
        ''' <returns></returns>
        Public Property ECNumber As String

        ''' <summary>
        ''' E: The KEGG RC number
        ''' </summary>
        ''' <returns></returns>
        Public Property RCNumber As String Implements IKeyedEntity(Of String).Key

        Public Shared Function LoadFromResource() As ReactionClass
            Dim htext As htext = htext.br08201
        End Function

    End Class
End Namespace