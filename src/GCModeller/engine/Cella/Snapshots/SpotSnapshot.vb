#Region "Microsoft.VisualBasic::3f0ea3c6261fb0088d1959bc8c629f2c, engine\Cella\Snapshots\SpotSnapshot.vb"

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

    '   Total Lines: 30
    '    Code Lines: 9 (30.00%)
    ' Comment Lines: 19 (63.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (6.67%)
    '     File Size: 900 B


    ' Class SpotSnapshot
    ' 
    '     Properties: cells, externals, ph, temperature, x
    '                 y, z
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 某一个单元格在某一数据帧上的快照数据
''' </summary>
Public Class SpotSnapshot

    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer
    ''' <summary>
    ''' 当前单元格内的细胞列表
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As CellSnapshot()
    ''' <summary>
    ''' 当前单元格内的外部物质组成信息，[molecule_id => mass_contents]
    ''' </summary>
    ''' <returns></returns>
    Public Property externals As Dictionary(Of String, Double)
    ''' <summary>
    ''' 当前网格环境内的ph值
    ''' </summary>
    ''' <returns></returns>
    Public Property ph As Double
    ''' <summary>
    ''' 摄氏度为单位的温度
    ''' </summary>
    ''' <returns></returns>
    Public Property temperature As Double

End Class

