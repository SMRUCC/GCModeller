#Region "Microsoft.VisualBasic::af369249722a6d669c5157e5e80f51ea, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\KGML.vb"

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

    '   Total Lines: 35
    '    Code Lines: 24 (68.57%)
    ' Comment Lines: 5 (14.29%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 6 (17.14%)
    '     File Size: 1.34 KB


    '     Class pathway
    ' 
    '         Properties: image, items, link, name, number
    '                     org, reactions, relations, title
    ' 
    '         Function: ResourceURL, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' The kegg pathway map layout data in KGML file format.
    ''' 
    ''' KEGG通过通路图（Pathway Map）来直观展示生物分子及其相互作用网络。为了便于计算机处理和通路可视化，KEGG引入了KGML（KEGG Markup Language）作为通路信息的交换格式。
    ''' KGML是一种基于XML的描述语言，用于表示KEGG通路图中的图形对象及其相互关系。它将通路中手动绘制的复杂图形信息转化为计算机可读的结构化数据，
    ''' 从而支持通路的自动绘制和计算分析。KGML文件通常对应KEGG中的某条通路，例如糖酵解通路（map00010）。一个KGML文件包含了该通路中所有节点（基因、酶、代谢物等）以及
    ''' 它们之间相互作用的详细信息。通过解析KGML文件，我们可以提取出基因、酶、反应和代谢物等关键元素，并构建出相应的代谢网络模型。这为后续的可视化分析和计算建模提供了基础。
    ''' </summary>
    Public Class pathway

        ''' <summary>
        ''' 通路的KEGG标识符，格式通常为path:&lt;organism>&lt;number>，例如path:hsa00010表示人类糖酵解通路。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' 通路所属的生物体分类，如ko表示参考通路（以KO编号表示），ec表示以酶EC编号表示的参考通路，或具体的三字母生物体代码（如hsa表示人类）。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property org As String
        ''' <summary>
        ''' 通路的数字编号，如00010，用于唯一标识该通路。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property number As String
        ''' <summary>
        ''' 通路的标题或名称，提供对通路内容的描述。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property title As String
        ''' <summary>
        ''' 该通路的官方图像资源的URL地址，指向KEGG服务器上存储的通路图图片。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property image As String
        ''' <summary>
        ''' 该通路在KEGG网站上的详细页面链接，提供进一步的信息。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property link As String

#Region "pathway network"
        <XmlElement(NameOf(entry))> Public Property entries As entry()
        <XmlElement(NameOf(relation))> Public Property relations As relation()
        <XmlElement(NameOf(reaction))> Public Property reactions As reaction()
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ResourceURL(entry As String) As String
            Return $"http://www.kegg.jp/kegg-bin/download?entry={entry}&format=kgml"
        End Function

        Public Overrides Function ToString() As String
            Return $"[{name}] {title}"
        End Function
    End Class
End Namespace
