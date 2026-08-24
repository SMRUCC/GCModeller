#Region "Microsoft.VisualBasic::aa21c729d4d78464b9540e27f27d1570, analysis\OperonMapper\OperonPrediction\GeneInfo.vb"

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

    '   Total Lines: 68
    '    Code Lines: 20 (29.41%)
    ' Comment Lines: 43 (63.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (7.35%)
    '     File Size: 2.31 KB


    '     Structure GeneInfo
    ' 
    '         Properties: [End], EC_numbers, GeneID, GO_Terms, KO_Terms
    '                     Length, PhylogeneticProfile, Start, Strand
    ' 
    '     Enum IntergenicDistanceGroup
    ' 
    '         NA
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    ''' <summary>
    ''' 基因信息结构，存储单个基因的坐标、方向及功能注释等信息
    ''' </summary>
    Public Structure GeneInfo
        ''' <summary>
        ''' 基因的唯一标识符
        ''' </summary>
        Public Property GeneID As String
        ''' <summary>
        ''' 基因在基因组上的起始位置
        ''' </summary>
        Public Property Start As Integer
        ''' <summary>
        ''' 基因在基因组上的终止位置
        ''' </summary>
        Public Property [End] As Integer
        ''' <summary>
        ''' 基因的长度 (bp)
        ''' </summary>
        Public Property Length As Integer
        ''' <summary>
        ''' 基因所在的链方向 ('+' 或 '-')
        ''' </summary>
        Public Property Strand As Strands
        ''' <summary>
        ''' 基因关联的 Gene Ontology 术语列表
        ''' </summary>
        Public Property GO_Terms As String()
        Public Property KO_Terms As String()
        ''' <summary>
        ''' 基因关联的EC编号列表，用于KEGG代谢网络分析
        ''' </summary>
        Public Property EC_numbers As String()
        ''' <summary>
        ''' 基因的系统发育谱，键为参考基因组ID，值为该基因是否在该基因组中存在
        ''' </summary>
        ''' <remarks>
        ''' 基因组ID -> 存在状态
        ''' </remarks>
        Public Property PhylogeneticProfile As Dictionary(Of String, Boolean)

    End Structure

    ''' <summary>
    ''' 基因间距离分组枚举，用于分类器的分组训练策略
    ''' 论文 Results 部分指出，根据基因间距离将数据集分为三个子组能有效降低分类误差
    ''' </summary>
    Public Enum IntergenicDistanceGroup
        NA

        ''' <summary>
        ''' 基因间距离小于 40 nt (U40)
        ''' </summary>
        U40   ' < 40 nt
        ''' <summary>
        ''' 基因间距离在 40 nt 到 200 nt 之间 (U200)
        ''' </summary>
        U200  ' 40 - 200 nt
        ''' <summary>
        ''' 基因间距离大于 200 nt (O200)
        ''' </summary>
        O200  ' > 200 nt
    End Enum
End Namespace
