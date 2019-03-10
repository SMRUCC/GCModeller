﻿#Region "Microsoft.VisualBasic::c4bb460cea8150dbec72af12b2a03dee, gr\network-visualization\Datavisualization.Network\IO\Generic\IDynamicsTable.vb"

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

    '     Class IDynamicsTable
    ' 
    '         Properties: Properties
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace FileStream.Generic

    ''' <summary>
    ''' The network graph object contains the dynamics property for contains the extra information of the object.
    ''' </summary>
    Public MustInherit Class IDynamicsTable : Inherits DynamicPropertyBase(Of String)

        ''' <summary>
        ''' The dynamics property table of this network component
        ''' </summary>
        ''' <returns></returns>
        <Meta(GetType(String))> Public Overrides Property Properties As Dictionary(Of String, String)
            Get
                Return MyBase.Properties
            End Get
            Set(value As Dictionary(Of String, String))
                MyBase.Properties = value
            End Set
        End Property

        Public Sub Add(key As String, value As String)
            Call Properties.Add(key, value)
        End Sub
    End Class
End Namespace
