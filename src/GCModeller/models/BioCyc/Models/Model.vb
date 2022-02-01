Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public MustInherit Class Model : Implements IReadOnlyId

    Public ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity

End Class
