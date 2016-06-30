Imports Microsoft.VisualBasic

''' <summary>
''' 数据采集器
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="TDataSource"></typeparam>
''' <remarks></remarks>
Public Class DataAdapter(Of T, TDataSource As DataSourceHandler(Of T))

    ''' <summary>
    ''' 本列表之中的所有数据都不会被记录下来
    ''' </summary>
    ''' <remarks></remarks>
    Protected _filtedHandles As Long() = New Long() {}
    Protected ReadOnly _innerBuffer As List(Of TDataSource) = New List(Of TDataSource)

    Public Function FetchData(objectHandlers As Generic.IEnumerable(Of DataStorage.FileModel.ObjectHandle)) As DataStorage.FileModel.DataSerials(Of T)()
        Dim TimeSerials = (From data0Expr As TDataSource In _innerBuffer
                           Select data0Expr
                           Group data0Expr By data0Expr.Handle Into Group
                           Order By Handle Ascending).ToArray
        Dim [handles] As Long() = (From data0Expr In TimeSerials Select data0Expr.Handle).ToArray
        objectHandlers = (From hwnd As Long In [handles]
                          Let arraySel = (From objHwnd In objectHandlers Where objHwnd.Handle = hwnd Select objHwnd).FirstOrDefault
                          Select arraySel).ToArray
        Dim LQuery = (From objHwnd As DataStorage.FileModel.ObjectHandle
                      In objectHandlers.AsParallel
                      Let ChunkBuffer = (From data0Expr In TimeSerials Where data0Expr.Handle = objHwnd.Handle Select data0Expr).First
                      Let sampleSource As TDataSource() = (From DsGroup As TDataSource In ChunkBuffer.Group Select DsGroup Order By DsGroup.TimeStamp Ascending).ToArray
                      Let SampleValue As T() = (From data0Expr As TDataSource In sampleSource Select data0Expr.Value).ToArray
                      Let row = New DataStorage.FileModel.DataSerials(Of T) With {
                          .Handle = objHwnd.Handle,
                          .UniqueId = objHwnd.Identifier,
                          .Samples = SampleValue
                      }
                      Select row).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 清除数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        Call _innerBuffer.Clear()
    End Sub

    ''' <summary>
    ''' 空值会自动清除过滤器的句柄列表
    ''' </summary>
    ''' <param name="lstHwnd"></param>
    Public Sub SetFiltedHandles(lstHwnd As Long())
        If lstHwnd Is Nothing Then
            lstHwnd = New Int64() {}
        End If
        _filtedHandles = lstHwnd
    End Sub

    Public Sub DataAcquiring(chunkBuffer As Generic.IEnumerable(Of TDataSource))
        Call _innerBuffer.AddRange(__filterData(chunkBuffer, Me._filtedHandles))
    End Sub

    Private Shared Function __filterData(chunkBuffer As Generic.IEnumerable(Of TDataSource), filters As Long()) As TDataSource()
        Return (From data0Expr As TDataSource
                In chunkBuffer.AsParallel
                Where Array.IndexOf(filters, data0Expr.Handle) = -1
                Select data0Expr).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return "CHUNK_SIZE:= " & _innerBuffer.Count
    End Function
End Class
