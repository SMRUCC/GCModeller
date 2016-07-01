#Region "Microsoft.VisualBasic::3ac5641b87481bf6773b4137f25f807b, ..\GCModeller\visualize\GCModeller.DataVisualization\ColorMgr.vb"

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

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

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
            Return __colorMaps.__default
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
            Return __colorMaps.__default
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

Public Class ClMap : Implements sIdEnumerable

    Public Property Identifier As String Implements sIdEnumerable.Identifier
    Public Property map As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
