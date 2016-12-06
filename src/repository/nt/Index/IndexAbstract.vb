Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public MustInherit Class IndexAbstract
    Implements INamedValue
    Implements IDisposable

    Dim __gi As String

    ''' <summary>
    ''' 只读
    ''' </summary>
    ''' <returns></returns>
    Public Property gi As String Implements INamedValue.Identifier
        Get
            Return __gi
        End Get
        Private Set(value As String)
            __gi = value
        End Set
    End Property

    Protected ReadOnly tab As Byte() = Encoding.ASCII.GetBytes(vbTab)
    Protected ReadOnly lf As Byte() = Encoding.ASCII.GetBytes(vbLf)

    Protected Sub New(uid$)
        gi = uid
    End Sub

    Public Overrides Function ToString() As String
        Return gi
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

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class