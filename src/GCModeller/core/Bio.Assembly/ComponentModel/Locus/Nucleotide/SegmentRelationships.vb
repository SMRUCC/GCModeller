#Region "Microsoft.VisualBasic::9a1f6944388210b72266297710343dac, core\Bio.Assembly\ComponentModel\Locus\Nucleotide\SegmentRelationships.vb"

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

    '     Enum SegmentRelationships
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum Strands
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.ComponentModel

Namespace ComponentModel.Loci

    ''' <summary>
    ''' The location relationship description enumeration for the two loci sites on the nucleotide sequence.
    ''' (核酸链上面的位点片段之间的位置关系的描述)
    ''' </summary>
    ''' <remarks>为了能够在查询的时候对输入进行叠加，在这里采取互斥</remarks>
    Public Enum SegmentRelationships As Integer
        ''' <summary>
        ''' There is nothing on this location.
        ''' </summary>
        ''' <remarks></remarks>
        Blank = -100
        ''' <summary>
        ''' The loci is on the upstream of the target loci, but part of the loci was overlapping.
        ''' (目标位点和当前的位点重叠在一个，但是目标位点的左端是在当前位点的上游的)
        ''' </summary>
        UpStreamOverlap = 2
        DownStreamOverlap = 4
        Inside = 8
        UpStream = 16
        DownStream = 32
        ''' <summary>
        ''' 比较的目标位点包括了当前的这个位置参照
        ''' </summary>
        Cover = 64

        ''' <summary>
        ''' Target loci is on the same <see cref="Strands"/> with current loci and position is also equals.
        ''' </summary>
        Equals = 128

        ''' <summary>
        ''' 指定的位点在目标位点的内部的反向序列之上
        ''' </summary>
        InnerAntiSense = 256
    End Enum

    ''' <summary>
    ''' The direction of this segment on the nucleotide sequence.
    ''' (片段在DNA链上面的方向或者是否为互补链, KEGG网站上面的``cutsequence``
    ''' 函数之中的``VECTOR``参数也和本枚举值所表述的含义一致，
    ''' 正义链为1，互补链为-1值)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Strands As Integer

        ''' <summary>
        ''' The loci site was on the DNA sequence.(这个片段在DNA链的正义链之上)
        ''' </summary>
        ''' <remarks></remarks>
        <Description("+")> Forward = 1
        ''' <summary>
        ''' The loci site was on the DNA complement strand.(这个片段在DNA链的互补链之上) 
        ''' </summary>
        ''' <remarks></remarks>
        <Description("-")> Reverse = -1
        ''' <summary>
        ''' I really don't know what the direction of the loci site it is.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("?")> Unknown = 0
    End Enum
End Namespace
