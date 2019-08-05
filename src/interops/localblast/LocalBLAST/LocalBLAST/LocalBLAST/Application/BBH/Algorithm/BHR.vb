#Region "Microsoft.VisualBasic::4bbae16b799edd251e51408c1012d0ca, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Algorithm\BHR.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module BHR
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' **BHR(Bi-directional hit rate)** 
    ''' 
    ''' http://www.genome.jp/tools/kaas/help.html
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
    ''' + Rf和Rr分别为forward sbh以及reverse sbh的``R``值
    ''' 
    ''' Given a genome to be annotated, it is compared against each genome in the reference set of the KEGG GENES database 
    ''' by the homology searches in both forward and reverse directions, taking each gene in genome A as a query compared 
    ''' against all genes in genome B, and vice versa. Those hits with bit scores less than 60 are removed. Because the bit 
    ''' scores of a gene pair a and b from two genomes A and B, respectively, can be different in forward and reverse 
    ''' directions, and because the top scores do not necessarily reflect the order of the rigorous Smith-Waterman scores
    ''' 
    ''' Here, ``R = S'/Sb`` where S' is the bit score of a against b, and Sb is the score of a against the best-hit gene in 
    ''' genome B (which may not necessarily be b). Rf refers to the score from the forward hit (A against B), and Rr refers 
    ''' to the score from the reverse hit (B against A). We select those genes whose BHR is greater than 0.95 in BBH method, 
    ''' and Rf is greater than 0.95 in SBH method.
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
    ''' <remarks>
    ''' + Moriya, Y., Itoh, M., Okuda, S., Yoshizawa, A., and Kanehisa, M.; KAAS: an automatic genome annotation and pathway reconstruction server. Nucleic Acids Res. 35, W182-W185 (2007). 
    ''' </remarks>
    Public Module BHR



    End Module
End Namespace
