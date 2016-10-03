#Region "Microsoft.VisualBasic::bd1b09c55cd3fc4723bdeb6bda0cd978, ..\R.Bioconductor\RDotNET\Graphics\RasterExtension.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Namespace Graphics

    Public Module RasterExtension
      
        <System.Runtime.CompilerServices.Extension> _
        Public Function CreateIntegerMatrix(engine As REngine, raster As Raster) As IntegerMatrix
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
            For x As Integer = 0 To width - 1
                For y As Integer = 0 To height - 1
                    matrix(x, y) = ToInteger(raster(x, y))
                Next
            Next

            Return matrix
        End Function

        Private Function ToInteger(color As Color) As Integer
            Return (color.Alpha << 24) Or (color.Blue << 16) Or (color.Green << 8) Or color.Red
        End Function
    End Module
End Namespace
