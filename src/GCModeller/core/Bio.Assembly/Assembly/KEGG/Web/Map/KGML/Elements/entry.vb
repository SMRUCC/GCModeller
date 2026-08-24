#Region "Microsoft.VisualBasic::f4ef40851b884d1bbff885ac9282a3e2, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Elements\entry.vb"

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

    '   Total Lines: 62
    '    Code Lines: 15 (24.19%)
    ' Comment Lines: 41 (66.13%)
    '    - Xml Docs: 95.12%
    ' 
    '   Blank Lines: 6 (9.68%)
    '     File Size: 3.69 KB


    '     Class entry
    ' 
    '         Properties: graphics, id, link, name, reaction
    '                     type
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' Network nodes
    ''' </summary>
    ''' <remarks>
    ''' &lt;entry>元素表示通路中的一个“节点”或“条目”，是构建代谢网络图的基本单元。每个&lt;entry>都有一个唯一的ID（通过id属性指定）和一个或多个名称（通过name属性指定），
    ''' 以及该条目的类型（通过type属性指定）。根据type的不同，&lt;entry>可以代表不同类型的生物学实体。
    ''' 
    ''' 每个&lt;entry>元素的核心作用是定义通路中的一个节点实体。例如，在糖酵解通路中，一个&lt;entry>可以表示己糖激酶（基因/酶），另一个可以表示葡萄糖-6-磷酸（代谢物）。
    ''' 节点之间通过&lt;reaction>和&lt;relation>元素连接，从而形成网络。
    ''' </remarks>
    Public Class entry : Implements INamedValue

        ''' <summary>
        ''' 该条目的唯一标识符（在当前通路中唯一），是一个正整数。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String Implements INamedValue.Key
        ''' <summary>
        ''' 该条目对应的KEGG数据库标识符，通常以“数据库:ID”的形式表示。例如，name="hsa:124"表示人类基因ID 124，name="cpd:C00031"表示化合物C00031（葡萄糖）。name属性可以包含多个值（多个基因或化合物），多个值之间用空格分隔。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String()

        ''' <summary>
        ''' 条目的类型，如gene、compound等，用于标识该节点的生物学类别。entry的类别包括有：
        ''' 
        ''' + gene：表示基因产物（通常是蛋白质），是代谢通路中的酶或其它蛋白质因子。
        ''' + enzyme：表示酶（有时与gene类似，但在参考通路中特指以EC编号表示的酶）。
        ''' + ortholog：表示直系同源群（以KO编号表示），在参考通路中代表一个保守的功能模块，可能在具体生物体中由一个或多个基因编码。
        ''' + compound：表示化学代谢物，是代谢反应的底物或产物。
        ''' + map：表示另一个通路的链接，相当于一个嵌套的子图节点（通常用于连接相关的通路图）。
        ''' + other：其它未分类的节点类型。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As String
        ''' <summary>
        ''' 可选属性，提供该条目在KEGG数据库中的链接地址（例如基因或化合物的详情页面）。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property link As String
        ''' <summary>
        ''' 可选属性，对于某些基因/酶类型的节点，可以指定其催化或参与的化学反应ID（例如reaction="rn:R00710"）。这通常用于酶节点，以表示该酶参与的反应。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property reaction As String
        ''' <summary>
        ''' 一个或多个&lt;graphics>子元素，用于描述该条目在官方通路图中的图形表示。
        ''' </summary>
        ''' <returns></returns>
        Public Property graphics As graphics

        Public Overrides Function ToString() As String
            Return name.JoinBy("; ")
        End Function
    End Class

End Namespace
