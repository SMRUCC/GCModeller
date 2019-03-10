Namespace Runtime.SCOM

    ''' <summary>
    ''' This type of the class object consist of the shoal shell scripting engine.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class RuntimeComponent : Implements System.IDisposable

        ''' <summary>
        ''' Script engine.(脚本引擎)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ScriptEngine As ScriptEngine

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Me.ScriptEngine = ScriptEngine
        End Sub

        Public Overrides Function ToString() As String
            Return ScriptEngine.ToString & "::" & Me.GetType.FullName
        End Function

#Region "IDisposable Support"
        Protected disposedValue As Boolean ' To detect redundant calls

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
End Namespace