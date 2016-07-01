#Region "Microsoft.VisualBasic::ea00f6dde9f55e8f6e506bf3df2557ba, ..\interops\visualize\Circos\Circos\Colors\Mappings.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Namespace Colors

    Public Structure Mappings
        Public Property value As Double
        Public Property Level As Integer
        Public Property Color As Color

        Public ReadOnly Property CircosColor As String
            Get
                Return $"({Color.R},{Color.G},{Color.B})"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{value} ==> {Level}   @{Color.ToString}"
        End Function
    End Structure
End Namespace
