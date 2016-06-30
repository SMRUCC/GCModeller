
''' <summary>
''' 数据存储器
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="TDataSource"></typeparam>
''' <remarks></remarks>
Public MustInherit Class DataStorage(Of T, TDataSource As DataStorage.FileModel.DataSerials(Of T))
    Implements IDisposable
    Implements Framework.Kernel_Driver.IDataStorage

    Public MustOverride Function WriteData(chunkbuffer As IEnumerable(Of TDataSource), url As String) As Boolean

    Public Function WriteData(driver As IDriver_DataSource_Adapter(Of T), url As String) As Boolean
        Return WriteData(chunkbuffer:=driver.get_DataSerials, url:=url)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public MustOverride Function WriteData(url As String) As Boolean Implements IDataStorage.WriteData
End Class

