#Region "Microsoft.VisualBasic::8814c4f37e5aeb1a5da7931fe4930c21, GCModeller\engine\Dynamics\Core\Flux\Boundary.vb"

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


    ' Code Statistics:

    '   Total Lines: 34
    '    Code Lines: 21
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 1007 B


    '     Class Boundary
    ' 
    '         Properties: forward, reverse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core

    Public Class Boundary

        Public Property forward As Double

        ''' <summary>
        ''' 反向的上限为零的售后表示不可逆反应
        ''' </summary>
        ''' <returns></returns>
        Public Property reverse As Double

        Sub New()
        End Sub

        Sub New(forward As Double, reverse As Double)
            Me.forward = forward
            Me.reverse = reverse
        End Sub

        Public Overrides Function ToString() As String
            Return $"[reactant <- {reverse} | {forward} -> product]"
        End Function

        Public Shared Widening Operator CType(range As Double()) As Boundary
            Return New Boundary With {.forward = range(0), .reverse = range(1)}
        End Operator

        Public Shared Widening Operator CType(range As Integer()) As Boundary
            Return New Boundary With {.forward = range(0), .reverse = range(1)}
        End Operator

    End Class
End Namespace
