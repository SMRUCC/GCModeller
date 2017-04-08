Imports System.ComponentModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' Deoxyribonucleotides NT base which consist of the DNA sequence.(枚举所有的脱氧核糖核苷酸)
    ''' </summary>
    ''' <remarks></remarks>
    <Description("Deoxyribonucleotides")> Public Enum DNA As SByte
        ''' <summary>
        ''' Gaps/Rare bases(空格或者其他的稀有碱基)
        ''' </summary>
        ''' <remarks></remarks>
        NA = -1
        ''' <summary>
        ''' Adenine, paired with <see cref="DNA.dTMP"/>(A, 腺嘌呤)
        ''' </summary>
        dAMP = 0
        ''' <summary>
        ''' Guanine, paired with <see cref="DNA.dCMP"/>(G, 鸟嘌呤)
        ''' </summary>
        dGMP = 1
        ''' <summary>
        ''' Cytosine, paired with <see cref="DNA.dGMP"/>(C, 胞嘧啶)
        ''' </summary>
        dCMP = 2
        ''' <summary>
        ''' Thymine, paired with <see cref="DNA.dAMP"/>(T, 胸腺嘧啶)
        ''' </summary>
        dTMP = 3
    End Enum
End Namespace