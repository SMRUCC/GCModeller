#Region "Microsoft.VisualBasic::f6f1a3622566e0b9689e5acea34240a9, models\BioCyc\Models\Reflection\AttributeField.vb"

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

    '   Total Lines: 28
    '    Code Lines: 21 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (25.00%)
    '     File Size: 777 B


    ' Class AttributeField
    ' 
    '     Properties: name
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getMappingKey, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices

<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
Public Class AttributeField : Inherits Attribute

    Public ReadOnly Property name As String

    Sub New(name As String)
        Me.name = name
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return name
    End Function

    Friend Shared Function getMappingKey(p As PropertyInfo) As String
        Dim attrs = p.GetCustomAttributes(Of AttributeField)().ToArray

        If attrs.IsNullOrEmpty Then
            Return Nothing
        Else
            Return attrs(Scan0).name
        End If
    End Function

End Class
