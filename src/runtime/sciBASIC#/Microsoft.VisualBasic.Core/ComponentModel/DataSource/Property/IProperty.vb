﻿#Region "Microsoft.VisualBasic::9323d6cc7d0a0aa572c751ef79fd2eab, Microsoft.VisualBasic.Core\ComponentModel\DataSource\Property\IProperty.vb"

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

    '     Interface IProperty
    ' 
    '         Function: GetValue
    ' 
    '         Sub: SetValue
    ' 
    '     Interface IDynamicMeta
    ' 
    '         Properties: Properties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DataSourceModel

    Public Interface IProperty : Inherits IReadOnlyId

        ''' <summary>
        ''' Gets property value from <paramref name="target"/> object.
        ''' </summary>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Function GetValue(target As Object) As Object

        ''' <summary>
        ''' Set <paramref name="value"/> to the property of <paramref name="target"/> object.
        ''' </summary>
        ''' <param name="target"></param>
        ''' <param name="value"></param>
        Sub SetValue(target As Object, value As Object)
    End Interface

    ''' <summary>
    ''' Abstracts for the dynamics property.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Interface IDynamicMeta(Of T)

        ''' <summary>
        ''' Properties
        ''' </summary>
        ''' <returns></returns>
        Property Properties As Dictionary(Of String, T)
    End Interface
End Namespace
