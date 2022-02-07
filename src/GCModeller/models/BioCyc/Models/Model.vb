Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public MustInherit Class Model : Implements IReadOnlyId

    ''' <summary>
    ''' the unique reference id of current feature 
    ''' element object.
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("UNIQUE-ID")>
    Public Property uniqueId As String Implements IReadOnlyId.Identity

    <AttributeField("TYPES")>
    Public Property types As String()

    <AttributeField("COMMON-NAME")>
    Public Property commonName As String

    <AttributeField("CITATIONS")>
    Public Property citations As String()

    <AttributeField("COMMENT")>
    Public Property comment As String

    <AttributeField("CREDITS")>
    Public Property credits As String()

    <AttributeField("INSTANCE-NAME-TEMPLATE")>
    Public Property instanceNameTemplate As String
    <AttributeField("SYNONYMS")>
    Public Property synonyms As String()

    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

End Class
