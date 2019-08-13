
Public Class BSONArray
    Inherits BSONValue
    Implements IEnumerable

    Private mList As New List(Of BSONValue)()

    Public Sub New()
        MyBase.New(BSONValue.valueType.Array)
    End Sub

    '
    ' Indexer
    '
    Default Public Overrides Property Item(index As Integer) As BSONValue
        Get
            Return mList(index)
        End Get
        Set
            mList(index) = Value
        End Set
    End Property
    Public ReadOnly Property Count() As Integer
        Get
            Return mList.Count
        End Get
    End Property

    '
    ' Methods
    '
    Public Overrides Sub Add(v As BSONValue)
        mList.Add(v)
    End Sub

    Public Function IndexOf(item As BSONValue) As Integer
        Return mList.IndexOf(item)
    End Function
    Public Sub Insert(index As Integer, item As BSONValue)
        mList.Insert(index, item)
    End Sub
    Public Function Remove(v As BSONValue) As Boolean
        Return mList.Remove(v)
    End Function
    Public Sub RemoveAt(index As Integer)
        mList.RemoveAt(index)
    End Sub
    Public Overrides Sub Clear()
        mList.Clear()
    End Sub

    Public Overridable Overloads Function Contains(v As BSONValue) As Boolean
        Return mList.Contains(v)
    End Function


    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mList.GetEnumerator()
    End Function
End Class
