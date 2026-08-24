#Region "Microsoft.VisualBasic::1c2879d6dc0ea1d7bc844c0a68eb2cfb, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Elements\relation.vb"

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

    '   Total Lines: 34
    '    Code Lines: 8 (23.53%)
    ' Comment Lines: 20 (58.82%)
    '    - Xml Docs: 95.00%
    ' 
    '   Blank Lines: 6 (17.65%)
    '     File Size: 1.94 KB


    '     Class relation
    ' 
    '         Properties: entry1, entry2, subtype
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' &lt;relation>元素表示通路中两个节点之间的关联关系，主要用于描述蛋白质（或基因产物）之间的相互作用，以及蛋白质与代谢物之间的调控关系。与&lt;reaction>描述化学转化不同，
    ''' &lt;relation>更侧重于调控和相互作用，例如一个蛋白质激活或抑制另一个蛋白质，或一个蛋白质结合一个代谢物等。&lt;relation>元素在KGML中通常用于表示蛋白质网络（protein network）
    ''' 或调控网络，即以蛋白质/基因为节点、调控关系为边的网络。
    ''' </summary>
    Public Class relation : Inherits link

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

End Namespace
