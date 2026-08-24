#Region "Microsoft.VisualBasic::0dd43b194049fddfa5fd403cc419e504, engine\Cella\Snapshots\Metadata.vb"

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

    '   Total Lines: 87
    '    Code Lines: 15 (17.24%)
    ' Comment Lines: 64 (73.56%)
    '    - Xml Docs: 95.31%
    ' 
    '   Blank Lines: 8 (9.20%)
    '     File Size: 2.81 KB


    ' Class Metadata
    ' 
    '     Properties: cells, depth, height, pathways, shape
    '                 time_frames, total_time, width
    ' 
    ' Class CellMetadata
    ' 
    '     Properties: ec_numbers, genes, taxonomy
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' 结果快照数据的元数据信息
''' </summary>
''' <remarks>
''' 结果快照数据在硬盘上的文件列表为：
''' 
''' ```
''' metadata.json
''' frame_1.json
''' frame_2.json
''' frame_3.json
''' ...
''' ```
''' 
''' 其中，metadata.json就是本对象的json序列化结果，而frame_xxx.json文件则是具体的数据帧快照<see cref="TimeFrameSnapshot"/>对象的json序列化结果
''' </remarks>
Public Class Metadata

    ''' <summary>
    ''' 总时间大小
    ''' </summary>
    ''' <returns></returns>
    Public Property total_time As Double
    ''' <summary>
    ''' 每一帧时间的数据快照对应的时间点，例如：
    ''' 
    ''' frame_1.json -> [0] 0.0min
    ''' frame_2.json -> [1] 1.0min
    ''' frame_3.json -> [2] 2.0min
    ''' </summary>
    ''' <returns></returns>
    Public Property time_frames As Double()

    ''' <summary>
    ''' 使用一个一维数组来表示一个三维空间的三维数组 boolean(,,)，<see cref="width"/>, <see cref="height"/>, <see cref="depth"/>标记了这个三维数组的维度信息。
    ''' 在这个三维空间中，采用boolean来标记模拟的空间形状，false表示空（不存在任何数据），true表示对应的位置是模拟环境空间的一部分
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1D index = (x * HEIGHT + y) * DEPTH + z
    ''' </remarks>
    Public Property shape As Boolean()

    ''' <summary>
    ''' width of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property width As Integer
    ''' <summary>
    ''' height of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property height As Integer
    ''' <summary>
    ''' depth of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property depth As Integer
    ''' <summary>
    ''' 进行模拟计算的细胞的种类信息，具体的虚拟细胞实例会依照这个细胞元数据信息进行实例化
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Dictionary(Of String, CellMetadata)
    ''' <summary>
    ''' [pathway_id => molecule_id array]
    ''' </summary>
    ''' <returns></returns>
    Public Property pathways As Dictionary(Of String, String())

End Class

Public Class CellMetadata

    Public Property taxonomy As String
    ''' <summary>
    ''' [gene_id => GO terms]
    ''' </summary>
    ''' <returns></returns>
    Public Property genes As Dictionary(Of String, String())
    ''' <summary>
    ''' [gene_id => EC numbers]
    ''' </summary>
    ''' <returns></returns>
    Public Property ec_numbers As Dictionary(Of String, String())

End Class
