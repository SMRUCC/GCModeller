Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class IndexWriter : Inherits IndexAbstract

    Sub New(EXPORT$, db$, gi2$)

    End Sub
End Class

Public MustInherit Class IndexAbstract
    Implements sIdEnumerable

    Dim __gi As String

    ''' <summary>
    ''' 只读
    ''' </summary>
    ''' <returns></returns>
    Public Property gi As String Implements sIdEnumerable.Identifier
        Get
            Return __gi
        End Get
        Private Set(value As String)
            __gi = value
        End Set
    End Property

End Class