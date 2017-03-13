Imports System.ComponentModel

Namespace Assembly.KEGG.DBGET

    Public Enum KEGGObjects As Integer
        <Description("cpd")> Compound
        <Description("gl")> Galycan
        <Description("rn")> Reaction
        <Description("ec")> Enzyme
        <Description("map")> Pathway
        <Description("m")> [Module]
        <Description("dr")> Drug
        <Description("ds")> HumanDisease
        <Description("hsa")> HumanGenome
        <Description("ko")> Orthology
    End Enum
End Namespace