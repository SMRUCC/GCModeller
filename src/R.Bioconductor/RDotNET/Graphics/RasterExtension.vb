Imports System
Imports System.Runtime.CompilerServices

Namespace Graphics
    Public Module RasterExtension
        <Extension()>
        Public Function CreateIntegerMatrix(ByVal engine As REngine, ByVal raster As Raster) As IntegerMatrix
            If engine Is Nothing Then
                Throw New ArgumentNullException("engine")
            End If

            If Not engine.IsRunning Then
                Throw New InvalidOperationException()
            End If

            If raster Is Nothing Then
                Throw New ArgumentNullException("raster")
            End If

            Dim width = raster.Width
            Dim height = raster.Height
            Dim matrix = New IntegerMatrix(engine, height, width)

            For x = 0 To width - 1

                For y = 0 To height - 1
                    matrix(x, y) = ToInteger(raster(x, y))
                Next
            Next

            Return matrix
        End Function

        Private Function ToInteger(ByVal color As Color) As Integer
            Return color.Alpha << 24 Or color.Blue << 16 Or color.Green << 8 Or color.Red
        End Function
    End Module
End Namespace
