#Region "Microsoft.VisualBasic::db95c54309d88d299ae4a6daba62a824, engine\Cella\Snapshots\TimeFrameSnapshot.vb"

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

    '   Total Lines: 18
    '    Code Lines: 5 (27.78%)
    ' Comment Lines: 11 (61.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (11.11%)
    '     File Size: 570 B


    ' Class TimeFrameSnapshot
    ' 
    '     Properties: cells, environment, time
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' frame_xxx.json
''' </summary>
Public Class TimeFrameSnapshot

    ''' <summary>
    ''' <see cref="Metadata.shape"/>中为true的单元格位置上的实例快照数据
    ''' </summary>
    ''' <returns></returns>
    Public Property environment As SpotSnapshot()
    Public Property time As Double
    ''' <summary>
    ''' count of the cell in <see cref="CellSnapshot.taxonomy"/> group, [taxonomy => cell_count]
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Dictionary(Of String, Integer)

End Class

