#Region "Microsoft.VisualBasic::3fe9d15ded1227e9c14a01b0b6d4628a, localblast\PanGenome\Output\GeneCategoryType.vb"

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

    '   Total Lines: 16
    '    Code Lines: 10 (62.50%)
    ' Comment Lines: 3 (18.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (18.75%)
    '     File Size: 884 B


    ' Enum GeneCategoryType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization

''' <summary>
''' 扩展的基因家族分类定义 (基于存在度百分比)
''' </summary>
<DataContract> Public Enum GeneCategoryType As Integer

    <EnumMember(Value:=NameOf(NA))> NA = 0

    <EnumMember(Value:=NameOf(Core))> Core          ' 核心基因 (100% 存在)
    <EnumMember(Value:=NameOf(SoftCore))> SoftCore  ' 软核心基因 (95% <= 存在 < 100%)，通常用于容错
    <EnumMember(Value:=NameOf(Shell))> Shell        ' 壳基因 (15% <= 存在 < 95%)，中等频率
    <EnumMember(Value:=NameOf(Cloud))> Cloud        ' 云基因 (存在 < 15%)，低频率/特异基因
    <EnumMember(Value:=NameOf(Specific))> Specific  ' 特异基因 (仅1个基因组存在)，Cloud的子集
    <EnumMember(Value:=NameOf(Unique))> Unique      ' 独有基因 (定义同Specific，根据需求可合并或分开)
End Enum
