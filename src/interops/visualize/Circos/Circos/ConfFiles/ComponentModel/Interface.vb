#Region "Microsoft.VisualBasic::400ed731d2bd36ead7c6e02380ad131f, visualize\Circos\Circos\ConfFiles\ComponentModel\Interface.vb"

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

    '     Class CircosDocument
    ' 
    ' 
    ' 
    '     Interface ICircosDocNode
    ' 
    '         Function: Build
    ' 
    '     Interface ICircosDocument
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel

Namespace Configurations.ComponentModel

    Public MustInherit Class CircosDocument : Implements ICircosDocNode
        Public MustOverride Function Build(indents As Integer, directory As String) As String Implements ICircosDocNode.Build

    End Class

    ''' <summary>
    ''' The circos allowed top level configuration block names.
    ''' </summary>
    ''' <remarks>
    ''' See the source file ``lib/Circos/Configuration.pm`` in the circos distribution:
    '''
    ''' ```perl
    ''' my @ok = qw(ideogram colors fonts patterns image plots links highlights);
    ''' ```
    '''
    ''' Please notice that the ``link`` Is Not a valid ``&lt;plot>`` type, it must be
    ''' placed inside the independent top level ``&lt;links>`` block instead of the ``&lt;plots>`` block.
    ''' (See ``lib/Circos/Track.pm``: ``our @type_ok = qw(scatter line histogram heatmap highlight tile text connector)``)
    ''' </remarks>
    Public Module CircosBlocks

        ''' <summary>
        ''' ``&lt;plots>``: 二维数据绘图元素的容器(scatter/line/histogram/heatmap/tile/text/connector/highlight)
        ''' </summary>
        Public Const plots As String = "plots"
        ''' <summary>
        ''' ``&lt;links>``: 连接关系的容器(&lt;link>)
        ''' </summary>
        Public Const links As String = "links"
        ''' <summary>
        ''' ``&lt;highlights>``: 高亮区域的顶层容器(&lt;highlight>)
        ''' </summary>
        Public Const highlights As String = "highlights"
    End Module

    ''' <summary>
    ''' This object can be convert to text document by using method <see cref="Build"/>
    ''' </summary>
    Public Interface ICircosDocNode

        ''' <summary>
        ''' Build doc and save components include files
        ''' </summary>
        ''' <param name="indents"></param>
        ''' <param name="directory">The root directory of ``circos.conf`` file.</param>
        ''' <returns></returns>
        Function Build$(indents%, directory$)
    End Interface

    ''' <summary>
    ''' This object can be save as a text doc for the circos plot
    ''' </summary>
    Public Interface ICircosDocument : Inherits ICircosDocNode, ISaveHandle
    End Interface
End Namespace
