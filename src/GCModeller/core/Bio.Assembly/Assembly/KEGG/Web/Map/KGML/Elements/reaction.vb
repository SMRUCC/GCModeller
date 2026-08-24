#Region "Microsoft.VisualBasic::1a8cf3d123c3e1c0a0e3d3506bfde827, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Elements\reaction.vb"

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

    '   Total Lines: 53
    '    Code Lines: 25 (47.17%)
    ' Comment Lines: 21 (39.62%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (13.21%)
    '     File Size: 2.66 KB


    '     Class reaction
    ' 
    '         Properties: id, name, products, substrates
    ' 
    '         Function: GetModel, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' &lt;reaction>元素表示通路中的一种生化反应，用于连接代谢物节点，形成代谢网络中的化学转化关系。在代谢通路中，&lt;reaction>元素主要涉及底物（substrates）
    ''' 和产物（products）两类代谢物节点。它描述了一个化学反应如何将一组底物转化为另一组产物，以及该反应是否可逆等信息。&lt;reaction>元素在KGML中通常用于
    ''' 表示化学网络（chemical network），即以代谢物为节点、反应为边的关系网络。
    ''' </summary>
    Public Class reaction : Inherits link

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
                .id = id,
                .reversible = True,
                .Reactants = substrates.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray,
                .products = products.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return GetModel.ToString
        End Function
    End Class

End Namespace
