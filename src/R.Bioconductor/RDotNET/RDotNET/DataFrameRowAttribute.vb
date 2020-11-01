Imports System.Reflection

Friend Delegate Sub Map(ByVal from As DataFrameRow, ByVal [to] As Object)

''' <summary>
''' Indicates the class with the attribute represents rows of certain data frames.
''' </summary>
<AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
Public Class DataFrameRowAttribute
    Inherits Attribute

    Private ReadOnly cache As Dictionary(Of Type, Map)

    ''' <summary>
    ''' Initializes a new instance.
    ''' </summary>
    Public Sub New()
        cache = New Dictionary(Of Type, Map)()
    End Sub

    Friend Function Convert(Of TRow As {Class, New})(ByVal row As DataFrameRow) As TRow
        Dim rowType = GetType(TRow)
        Dim map As Map = Nothing

        If Not cache.TryGetValue(rowType, map) Then
            map = CreateMap(rowType)
            cache.Add(rowType, map)
        End If

        Dim result = Activator.CreateInstance(rowType)
        map(row, result)
        Return result
    End Function

    Private Shared Function CreateMap(ByVal rowType As Type) As Map
        Dim tuples = (From [property] In rowType.GetProperties() Let attribute = CType([property].GetCustomAttributes(GetType(DataFrameColumnAttribute), True).SingleOrDefault(), DataFrameColumnAttribute) Where attribute IsNot Nothing Select Tuple.Create(attribute, [property].GetSetMethod())).ToArray()
        Return Sub(from, [to]) Map(from, [to], tuples)
    End Function

    Private Shared Sub Map(ByVal from As DataFrameRow, ByVal [to] As Object, ByVal tuples As Tuple(Of DataFrameColumnAttribute, MethodInfo)())
        Dim names = from.DataFrame.ColumnNames

        For Each t In tuples
            Dim attribute = t.Item1
            Dim setter = t.Item2
            Dim index = attribute.GetIndex(names)
            setter.Invoke([to], New Object() {from.GetInnerValue(index)})
        Next
    End Sub
End Class

