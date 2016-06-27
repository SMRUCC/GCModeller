Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine

''' <summary>
''' Formats options for alignment inputs and outputs.
''' --outfmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]} MSA output file format (default: fasta)
''' </summary>
Public Enum OutFmts As Integer
    ''' <summary>
    ''' 不显示申明这个命令行参数，由系统自动选择
    ''' </summary>
    <NullOrDefault> auto = -1

    ''' <summary>
    ''' fa[sta], Default
    ''' </summary>
    <Description("fa")> fasta = 0
    ''' <summary>
    ''' clu[stal]
    ''' </summary>
    <Description("clu")> clustal
    ''' <summary>
    ''' msf
    ''' </summary>
    <Description("msf")> msf
    ''' <summary>
    ''' phy[lip]
    ''' </summary>
    <Description("phy")> phylip
    ''' <summary>
    ''' selex
    ''' </summary>
    <Description("selex")> selex
    ''' <summary>
    ''' st[ockholm]
    ''' </summary>
    <Description("st")> stockholm
    ''' <summary>
    ''' vie[nna]
    ''' </summary>
    <Description("vie")> vienna
End Enum
