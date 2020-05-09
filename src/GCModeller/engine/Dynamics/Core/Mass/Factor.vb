#Region "Microsoft.VisualBasic::e8aa4e630aaf5f25731a119ae6dd6af8, Dynamics\Core\Mass\Factor.vb"

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
    '         Properties: hashCode, ID
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

        ''' <summary>
        ''' 分子角色
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property role As MassRoles

        ''' <summary>
        ''' debug view
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property hashCode As Integer
            Get
                Return Me.GetHashCode
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{ID} ({Value} unit)"
        End Function
    End Class
End Namespace
