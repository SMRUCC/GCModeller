#Region "Microsoft.VisualBasic::64596130438f434a3dfc7ac76980f89e, visualize\Circos\Circos\ConfFiles\Nodes\Base\Rule.vb"

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

    '     Class ConditionalRule
    ' 
    '         Properties: color, condition
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Rules are used to dynamically alter the format of a data point, based on
    ''' the value of the data point (or any other property of it).
    '''
    ''' Each rule has a condition expression; any plot parameter may be placed inside
    ''' a rule to override the plot's default value for that parameter.
    '''
    ''' ```
    ''' &lt;rules>
    '''   &lt;rule>
    '''     condition    = var(value) > 0.6
    '''     color        = red
    '''     fill_color   = red
    '''   &lt;/rule>
    ''' &lt;/rules>
    ''' ```
    ''' </summary>
    Public Class ConditionalRule : Inherits CircosDocument
        Implements ICircosDocNode

        ''' <summary>
        ''' The rule trigger expression.(例如 ``var(value) &gt; 0.6``)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property condition As String = "var(value) > 0.6"
        ''' <summary>
        ''' 设置为 ``1`` 的时候，当本规则被触发之后便不再继续测试后续的其它规则
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property flow As String = null
        ''' <summary>
        ''' Rules are triggered in the order they are defined; this parameter can be used
        ''' to define the importance of a rule irrespective of its order in the block.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property importance As String = null
        ''' <summary>
        ''' Whether the rule condition Is evaluated with Perl eval() 
        ''' (``eval``, the Default) Or without it (``noeval``)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property [type] As String = null
        <Circos> Public Property color As String = "red"
        <Circos> Public Property fill_color As String = null
        <Circos> Public Property stroke_color As String = null
        <Circos> Public Property stroke_thickness As String = null
        <Circos> Public Property thickness As String = null
        <Circos> Public Property glyph As String = null
        <Circos> Public Property glyph_size As String = null
        ''' <summary>
        ''' The z-depth of the data point, larger value means drawn later(on the top).
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property z As String = null
        ''' <summary>
        ''' The new value of the data point, this allows you to remap the value scale.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property value As String = null

        Public Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Me.GenerateCircosDocumentElement("rule", IndentLevel, Nothing, directory)
        End Function
    End Class
End Namespace
