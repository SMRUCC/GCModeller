#Region "Microsoft.VisualBasic::2783b2b1ba8a79331b36f6fae4ac1061, ..\visualize\ChromosomeMap\DrawingModels\Abstract.vb"

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

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports SMRUCC.genomics.Visualize.ChromosomeMap.FootprintMap

Namespace DrawingModels

    ''' <summary>
    ''' 基因组之中的一个位点
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Site

        Public Property SiteName As String
        Public Property Comments As String
        Public Property Left As Integer
        Public Property Right As Integer

        Public Overridable ReadOnly Property Width As Integer
            Get
                Return Math.Abs(Left - Right)
            End Get
        End Property

        Public MustOverride Sub Draw(Device As Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)

        Public Overrides Function ToString() As String
            Return SiteName
        End Function
    End Class
End Namespace
