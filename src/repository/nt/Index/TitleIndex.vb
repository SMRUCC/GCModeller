Imports Microsoft.VisualBasic.Data.IO

Public Class TitleIndex
    Inherits IndexAbstract

    ReadOnly __handle As BinaryDataReader
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)

    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)
    End Sub


End Class