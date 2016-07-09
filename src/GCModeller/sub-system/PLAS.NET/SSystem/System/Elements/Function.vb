#Region "Microsoft.VisualBasic::11bf798a81d3d2e99501fbc65d05799e, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Elements\Function.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' User define function
    ''' </summary>
    Public Structure [Function] : Implements sIdEnumerable

        ''' <summary>
        ''' The function name
        ''' </summary>
        <XmlAttribute> Public Property Name As String Implements sIdEnumerable.Identifier

        ''' <summary>
        ''' [function name](args) expression
        ''' </summary>
        <XmlAttribute> Public Property Declaration As String

        Public Overrides Function ToString() As String
            Return $"{Name} <- {Declaration}"
        End Function

        Public Shared Widening Operator CType(s As String) As [Function]
            Dim Tokens = s.Split
            Dim [Function] = New [Function]

            [Function].Name = Tokens(1)
            [Function].Declaration = Mid(s, 7 + Len([Function].Name))

            Return [Function]
        End Operator
    End Structure
End Namespace
