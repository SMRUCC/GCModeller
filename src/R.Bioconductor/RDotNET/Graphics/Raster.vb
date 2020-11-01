#Region "Microsoft.VisualBasic::8a7872d2cb96ba708a6601169bdbc2d6, RDotNET\Graphics\Raster.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class Raster
    ' 
    '         Properties: Height, Width
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics

    Public Class Raster
        Private raster As Color(,)

        Public Sub New(width As Integer, height As Integer)
            Me.raster = New Color(width - 1, height - 1) {}
        End Sub

        Default Public Property Item(x As Integer, y As Integer) As Color
            Get
                Return Me.raster(x, y)
            End Get
            Set
                Me.raster(x, y) = Value
            End Set
        End Property

        Public ReadOnly Property Width() As Integer
            Get
                Return Me.raster.GetLength(1)
            End Get
        End Property

        Public ReadOnly Property Height() As Integer
            Get
                Return Me.raster.GetLength(0)
            End Get
        End Property
    End Class
End Namespace
