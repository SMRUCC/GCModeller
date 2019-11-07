
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

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

        Public Shared Iterator Function LoadFromResource() As IEnumerable(Of ReactionClass)
            Dim htext As htext = htext.br08204

            For Each [class] As BriteHText In htext.Hierarchical.categoryItems
                For Each subclass As BriteHText In [class].categoryItems
                    For Each category As BriteHText In subclass.categoryItems
                        For Each ECNumber As BriteHText In category.categoryItems
                            For Each entry As BriteHText In ECNumber.categoryItems.SafeQuery
                                Yield New ReactionClass With {
                                    .category = category.classLabel,
                                    .[class] = [class].classLabel,
                                    .ECNumber = ECNumber.classLabel,
                                    .RCNumber = entry.entryID,
                                    .subclass = subclass.classLabel
                                }
                            Next
                        Next
                    Next
                Next
            Next
        End Function

        Friend Function GetPathComponents() As String
            Dim tokens As New List(Of String)

            If [class].Length > 64 Then
                tokens += Mid([class], 1, 61).NormalizePathString & "~"
            Else
                tokens += [class]
            End If
            If subclass.Length > 64 Then
                tokens += Mid(subclass, 1, 61).NormalizePathString & "~"
            Else
                tokens += subclass
            End If
            If category.Length > 64 Then
                tokens += Mid(category, 1, 61).NormalizePathString & "~"
            Else
                tokens += category
            End If

            tokens += ECNumber

            Return tokens.JoinBy("/")
        End Function
    End Class
End Namespace