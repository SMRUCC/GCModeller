#Region "Microsoft.VisualBasic::e14014635f5dc89f5f8b842a8e7995fb, localblast\PanGenome\Output\StructuralVariation.vb"

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

    '   Total Lines: 46
    '    Code Lines: 14 (30.43%)
    ' Comment Lines: 26 (56.52%)
    '    - Xml Docs: 96.15%
    ' 
    '   Blank Lines: 6 (13.04%)
    '     File Size: 1.39 KB


    ' Enum SVType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class StructuralVariation
    ' 
    '     Properties: Breakpoint_Chromosome, Breakpoint_Position
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization

''' <summary>
''' 结构变异类型枚举
''' </summary>
''' 
<DataContract> Public Enum SVType As Integer

    <EnumMember(Value:=NameOf(None))>
    None = 0

    ''' <summary>
    ''' 缺失：该基因组相对于其他基因组缺少该基因家族
    ''' </summary>
    <EnumMember(Value:=NameOf(PAV_Absence))> PAV_Absence
    ''' <summary>
    ''' 获得：该基因组相对于其他基因组特有该基因家族
    ''' </summary>
    <EnumMember(Value:=NameOf(PAV_Presence))> PAV_Presence
    ''' <summary>
    ''' 拷贝数增加：相对于核心拷贝数增加
    ''' </summary>
    <EnumMember(Value:=NameOf(CNV_Gain))> CNV_Gain
    ''' <summary>
    ''' 拷贝数减少：相对于核心拷贝数减少
    ''' </summary>
    <EnumMember(Value:=NameOf(CNV_Loss))> CNV_Loss
    ''' <summary>
    ''' 共线性断裂（倒位/易位）
    ''' </summary>
    <EnumMember(Value:=NameOf(Collinearity_Break))> Collinearity_Break
End Enum

''' <summary>
''' 结构变异事件记录
''' </summary>
<DataContract> Public Class StructuralVariation : Inherits SVData

    ''' <summary>
    ''' 如果是共线性断裂，记录断点信息
    ''' </summary>
    ''' <returns></returns>
    Public Property Breakpoint_Chromosome As String
    Public Property Breakpoint_Position As Integer

End Class
