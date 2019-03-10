#Region "Microsoft.VisualBasic::35dd620611872da44adb783a7316bcfa, sub-system\PLAS.NET\SSystem\System\Elements\Function.vb"

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

    '     Structure [Function]
    ' 
    '         Properties: Declaration, Name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' User define function
    ''' </summary>
    Public Structure [Function] : Implements INamedValue

        ''' <summary>
        ''' The function name
        ''' </summary>
        <XmlAttribute> Public Property Name As String Implements INamedValue.Key

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
