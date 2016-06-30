Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.GCModeller.DataVisualization

Namespace ComponentModel

    Public Class DrawingDevice : Implements IDisposable

        Dim _GraphicDevice As Graphics, _ImageData As Bitmap
        Dim _ColorProfiles As ColorProfiles

        Public ReadOnly Property GraphicDevice As Graphics
            Get
                Return _GraphicDevice
            End Get
        End Property

        Public ReadOnly Property Image As Bitmap
            Get
                Return _ImageData
            End Get
        End Property

        Public ReadOnly Property Color(Name As String) As Color
            Get
                Return _ColorProfiles(Name)
            End Get
        End Property

        Sub New(Width As Integer, Height As Integer, ColorProfiles As Generic.IEnumerable(Of String), Optional DefaultColor As Color = Nothing)
            _ColorProfiles = New ColorProfiles(ColorProfiles, DefaultColor)
            _ImageData = New Bitmap(Width, Height)
            _GraphicDevice = Graphics.FromImage(_ImageData)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("<{0}, {1}>", _ImageData.Width, _ImageData.Height)
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

    End Class
End Namespace