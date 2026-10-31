Namespace LinearRegression

    ''' <summary>
    ''' 先验因果符号的使用方式。
    ''' </summary>
    Public Enum PriorSignMode

        ''' <summary>
        ''' 不使用符号，只保留"候选注入 + 惩罚放松"(方案 A + B)。
        ''' </summary>
        Off = 0

        ''' <summary>
        ''' <b>符号约束</b>(signed lasso)：把先验边的系数限制在半空间内
        ''' (激活 β ≥ 0、抑制 β ≤ 0)。
        ''' 
        ''' 数据与先验冲突时系数会被压到 0 —— 即"宁可不要这条边，也不保留错误方向"。
        ''' 行为最保守，但不会产生上调。
        ''' </summary>
        Constrain = 1

        ''' <summary>
        ''' <b>保幅翻号</b>：先自由拟合(幅度来自数据)，再把先验边的系数符号翻正/翻负，
        ''' 幅度不变。
        ''' 
        ''' 与 <see cref="Constrain"/> 的区别是关键性的：共表达数据里抑制边同样是正相关，
        ''' 约束只会把它压成 0，而翻号能保留 |β| 并输出正确的因果方向
        ''' (敲降抑制因子 → 靶基因<b>上调</b>)。
        ''' </summary>
        Flip = 2

    End Enum

End Namespace
