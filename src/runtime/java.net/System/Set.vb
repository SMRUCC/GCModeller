Imports Microsoft.VisualBasic

Public Class [Set](Of T) : Inherits List(Of T)

    Property size As Integer

    Sub New()
        Call MyBase.New
    End Sub

End Class

Public Class LinkedList : Inherits List(Of Object)

    Sub New(data As IList)
        Call MyBase.New(data)
    End Sub

    Sub New()
        Call MyBase.New
    End Sub

End Class

Public Class HashSet : Inherits HashSet(Of Object)

End Class