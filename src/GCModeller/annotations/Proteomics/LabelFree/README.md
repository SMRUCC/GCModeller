> https://www.cnblogs.com/yanzhi123/p/11712926.html

## 1. Maxquant的iBAQ和LFQ，该用哪个？

我们使用Maxquant做Label Free蛋白质组学定量分析的时候，在Maxquant的参数设置时，会遇到两个参数，LFQ和iBAQ，那么，选择哪个好呢？
如果你都选上，在最终的proteingroups.txt中，会出现三列：Intensity、IBAQ、LFQ intensity，这三列中的数字，也就是蛋白的定量强度，并不一样，那么，到底那一列比较准呢？
首先，让我们来看一下三者的计算原理是什么？

> + ``Intensity``是将某Protein Groups里面的所有Unique和Razor peptides的信号强度加起来，作为一个原始强度值。
> + ``iBAQ``是在上面的基础上，将原始强度值除以本蛋白的理论肽段数目。
> + ``LFQ``则是将原始强度值在样本之间进行校正，以消除处理、上样、预分、仪器等造成的样本间误差。

假设有两个蛋白，A和B，A和B在样本中的量是相等的，也就是等量。 假设A的长度是10个肽段，B的是100个肽段，假设鉴定结果中，覆盖度都是30%，那么蛋白A的强度是3，B的是30,。这时候我们对比一下，B是A的10倍，但是，A和B原本是相等，这样就存在较为严重的误差。
这时候，如果我们将其原始强度值除以理论肽段数目，A的强度变成了3/10, B的强度变成了3/10。 A = B，Perfect！
上面就是IBAQ的原理和用处。
但是在定量蛋白质组学中，我们并不做蛋白A和 B之间的定量，假如你有一个药物处理前的细胞和药物处理后的细胞的对照型样本做的定量蛋白质组学实验，我们关注的蛋白A在处理前和处理后的变化，至于A和B之间的比值，并不重要。
所以，如果是样本内对比，当然用iBAQ，因为其表征的是蛋白的摩尔比值（copy number）。如果是样本间对比，当然是LFQ（正式名称为MaxLFQ，也就是搜库结果中的txt文件中的LFQ Intensity）[1]
当然，如果你执意要用iBAQ，你可以手工校准样本件误差，方法很简单：蛋白IBAQ值除以此样品所有蛋白的强度的和，计算比例（这也是组学中“等质量上样”和“等体积上样”的核心区别，等质量上样来看的是比例，但是计算比例是有压缩效应的）[2]。
最后，总结一下：
+ 同一个（或者说同一针）样品内部的蛋白互相比较，用IBAQ；
+ 不同样品间互相比较（不管是重复还是不同的处理组），用LFQ。

##### Reference：
+ [1] Cox J, Hein M Y,Luber C A, et al. Accurate Proteome-wide Label-free Quantification by DelayedNormalization and Maximal Peptide Ratio Extraction, Termed MaxLFQ[J]. Molecular& Cellular Proteomics Mcp, 2014, 13(9):2513.
+ [2] Shin J B, Krey JF, Hassan A, et al. Molecular architecture of the chick vestibular hairbundle[J]. Nature Neuroscience, 2013, 16(3):365-74.

## 2. 关于数据标准化方法的描述【thermo 配带的PD2.2为例】

#### 1). 从原始的abundance到abundance（normalize），是利用样品总面积进行normalize的【total sum intensity normalization】

+ ref1：Sialana F J, Wang A L, Fazari B, et al. Quantitative proteomics of synaptosomal fractions in a rat overexpressing human DISC1 gene indicates profound synaptic dysregulation in the dorsal striatum[J]. Frontiers in molecular neuroscience, 2018, 11: 26.
+ ref2：Dittenhafer-Reed K E, Richards A L, Fan J, et al. SIRT3 mediates multi-tissue coupling for metabolic fuel switching[J]. Cell metabolism, 2015, 21(4): 637-646.

abundance到abundance（normalize），是利用样品总面积进行normalize，计算如下：

+ a. 计算3个样本Sample1，Sample2，Sample3中蛋白总量（sum行），
+ b. 选取其中一个样本（这里选取Sample3）的总量当作参考，进行其他两个样本系数（Sample1总量/Sample3总量，Sample2总量/Sample3总量）的计算;
+ c. 每个蛋白丰度值除以相应样本的系数，获得normalize数值；最终，达到个样本的总量相一致；

|protein Sample1 Sample2 Sample3 Sample1.norm Sample2.norm Sample3.norm
|P1 96263572.85 104019086.7 154492068.8 188852720.2 195452761.3 154492068.8
|P2 49830964.66 46392160.22 67074679.03 97759858.15 87171269.3 67074679.03
|P3 143632391.8 137680969.2 194423852.5 281782268.3 258703728.9 194423852.5
|P4 46985091.01 50239488.8 28002701.31 92176739.18 94400432.89 28002701.31
|P5 62493244.91 78469297.48 339179377.8 122601093.5 147444486.9 339179377.8
|sum 399205265.2 416801002.4 783172679.3 783172679.3 783172679.3 783172679.3
|系数 0.509728283 0.532195534 1 1 1 1

#### 2).abundance(group)或scaled是在abundance（normalize）基础上均一化之后的结果，主要是为了方便提取数据，把数据映射到一定范围之内，使数据大小更直观

计算如下：

+ ``a``. 蛋白a在三个样品中abundance（normalize）的结果为分别为Sample1.norm,Sample2.norm,Sample3.norm，平均值average=(Sample1.norm+Sample2.norm+Sample3.norm)/3；
+ ``b``. 所以蛋白a在三个样品中abundance(group或scale)（即均一化）分别为：Sample1.norm/average,Sample2.norm/average,Sample3.norm/average;
+ ``c``. 为方便数据分析，将结果扩大100倍，蛋白a的三个样品中abundance(group或scale)结果为100Sample1.norm/average,100Sample2.norm/average,100Sample3.norm/average;

#### 3). 关于组内样本蛋白总量的波动性评估

看了一篇文章，文章公布了 label-free quantification【LFQ】的数据。在一组重复数据中，有变化的倍数能达到2倍多。如附件1-s2.0-S2211124717311889-mmc3 - 副本.xlsx。
ref：Itzhak D N, Davies C, Tyanova S, et al. A mass spectrometry-based approach for mapping protein subcellular localization reveals the spatial proteome of mouse primary neurons[J]. Cell reports, 2017, 20(11): 2706-2718.【A Mass Spectrometry-Based Approach for Mapping.pdf】