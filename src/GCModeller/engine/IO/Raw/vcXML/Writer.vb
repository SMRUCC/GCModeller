Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace vcXML

    Public Class Writer : Implements IDisposable

        Dim frameCount As i32 = 1
        Dim fs As StreamWriter

        Sub New(file As String)
            fs = file.OpenWriter(Encodings.UTF8WithoutBOM)
        End Sub

        Public Sub addFrame(time As Double, module$, type$, data As IEnumerable(Of Double))
            Dim vecBase64$ = encode(data)
            Dim frame As New frame With {
                .frameTime = time,
                .[module] = [module],
                .num = ++frameCount,
                .vector = New vector With {
                    .data = vecBase64,
                    .contentType = type
                }
            }
        End Sub

        Private Function encode(data As IEnumerable(Of Double)) As String

        End Function

        Private Sub writeIndex()

        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call writeIndex()
                    Call fs.Flush()
                    Call fs.Close()
                    Call fs.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
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