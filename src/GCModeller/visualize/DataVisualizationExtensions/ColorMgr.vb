#Region "Microsoft.VisualBasic::155444b1e4a35fd181305605ea02136c, GCModeller\visualize\DataVisualizationExtensions\ColorMgr.vb"

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

    '   Total Lines: 76
    '    Code Lines: 41
    ' Comment Lines: 25
    '   Blank Lines: 10
    '     File Size: 2.40 KB


    ' Class ColorMgr
    ' 
    '     Properties: [Default]
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetEntityColor, GetValue, ToString
    ' 
    ' Class ColorMap
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
    ''' <see cref="ColorMap.Identifier"/> --> <see cref="ColorMap.map"/>
    ''' </summary>
    ReadOnly __entityMaps As Dictionary(Of ColorMap)
    ''' <summary>
    ''' <see cref="ColorMap.map"/> --> <see cref="Color"/>
    ''' </summary>
    ReadOnly __colorMaps As MapsHelper(Of Color)
    ''' <summary>
    ''' Array of <see cref="ColorMap.map"/>
    ''' </summary>
    ReadOnly __categories As String()

    Public ReadOnly Property [Default] As Color
        Get
            Return __colorMaps.Default
        End Get
    End Property

    Sub New(source As IEnumerable(Of ColorMap), [default] As Color)
        __entityMaps = source.ToDictionary
        __categories = (From x As ColorMap In source Select x.map Distinct).ToArray
        __colorMaps =
            New MapsHelper(Of Color)(__categories.GenerateColorProfiles, [default])
    End Sub

    ''' <summary>
    ''' <see cref="ColorMap.Identifier"/>
    ''' </summary>
    ''' <param name="id"><see cref="ColorMap.Identifier"/></param>
    ''' <returns></returns>
    Public Function GetEntityColor(id As String) As Color
        If __entityMaps.ContainsKey(id) Then
            Return __colorMaps.GetValue(__entityMaps(id).map)
        Else
            Return __colorMaps.Default
        End If
    End Function

    ''' <summary>
    ''' <see cref="ColorMap.map"/>
    ''' </summary>
    ''' <param name="category"><see cref="ColorMap.map"/></param>
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
Public Class ColorMap : Implements INamedValue

    Public Property Identifier As String Implements INamedValue.Key
    Public Property map As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
