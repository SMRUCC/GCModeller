#Region "Microsoft.VisualBasic::7ca7e430af074c1866c8d4df02ef2156, ..\workbench\d3js\Hierarchical-Edge-Bundling\Hierarchical-Edge-Bundling\FlareImports.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization

Public Structure FlareImports : Implements sIdEnumerable

    Public Property name As String Implements sIdEnumerable.Identifier
    Public Property size As Integer
    ''' <summary>
    ''' 与本节点对象<see cref="name"/>相连接的节点对象的标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property [imports] As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

