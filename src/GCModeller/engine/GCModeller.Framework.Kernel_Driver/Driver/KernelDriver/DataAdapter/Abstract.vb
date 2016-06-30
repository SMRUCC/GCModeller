
''' <summary>
''' <see cref="System.Double"></see>; <see cref="Integer"></see>; <see cref="System.Boolean"></see>
''' </summary>
''' <typeparam name="T"><see cref="System.Double"></see>; <see cref="Integer"></see>; <see cref="System.Boolean"></see></typeparam>
''' <remarks></remarks>
Public Interface IDriver_DataSource_Adapter(Of T)
    Function get_ObjectHandlers() As DataStorage.FileModel.ObjectHandle()
    Function get_DataSerials() As DataStorage.FileModel.DataSerials(Of T)()
End Interface

Public Class DataSourceHandler(Of TValue)
    Implements IDataSourceHandle

    Public Property Handle As Long Implements IDataSourceHandle.Handle
    Public Property Value As TValue
    Public Property TimeStamp As Integer Implements IDataSourceHandle.TimeStamp

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] ${1} -> {2}", Handle, TimeStamp, Value.ToString)
    End Function
End Class

Public Interface IDataSourceHandle
    Property Handle As Long
    Property TimeStamp As Integer
End Interface

Public Class TransitionStateSample : Inherits DataSourceHandler(Of Boolean)

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] ${1} -> Ts( '{2}' )", Handle, TimeStamp, Value)
    End Function
End Class

''' <summary>
''' <see cref="System.Double"></see>类型的值对象
''' </summary>
''' <remarks></remarks>
Public Class EntityQuantitySample : Inherits DataSourceHandler(Of Double)

End Class

Public Class StateEnumerationsSample : Inherits DataSourceHandler(Of Integer)

End Class

Public Interface IDataStorage : Inherits System.IDisposable

    Function WriteData(url As String) As Boolean
End Interface
