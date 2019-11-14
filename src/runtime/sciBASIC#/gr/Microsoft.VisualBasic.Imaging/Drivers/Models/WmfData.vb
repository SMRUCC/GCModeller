Imports System.Drawing
Imports System.IO

Namespace Driver

    Public Class WmfData : Inherits GraphicsData

        Public Overrides ReadOnly Property Driver As Drivers
            Get
                Return Drivers.WMF
            End Get
        End Property

        ReadOnly tmpfile As String = App.GetAppSysTempFile(".wmf", App.PID, RandomASCIIString(10, skipSymbols:=True))

        Public Sub New(img As Object, size As Size)
            MyBase.New(img, size)
        End Sub

        Public Overrides Function Save(path As String) As Boolean
            Return tmpfile.FileCopy(path)
        End Function

        Public Overrides Function Save(out As Stream) As Boolean
            Using reader As FileStream = tmpfile.Open(FileMode.Open, doClear:=False)
                Call out.Seek(Scan0, SeekOrigin.Begin)
                Call reader.CopyTo(out)
            End Using

            Return True
        End Function
    End Class
End Namespace