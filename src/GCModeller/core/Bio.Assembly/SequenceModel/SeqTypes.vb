
Imports System.ComponentModel

Namespace SequenceModel

    Public Enum SeqTypes As Integer
        ''' <summary>
        ''' the unknow sequence type
        ''' </summary>
        Generic = 0
        ''' <summary>
        ''' Deoxyribonucleotide - DNA(ATGC)
        ''' </summary>
        DNA
        ''' <summary>
        ''' Ribonucleotide - RNA(AUGC)
        ''' </summary>
        RNA
        ''' <summary>
        ''' Polypeptide
        ''' </summary>
        <Description("prot")> Protein
    End Enum
End Namespace