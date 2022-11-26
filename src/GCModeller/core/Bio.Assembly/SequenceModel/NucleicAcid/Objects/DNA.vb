#Region "Microsoft.VisualBasic::1c2560de2182c9308e0e54274b6f8498, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\DNA.vb"

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

    '   Total Lines: 82
    '    Code Lines: 23
    ' Comment Lines: 54
    '   Blank Lines: 5
    '     File Size: 2.25 KB


    '     Enum DNA
    ' 
    '         B, D, H, K, M
    '         N, S, V, W, Y
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' Deoxyribonucleotides NT base which consist of the DNA sequence.
    ''' (枚举所有的脱氧核糖核苷酸)
    ''' </summary>
    ''' <remarks></remarks>
    <Description("Deoxyribonucleotides")> Public Enum DNA As Byte

        ''' <summary>
        ''' Gaps/Rare bases(空格或者其他的稀有碱基)
        ''' </summary>
        ''' <remarks></remarks>
        <Description("-")> NA = 0
        ''' <summary>
        ''' Adenine, paired with <see cref="DNA.dTMP"/>(A, 腺嘌呤)
        ''' </summary>
        <Description("A")> dAMP = 1
        ''' <summary>
        ''' Guanine, paired with <see cref="DNA.dCMP"/>(G, 鸟嘌呤)
        ''' </summary>
        <Description("G")> dGMP = 2
        ''' <summary>
        ''' Cytosine, paired with <see cref="DNA.dGMP"/>(C, 胞嘧啶)
        ''' </summary>
        <Description("C")> dCMP = 3
        ''' <summary>
        ''' Thymine, paired with <see cref="DNA.dAMP"/>(T, 胸腺嘧啶)
        ''' </summary>
        <Description("T")> dTMP = 4

#Region "下面是简并碱基"

        ''' <summary>
        ''' degenerate bases: A/G
        ''' </summary>
        R = 10
        ''' <summary>
        ''' degenerate bases: C/T
        ''' </summary>
        Y
        ''' <summary>
        ''' degenerate bases: A/C
        ''' </summary>
        M
        ''' <summary>
        ''' degenerate bases: G/T
        ''' </summary>
        K
        ''' <summary>
        ''' degenerate bases: G/C
        ''' </summary>
        S
        ''' <summary>
        ''' degenerate bases: A/T
        ''' </summary>
        W
        ''' <summary>
        ''' degenerate bases: A/T/C
        ''' </summary>
        H
        ''' <summary>
        ''' degenerate bases: G/T/C
        ''' </summary>
        B
        ''' <summary>
        ''' degenerate bases: G/A/C
        ''' </summary>
        V
        ''' <summary>
        ''' degenerate bases: G/A/T
        ''' </summary>
        D
        ''' <summary>
        ''' degenerate bases: A/T/C/G
        ''' </summary>
        N
#End Region
    End Enum
End Namespace
