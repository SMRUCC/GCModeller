#Region "Microsoft.VisualBasic::68b1ea8e4e4e0377a6588a437bd7f6b1, GCModeller\models\BioCyc\Models\Reflection\ElementFactory.vb"

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

    '   Total Lines: 19
    '    Code Lines: 14
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 618 B


    ' Class ElementFactory
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CastTo
    ' 
    '     Sub: Register
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Public NotInheritable Class ElementFactory

    Private Sub New()
    End Sub

    Shared ReadOnly registry As New Dictionary(Of Type, Func(Of ValueString, Object))

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Register(Of T)(factory As Func(Of ValueString, Object))
        registry(GetType(T)) = factory
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CastTo(value As ValueString, template As Type) As Object
        Return registry(template)(value)
    End Function
End Class

