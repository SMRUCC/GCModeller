#Region "Microsoft.VisualBasic::45999908c258a731cb194eb20248d297, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\KEGGObjects.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel

Namespace Assembly.KEGG.DBGET

    ''' <summary>
    ''' KEGG数据库之中的对象的类型的列表
    ''' </summary>
    Public Enum KEGGObjects As Integer

        ''' <summary>
        ''' 代谢化合物
        ''' </summary>
        <Description("cpd")> Compound = 0
        ''' <summary>
        ''' 多糖
        ''' </summary>
        <Description("gl")> Galycan = 1
        ''' <summary>
        ''' 代谢反应
        ''' </summary>
        <Description("rn")> Reaction = 2
        ''' <summary>
        ''' 生物酶
        ''' </summary>
        <Description("ec")> Enzyme = 3
        ''' <summary>
        ''' 代谢途径
        ''' </summary>
        <Description("map")> Pathway = 4
        ''' <summary>
        ''' 代谢反应模块
        ''' </summary>
        <Description("m")> [Module] = 5
        ''' <summary>
        ''' 药物
        ''' </summary>
        <Description("dr")> Drug = 6
        ''' <summary>
        ''' 人类疾病
        ''' </summary>
        <Description("ds")> HumanDisease = 7
        ''' <summary>
        ''' 人类基因组
        ''' </summary>
        <Description("hsa")> HumanGenome = 8
        ''' <summary>
        ''' 直系同源
        ''' </summary>
        <Description("ko")> Orthology = 9

    End Enum
End Namespace
