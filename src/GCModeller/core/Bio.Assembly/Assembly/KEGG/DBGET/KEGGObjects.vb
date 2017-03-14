Imports System.ComponentModel

Namespace Assembly.KEGG.DBGET

    ''' <summary>
    ''' KEGG数据库之中的对象的类型的列表
    ''' </summary>
    Public Enum KEGGObjects As Integer

        ''' <summary>
        ''' 代谢化合物
        ''' </summary>
        <Description("cpd")> Compound = 0
        ''' <summary>
        ''' 多糖
        ''' </summary>
        <Description("gl")> Galycan = 1
        ''' <summary>
        ''' 代谢反应
        ''' </summary>
        <Description("rn")> Reaction = 2
        ''' <summary>
        ''' 生物酶
        ''' </summary>
        <Description("ec")> Enzyme = 3
        ''' <summary>
        ''' 代谢途径
        ''' </summary>
        <Description("map")> Pathway = 4
        ''' <summary>
        ''' 代谢反应模块
        ''' </summary>
        <Description("m")> [Module] = 5
        ''' <summary>
        ''' 药物
        ''' </summary>
        <Description("dr")> Drug = 6
        ''' <summary>
        ''' 人类疾病
        ''' </summary>
        <Description("ds")> HumanDisease = 7
        ''' <summary>
        ''' 人类基因组
        ''' </summary>
        <Description("hsa")> HumanGenome = 8
        ''' <summary>
        ''' 直系同源
        ''' </summary>
        <Description("ko")> Orthology = 9

    End Enum
End Namespace