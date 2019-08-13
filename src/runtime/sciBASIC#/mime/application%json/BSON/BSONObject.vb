
Public Class BSONObject
    Inherits BSONValue
    Implements IEnumerable
    Private mMap As New Dictionary(Of String, BSONValue)()

    Public Sub New()
        MyBase.New(ValueType.[Object])
    End Sub

    '
    ' Properties
    '
    Public ReadOnly Property Keys() As ICollection(Of String)
        Get
            Return mMap.Keys
        End Get
    End Property

    Public ReadOnly Property Values() As ICollection(Of BSONValue)
        Get
            Return mMap.Values
        End Get
    End Property
    Public ReadOnly Property Count() As Integer
        Get
            Return mMap.Count
        End Get
    End Property

    '
    ' Indexer
    '
    Default Public Overrides Property Item(key As String) As BSONValue
        Get
            Return mMap(key)
        End Get
        Set
            mMap(key) = Value
        End Set
    End Property
    '
    ' Methods
    '
    Public Overrides Sub Clear()
        mMap.Clear()
    End Sub
    Public Overrides Sub Add(key As String, value As BSONValue)
        mMap.Add(key, value)
    End Sub


    Public Overrides Function Contains(v As BSONValue) As Boolean
        Return mMap.ContainsValue(v)
    End Function
    Public Overrides Function ContainsKey(key As String) As Boolean
        Return mMap.ContainsKey(key)
    End Function

    Public Function Remove(key As String) As Boolean
        Return mMap.Remove(key)
    End Function

    Public Function TryGetValue(key As String, ByRef value As BSONValue) As Boolean
        Return mMap.TryGetValue(key, value)
    End Function


    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mMap.GetEnumerator()
    End Function
End Class

