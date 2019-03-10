﻿#Region "Microsoft.VisualBasic::298c39cd72471e76319c602fdb884b2f, GCModeller.DataVisualization\ColorMgr.vb"

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

    ' Class ColorMgr
    ' 
    '     Properties: [Default]
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetEntityColor, GetValue, ToString
    ' 
    ' Class ClMap
    ' 
    '     Properties: Identifier, map
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The color manager
''' </summary>
Public Class ColorMgr

    ''' <summary>
    ''' <see cref="ClMap.Identifier"/> --> <see cref="ClMap.map"/>
    ''' </summary>
    ReadOnly __entityMaps As Dictionary(Of ClMap)
    ''' <summary>
    ''' <see cref="ClMap.map"/> --> <see cref="Color"/>
    ''' </summary>
    ReadOnly __colorMaps As MapsHelper(Of Color)
    ''' <summary>
    ''' Array of <see cref="ClMap.map"/>
    ''' </summary>
    ReadOnly __categories As String()

    Public ReadOnly Property [Default] As Color
        Get
            Return __colorMaps.Default
        End Get
    End Property

    Sub New(source As IEnumerable(Of ClMap), [default] As Color)
        __entityMaps = source.ToDictionary
        __categories = (From x As ClMap In source Select x.map Distinct).ToArray
        __colorMaps =
            New MapsHelper(Of Color)(__categories.GenerateColorProfiles, [default])
    End Sub

    ''' <summary>
    ''' <see cref="ClMap.Identifier"/>
    ''' </summary>
    ''' <param name="id"><see cref="ClMap.Identifier"/></param>
    ''' <returns></returns>
    Public Function GetEntityColor(id As String) As Color
        If __entityMaps.ContainsKey(id) Then
            Return __colorMaps.GetValue(__entityMaps(id).map)
        Else
            Return __colorMaps.Default
        End If
    End Function

    ''' <summary>
    ''' <see cref="ClMap.map"/>
    ''' </summary>
    ''' <param name="category"><see cref="ClMap.map"/></param>
    ''' <returns></returns>
    Public Function GetValue(category As String) As Color
        Return __colorMaps.GetValue(category)
    End Function

    Public Overrides Function ToString() As String
        Return __categories.GetJson
    End Function
End Class

''' <summary>
''' Entity to color mapper
''' </summary>
Public Class ClMap : Implements INamedValue

    Public Property Identifier As String Implements INamedValue.Key
    Public Property map As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
