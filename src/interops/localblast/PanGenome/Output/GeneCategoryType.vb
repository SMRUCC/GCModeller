''' <summary>
''' 扩展的基因家族分类定义 (基于存在度百分比)
''' </summary>
Public Enum GeneCategoryType
    Core            ' 核心基因 (100% 存在)
    SoftCore        ' 软核心基因 (95% <= 存在 < 100%)，通常用于容错
    Shell           ' 壳基因 (15% <= 存在 < 95%)，中等频率
    Cloud           ' 云基因 (存在 < 15%)，低频率/特异基因
    Specific        ' 特异基因 (仅1个基因组存在)，Cloud的子集
    Unique          ' 独有基因 (定义同Specific，根据需求可合并或分开)
End Enum