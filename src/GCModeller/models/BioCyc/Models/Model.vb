Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public MustInherit Class Model : Implements IReadOnlyId

    ''' <summary>
    ''' the unique reference id of current feature 
    ''' element object.
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("UNIQUE-ID")>
    Public Property uniqueId As String Implements IReadOnlyId.Identity

End Class
