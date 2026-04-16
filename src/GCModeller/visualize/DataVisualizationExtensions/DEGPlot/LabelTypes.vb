#Region "Microsoft.VisualBasic::5456002c101cb357435e225c4bb149c9, visualize\DataVisualizationExtensions\DEGPlot\LabelTypes.vb"

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

    '   Total Lines: 17
    '    Code Lines: 12 (70.59%)
    ' Comment Lines: 3 (17.65%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (11.76%)
    '     File Size: 323 B


    ' Enum LabelTypes
    ' 
    '     ALL, Custom, DEG, None
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum DEGSet
    ' 
    '     ALL, Down, Up
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Enum LabelTypes
    None
    ''' <summary>
    ''' <see cref="DEGModel.label"/>不为空字符串的时候就会被显示出来
    ''' </summary>
    Custom
    ALL
    DEG
End Enum

Public Enum DEGSet
    ALL
    Up
    Down
End Enum
