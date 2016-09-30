Imports System.Diagnostics
Imports System.IO

''' <summary>
''' ``gnuplot.exe``
''' </summary>
Public Class Interop : Implements IDisposable

    Public Const GNUplotDefault As String = "C:\Program Files (x86)\gnuplot\bin\gnuplot.exe"

    Public ReadOnly Property GNUplot As New Process
    ''' <summary>
    ''' The standard inputs of <see cref="GNUplot"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property std_in As StreamWriter

    ''' <summary>
    ''' Default: ``<see cref="GNUplotDefault"/>``
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property PathToGnuplot As String

    Public ReadOnly Property Available As Boolean
        Get
            Return PathToGnuplot.FileExists
        End Get
    End Property

    Sub New(Optional gnuplot$ = GNUplotDefault)
        PathToGnuplot = gnuplot
    End Sub

    ''' <summary>
    ''' Execute the input commandline immediately.
    ''' </summary>
    ''' <param name="cmd$"></param>
    Public Sub Invoke(cmd$)
        Call std_in.WriteLine(cmd)
        Call std_in.Flush()
    End Sub

    Public Sub Write(cmd$)
        Call std_in.Write(cmd)
    End Sub

    Public Sub WriteLine(cmd$)
        Call std_in.WriteLine(cmd)
    End Sub

    Public Sub Flush()
        Call std_in.Flush()
    End Sub

    Public Function Start() As Boolean
        If PathToGnuplot.FileExists Then
            GNUplot.StartInfo.FileName = PathToGnuplot
            GNUplot.StartInfo.UseShellExecute = False
            GNUplot.StartInfo.RedirectStandardInput = True
            GNUplot.Start()

            _std_in = GNUplot.StandardInput

            Return True
        Else
            Return False
        End If
    End Function

    Public Overrides Function ToString() As String
        Return PathToGnuplot.ToFileURL
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call std_in.Close()
                Call std_in.Dispose()
                Call GNUplot.Kill()
                Call GNUplot.Close()
                Call GNUplot.Dispose()
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
