#Region "Microsoft.VisualBasic::0f82c16baacaa8fadc2da1bc6c56c382, GCModeller\data\GO_gene-ontology\obographs\obographs\Extensions.vb"

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
    '    Code Lines: 10
    ' Comment Lines: 6
    '   Blank Lines: 2
    '     File Size: 684 B


    ' Module Extensions
    ' 
    '     Function: DAGasTabular
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

<HideModuleName> Public Module Extensions

    ''' <summary>
    ''' 将构建出来的图对象转换为表格数据模型，以进行文件保存操作
    ''' </summary>
    ''' <param name="g"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function DAGasTabular(g As NetworkGraph) As NetworkTables
        Return g.Tabular(properties:={"relationship", "definition", "evidence", "is_obsolete", value_colors})
    End Function
End Module
