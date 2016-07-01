Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports Oracle
Imports Oracle.LinuxCompatibility.MySQL

Namespace EngineSystem.Services.MySQL

    ''' <summary>
    ''' The base class of the service object, they all needs a MYSQL data exchange module in commonly.
    ''' (所有的服务引擎对象的基类，这个服务模块都需要一个与MYSQL数据库进行数据交换的模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Service : Inherits RuntimeObject
        Implements System.IDisposable

        ''' <summary>
        ''' The mysql service engine.(MYSQL数据库服务引擎)
        ''' </summary>
        ''' <remarks></remarks>
        Protected MYSQL As LinuxCompatibility.MySQL.MySQL

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

    End Class
End Namespace