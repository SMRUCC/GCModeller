﻿#Region "Microsoft.VisualBasic::fc32b5a79e4c18ae509ed53a9f52eec5, engine\Dynamics\Core\Mass\Factor.vb"

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

    '   Total Lines: 50
    '    Code Lines: 22 (44.00%)
    ' Comment Lines: 20 (40.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (16.00%)
    '     File Size: 1.58 KB


    '     Class Factor
    ' 
    '         Properties: ID, name, role
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

Namespace Core

    ''' <summary>
    ''' 一个变量因子，这个对象主要是用于存储值
    ''' </summary>
    Public Class Factor : Inherits Value(Of Double)
        Implements INamedValue

        ''' <summary>
        ''' the unique reference id of current molecule
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        ''' <summary>
        ''' 分子角色
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property role As MassRoles

        ''' <summary>
        ''' the molecule entity name, just used for debug view
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String

        Sub New()
            role = MassRoles.compound
        End Sub

        ''' <summary>
        ''' create a new mass factor inside the runtime environment with value assigned ZERO.
        ''' </summary>
        ''' <param name="id$"></param>
        ''' <param name="role"></param>
        Sub New(id$, role As MassRoles)
            Me.ID = id
            Me.role = role
        End Sub

        Public Overrides Function ToString() As String
            Return $"{If(name, ID)} ({Value} unit, {role.Description})"
        End Function
    End Class
End Namespace
