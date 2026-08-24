#Region "Microsoft.VisualBasic::b258f825d057b0203f5e8ec5ed8f486b, engine\Cella\Snapshots\CellSnapshot.vb"

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

    '   Total Lines: 27
    '    Code Lines: 9 (33.33%)
    ' Comment Lines: 15 (55.56%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (11.11%)
    '     File Size: 803 B


    ' Class CellSnapshot
    ' 
    '     Properties: cell_id, is_alive, metabolite, parent_id, protein
    '                 rna, taxonomy
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 具体的细胞实例对象的快照数据
''' </summary>
Public Class CellSnapshot

    ''' <summary>
    ''' the cell unique id
    ''' </summary>
    ''' <returns></returns>
    Public Property cell_id As String
    ''' <summary>
    ''' parent cell its <see cref="cell_id"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property parent_id As String

    ''' <summary>
    ''' organism taxonomy info of current cell genome
    ''' </summary>
    ''' <returns></returns>
    Public Property taxonomy As String
    Public Property rna As Dictionary(Of String, Double)
    Public Property protein As Dictionary(Of String, Double)
    Public Property metabolite As Dictionary(Of String, Double)
    Public Property is_alive As Boolean

End Class

