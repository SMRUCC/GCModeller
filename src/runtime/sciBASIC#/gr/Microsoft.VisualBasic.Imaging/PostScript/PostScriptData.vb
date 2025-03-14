Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Net.Http

Namespace PostScript

    Public Class PostScriptData : Inherits GraphicsData

        Public Overrides ReadOnly Property Driver As Drivers
            Get
                Return Drivers.PostScript
            End Get
        End Property

        Public Sub New(img As Object, size As Size, padding As Padding)
            MyBase.New(img, size, padding)
        End Sub

        Public Overrides Function GetDataURI() As DataURI
            Throw New NotImplementedException()
        End Function

        Public Overrides Function Save(out As Stream) As Boolean
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace