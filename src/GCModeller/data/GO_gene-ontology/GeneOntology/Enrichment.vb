#Region "Microsoft.VisualBasic::178caf144a509f8c9898bf25e46abeb4, GCModeller\data\GO_gene-ontology\GeneOntology\Enrichment.vb"

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


    ' Code Statistics:

    '   Total Lines: 26
    '    Code Lines: 2
    ' Comment Lines: 23
    '   Blank Lines: 1
    '     File Size: 3.06 KB


    ' Module Enrichment
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' Gene Ontology可分为分子功能(``Molecular Function``)，生物过程(``biological process``)和细胞组成(``cellular component``)三个部分.
''' 蛋白质或者基因可以通过ID对应或者序列注释的方法找到与之对应的GO号,而GO号可对于到Term,即功能类别或者细胞定位.
''' 
''' + 功能富集分析: 功能富集需要有一个参考数据集, 通过该项分析可以找出在统计上显著富集的GO Term.该功能或者定位有可能与研究的目前有关.
''' + GO功能分类是在某一功能层次上统计蛋白或者基因的数目或组成, 往往是在GO的第二层次.此外也有研究都挑选一些Term, 而后统计直接对应到该Term的基因或蛋白数.结果一般以柱状图或者饼图表示.
''' 
''' ###### 1.GO分析
''' 根据挑选出的差异基因, 计算这些差异基因同GO 分类中某（几）个特定的分支的超几何分布关系,GO 分析会对每个有差异基因存在的GO 返回一个``p-value``,小的p 值表示差异基因在该GO 中出现了富集. 
''' GO 分析对实验结果有提示的作用, 通过差异基因的GO 分析,可以找到富集差异基因的GO分类条目,寻找不同样品的差异基因可能和哪些基因功能的改变有关.
''' 
''' ###### 2.Pathway分析
''' 根据挑选出的差异基因, 计算这些差异基因同Pathway 的超几何分布关系, Pathway 分析会对每个有差异基因存在的pathway 返回一个p-value, 小的p 值表示差异基因在该pathway 中出现了富集. 
''' Pathway 分析对实验结果有提示的作用, 通过差异基因的Pathway 分析,可以找到富集差异基因的Pathway 条目,寻找不同样品的差异基因可能和哪些细胞通路的改变有关.与``GO``分析不同, ``pathway``分析
''' 的结果更显得间接,这是因为,pathway 是蛋白质之间的相互作用,pathway 的变化可以由参与这条pathway 途径的蛋白的表达量或者蛋白的活性改变而引起.而通过芯片结果得到的是编码这些蛋白质的mRNA 
''' 表达量的变化.从mRNA 到蛋白表达还要经过microRNA 调控,翻译调控,翻译后修饰（如糖基化,磷酸化）,蛋白运输等一系列的调控过程,mRNA 表达量和蛋白表达量之间往往不具有线性关系,因此mRNA 
''' 的改变不一定意味着蛋白表达量的改变.同时也应注意到,在某些pathway 中,如EGF/EGFR 通路,细胞可以在维持蛋白量不变的情况下,通过蛋白磷酸化程度的改变（调节蛋白的活性）来调节这条通路.
''' 所以芯片数据``pathway``分析的结果需要有后期蛋白质功能实验的支持, 如``Western blot/ELISA``, ``IHC``（免疫组化）, ``over expression``（过表达）, ``RNAi``（RNA 干扰）, 
''' ``knockout``（基因敲除）, ``trans gene``（转基因）等.
''' 
''' ###### 3.基因网络分析
''' 目的：根据文献, 数据库和已知的pathway 寻找基因编码的蛋白之间的相互关系(不超过1000 个基因).
''' </summary>
Public Module Enrichment

End Module
