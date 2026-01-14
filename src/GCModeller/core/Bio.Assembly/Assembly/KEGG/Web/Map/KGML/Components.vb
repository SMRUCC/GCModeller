#Region "Microsoft.VisualBasic::da0f319a18c3f39d5770a83d36b8ef4c, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Components.vb"

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

'   Total Lines: 78
'    Code Lines: 55 (70.51%)
' Comment Lines: 6 (7.69%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 17 (21.79%)
'     File Size: 2.34 KB


'     Class Link
' 
'         Properties: entry1, entry2, type
' 
'     Class relation
' 
'         Properties: subtype
' 
'     Class subtype
' 
'         Properties: name, value
' 
'     Class compound
' 
'         Properties: id, name
' 
'         Function: ToString
' 
'     Class reaction
' 
'         Properties: products, substrates
' 
'         Function: ToString
' 
'     Class entry
' 
'         Properties: graphics, id, link, name, type
' 
'         Function: ToString
' 
'     Class graphics
' 
'         Properties: bgcolor, fgcolor, height, name, type
'                     width, x, y
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' Network edges
    ''' </summary>
    Public Class Link

        <XmlAttribute> Public Property type As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    ''' <summary>
    ''' &lt;relation>元素表示通路中两个节点之间的关联关系，主要用于描述蛋白质（或基因产物）之间的相互作用，以及蛋白质与代谢物之间的调控关系。与&lt;reaction>描述化学转化不同，
    ''' &lt;relation>更侧重于调控和相互作用，例如一个蛋白质激活或抑制另一个蛋白质，或一个蛋白质结合一个代谢物等。&lt;relation>元素在KGML中通常用于表示蛋白质网络（protein network）
    ''' 或调控网络，即以蛋白质/基因为节点、调控关系为边的网络。
    ''' </summary>
    Public Class relation : Inherits Link

        ''' <summary>
        ''' 关系的源节点ID，即参与该关系的前一个节点。该属性值对应一个&lt;entry>的id。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property entry1 As String
        ''' <summary>
        ''' 关系的目标节点ID，即参与该关系的后一个节点。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property entry2 As String

        ''' <summary>
        ''' 一个或多个&lt;subtype>子元素，用于进一步细化关系的性质。每个&lt;subtype>有两个属性：
        ''' 
        ''' name表示子类型名称，value表示该子类型的具体值（通常用符号表示）。例如，&lt;subtype name="activation" value="-->"/>表示从entry1到entry2的激活关系，用箭头-->表示方向性。
        ''' 再如，&lt;subtype name="inhibition" value="--|"/>表示抑制关系，用T型线--|表示。通过subtype，可以明确关系的正负激活/抑制性质以及方向性。
        ''' </summary>
        ''' <returns></returns>
        Public Property subtype As subtype

    End Class

    Public Class subtype

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As String

    End Class

    Public Class compound

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    ''' <summary>
    ''' &lt;reaction>元素表示通路中的一种生化反应，用于连接代谢物节点，形成代谢网络中的化学转化关系。在代谢通路中，&lt;reaction>元素主要涉及底物（substrates）
    ''' 和产物（products）两类代谢物节点。它描述了一个化学反应如何将一组底物转化为另一组产物，以及该反应是否可逆等信息。&lt;reaction>元素在KGML中通常用于
    ''' 表示化学网络（chemical network），即以代谢物为节点、反应为边的关系网络。
    ''' </summary>
    Public Class reaction : Inherits Link

        ''' <summary>
        ''' 该反应的唯一标识符，在通路范围内唯一。通常是一个整数，用于在KGML中引用该反应。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        ''' <summary>
        ''' 该反应的名称，通常以KEGG REACTION数据库的ID表示，例如name="rn:R00710"。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 一个或多个&lt;substrate>子元素，表示该反应的底物。每个&lt;substrate>通过id属性引用一个代谢物节点的ID（即对应&lt;entry>的id），并通过name属性提供该代谢物的KEGG ID。
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("substrate")>
        Public Property substrates As compound()
        ''' <summary>
        ''' 一个或多个&lt;product>子元素，表示该反应的产物。每个&lt;product>同样通过id引用代谢物节点ID，并通过name提供代谢物KEGG ID。
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("product")>
        Public Property products As compound()

        Public Function GetModel() As Equation
            Return New Equation With {
                .Id = id,
                .reversible = True,
                .Reactants = substrates.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray,
                .Products = products.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return GetModel.ToString
        End Function
    End Class

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

    Public Class graphics

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property fgcolor As String
        <XmlAttribute> Public Property bgcolor As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double
        <XmlAttribute> Public Property width As Double
        <XmlAttribute> Public Property height As Double

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class
End Namespace
