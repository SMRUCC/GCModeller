﻿#Region "Microsoft.VisualBasic::f4ae181d6a8e6536abf199259fe36a72, mime\application%json\Parser\JsonElement.vb"

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

    '     Class JsonModel
    ' 
    ' 
    ' 
    '     Class JsonElement
    ' 
    '         Function: [As], ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Parser

    Public MustInherit Class JsonModel : Inherits JsonElement

#Region "Json property and value"
        Default Public Property Item(str As String) As JsonElement
            Get
                Return CType(Me, JsonObject)(str)
            End Get
            Set(value As JsonElement)
                CType(Me, JsonObject)(str) = value
            End Set
        End Property

        Default Public Property Item(index As Integer) As JsonElement
            Get
                Return CType(Me, JsonArray)(index)
            End Get
            Set(value As JsonElement)
                CType(Me, JsonArray)(index) = value
            End Set
        End Property
#End Region

    End Class

    Public MustInherit Class JsonElement

        Public MustOverride Function BuildJsonString() As String

        Public Overrides Function ToString() As String
            Return "base::json"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function [As](Of T As JsonElement)() As T
            Return DirectCast(Me, T)
        End Function
    End Class
End Namespace
