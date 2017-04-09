Imports System.ComponentModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' Deoxyribonucleotides NT base which consist of the DNA sequence.
    ''' (枚举所有的脱氧核糖核苷酸)
    ''' </summary>
    ''' <remarks></remarks>
    <Description("Deoxyribonucleotides")> Public Enum DNA As SByte

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