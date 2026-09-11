Imports System.ComponentModel

Namespace Models

    ''' <summary>
    ''' TSS 相对于基因组注释（PTT/GFF）的上下文分类，
    ''' 对应 TSSAR 论文后处理阶段的 Primary / Internal / Antisense(Ai/Ad) / Orphan 分类。
    ''' </summary>
    Public Enum TssTypes As Integer

        ''' <summary>
        ''' 孤儿 TSS：附近没有任何基因注释与之关联。
        ''' </summary>
        <Description("Orphan")>
        Orphan = 0

        ''' <summary>
        ''' 初级（Primary）TSS：位于某个基因起始密码子上游的启动子区（默认 250 nt 内），
        ''' 通常对应基因的 5'UTR 起点。
        ''' </summary>
        <Description("Primary")>
        Primary = 1

        ''' <summary>
        ''' 内部（Internal）TSS：位于某个基因的开放阅读框（ORF）内部，可能是内部启动子。
        ''' </summary>
        <Description("Internal")>
        Internal = 2

        ''' <summary>
        ''' 反义内部（Antisense internal, Ai）：位于反义链基因的 ORF 内部。
        ''' </summary>
        <Description("Ai")>
        AntisenseInternal = 3

        ''' <summary>
        ''' 反义下游（Antisense downstream, Ad）：位于反义链基因下游（默认 30 nt 内）。
        ''' </summary>
        <Description("Ad")>
        AntisenseDownstream = 4
    End Enum
End Namespace
