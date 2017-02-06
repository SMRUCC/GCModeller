Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' BHR(Bi-directional hit rate) 
    ''' 
    ''' 把要注释的genome作为``query``，和KEGG数据库中的``reference``进行blast比对，输出的结果（E>10）称为 homolog。
    ''' 同时把 reference作为query，把genome作为refernce，进行blast比对。按照下面的条件对每个 homolog 进行过滤：
    ''' 
    ''' + Blast bits score > 60
    ''' + bi-directional hit rate (BHR)>0.95。
    ''' 
    ''' Blast Bits Score 是在``Blast raw score``换算过来的。
    ''' 
    ''' ``BHR``是KEGG在``Bi-directioanl Best Hit``的基础上进行修改的一个选项，``BHR = Rf * Rr``。
    ''' 
    ''' KEGG 在做注释的时候， 并不是把所有的基因都作为 refernce，而是按照是否来自同一个基因组分成一个一个的小的 
    ''' reference，分别进行 blast，假设有两个基因组 A 和B，含有的基因分别为 a1,a2,a3…an；b1,b2,b3…bn 先用A
    ''' 作为 query，B作为refer，进行blast比对，A中的基因a1对B中的基因进行遍历，和基因b1有最高的 bit score。
    ''' 现在用B作为refer,A作为query,进行blast比对，B中的基因b1对A中的基因进行遍历，如果bits score最高的是a1，
    ''' 则a1和a2就是一个BBH，但也有可能不是a1，只能成为 Single-directional hit rate。
    ''' 
    ''' 用刚才的A和B作为例子。``Rf``为用A作为query，B作为Refer,a1和B中的每一个基因都计算一次，
    ''' 
    ''' ```
    ''' R = Bits_score[a1-b1] / MaxBits_score[a1_b]
    ''' ```
    ''' 
    ''' 分子是a1和B中的一个基因的Bit_score,分母是a1和B中基因最大的bit_score。假设注释得到的a1和b1中的某个基因
    ''' 是``BBH``，则``BHR``一定等于1。当然，容许修改BHR参数``&lt;1``。计算``KO assignment score``后, 
    ''' 选择得分最高的``KO``作为这个``gene``的``KO``。
    ''' </summary>
    Public Module BHR

    End Module
End Namespace