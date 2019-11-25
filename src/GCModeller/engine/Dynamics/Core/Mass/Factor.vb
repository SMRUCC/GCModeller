#Region "Microsoft.VisualBasic::53a7de3bb7e3d07a8bc83024df262026, Dynamics\Core\Mass\Factor.vb"

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

    '     Class Factor
    ' 
    '         Properties: ID
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

Namespace Core

    ''' <summary>
    ''' 一个变量因子，这个对象主要是用于存储值
    ''' </summary>
    Public Class Factor : Inherits Value(Of Double)
        Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key

        Public Overrides Function ToString() As String
            Return $"{ID} ({Value} unit)"
        End Function

        Public Overloads Shared Widening Operator CType(name As String) As Factor
            Return New Factor With {.ID = name}
        End Operator
    End Class
End Namespace
