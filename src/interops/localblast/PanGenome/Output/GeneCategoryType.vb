Imports System.Runtime.Serialization

''' <summary>
''' 扩展的基因家族分类定义 (基于存在度百分比)
''' </summary>
<DataContract> Public Enum GeneCategoryType As Integer

    <EnumMember(Value:=NameOf(NA))> NA = 0

    <EnumMember(Value:=NameOf(Core))> Core          ' 核心基因 (100% 存在)
    <EnumMember(Value:=NameOf(SoftCore))> SoftCore  ' 软核心基因 (95% <= 存在 < 100%)，通常用于容错
    <EnumMember(Value:=NameOf(Shell))> Shell        ' 壳基因 (15% <= 存在 < 95%)，中等频率
    <EnumMember(Value:=NameOf(Cloud))> Cloud        ' 云基因 (存在 < 15%)，低频率/特异基因
    <EnumMember(Value:=NameOf(Specific))> Specific  ' 特异基因 (仅1个基因组存在)，Cloud的子集
    <EnumMember(Value:=NameOf(Unique))> Unique      ' 独有基因 (定义同Specific，根据需求可合并或分开)
End Enum