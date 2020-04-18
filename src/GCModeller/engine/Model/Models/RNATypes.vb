Imports System.ComponentModel

Public Enum RNATypes As Byte
    mRNA = 0
    tRNA

    <Description("rRNA")>
    ribosomalRNA
    ''' <summary>
    ''' 其他类型的RNA
    ''' </summary>
    micsRNA
End Enum