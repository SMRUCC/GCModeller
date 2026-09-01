#Region "Microsoft.VisualBasic::12aefb41c7e09f1842f306896d95ed3f, sub-system\BNLearn\DBN\RegulatoryLink.vb"

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

    '   Total Lines: 47
    '    Code Lines: 13 (27.66%)
    ' Comment Lines: 30 (63.83%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (8.51%)
    '     File Size: 1.25 KB


    ' Class RegulatoryLink
    ' 
    '     Properties: effector, regulate_genes, target_operon, TF_family, TF_id
    '                 TFBS_id
    ' 
    ' Enum Effector
    ' 
    '     Activator, Inhibitor, Unknown
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 本类有一个名为 effector 的属性，VB 名称解析不区分大小写，
' 会遮蔽同名的 Effector 枚举，这里用别名显式引用该枚举类型。
Imports EffectorRole = SMRUCC.genomics.Analysis.BNLearn.Effector

''' <summary>
''' Gene regulatory network
''' </summary>
Public Class RegulatoryLink

    ''' <summary>
    ''' transcript factor protein/rna id
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_id As String
    ''' <summary>
    ''' family of the TF
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_family As String
    ''' <summary>
    ''' motif id of the TFBS site
    ''' </summary>
    ''' <returns></returns>
    Public Property TFBS_id As String

    ''' <summary>
    ''' 该条调控边的调控方向（激活 / 抑制）。
    ''' 
    ''' 默认值为 <see cref="Effector.Activator"/>，以保持既有构造点的行为不变。
    ''' 由先验网络构建拓扑时，必须把先验边的 RegulationType 传入：若缺失该信息，
    ''' 网络中将不存在任何抑制性调控，激活得分恒为正，CPT 的 Low 分支将不可达。
    ''' </summary>
    ''' <returns></returns>
    Public Property RegulationType As EffectorRole = EffectorRole.Activator

    ''' <summary>
    ''' 该条调控边的置信度（0-1）。用于同一 TF 对同一靶点存在多条方向冲突的边时做确定性仲裁。
    ''' </summary>
    ''' <returns></returns>
    Public Property Confidence As Double = 1.0

    ''' <summary>
    ''' effector metabolite of this TF its regulation function
    ''' </summary>
    ''' <returns></returns>
    Public Property effector As Dictionary(Of String, Effector)
    ''' <summary>
    ''' target operon id that this TF regulates
    ''' </summary>
    ''' <returns></returns>
    Public Property target_operon As String
    ''' <summary>
    ''' the operon member genes, TF regulates this operon member genes theirs transcription.
    ''' </summary>
    ''' <returns></returns>
    Public Property regulate_genes As String()

End Class

''' <summary>
''' effects of the effector to the TF protein
''' </summary>
Public Enum Effector
    Unknown
    Activator
    Inhibitor
End Enum

