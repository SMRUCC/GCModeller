# 如何利用NCBI Taxonomy 数据库文件，根据物种名找到物种的分类地位

## Taxonom 数据库文件下载

> ftp://ftp.ncbi.nlm.nih.gov/pub/taxonomy/

下载``names.dmp``和``nodes.dmp``两个文件。

## names.dmp 文件的数据格式

文件的第一列是``tax_id``，第2列是拉丁名。下例中，第2列的拉丁名是大肠杆菌K-12菌株MG1655的拉丁名。
```
511145	|	Escherichia coli str. K-12 substr. MG1655	|		|	scientific name	|
```

## nodes.dmp 文件的数据格式

示例：
```
511145	|	83333	|	no rank	|		|	0	|	1	|	11	|	1	|	0	|	1	|	1	|	0	|		|
```
第1列是``tax_id``，第2列是第1列上一级分类级别（上一级节点）的``tax_id``。上面示例中，``tax_id`` 511145的上一级分类级别的``tax_id``是83333。

从names.dmp文件中可查到：
```
83333	|	Escherichia coli K-12	|		|	scientific name	|
```
其中，83333对应的名称是：Escherichia coli K-12。这是菌株Escherichia coli str. K-12 substr. MG1655的物种名。

## 在 nodes.dmp 文件中反复追溯上一级节点直到根节点

示例如下：
```
83333	|	562	|	no rank	|		|	0	|	1	|	11	|	1	|	0	|	1	|	1	|	0	|		|
```
在nodes.dmp文件找到562的上一级节点：
```
562	|	561	|	species	|	EC	|	0	|	1	|	11	|	1	|	0	|	1	|	1	|	0	|		|
```
561的上一级节点：
```
561	|	543	|	genus	|		|	0	|	1	|	11	|	1	|	0	|	1	|	0	|	0	|		|
```
543的上一级节点：
```
543	|	91347	|	family	|		|	0	|	1	|	11	|	1	|	0	|	1	|	0	|	0	|		|
```
91347的上一级节点：
```
91347	|	1236	|	order	|		|	0	|	1	|	11	|	1	|	0	|	1	|	0	|	0	|		|
```
1236的上一级节点：
```
1236	|	1224	|	class	|		|	0	|	1	|	11	|	1	|	0	|	1	|	0	|	0	|		|
```
1224的上一级节点：
```
1224	|	2	|	phylum	|		|	0	|	1	|	11	|	1	|	0	|	1	|	0	|	0	|		|
```
2的上一级节点：
```
2	|	131567	|	superkingdom	|		|	0	|	0	|	11	|	0	|	0	|	0	|	0	|	0	|		|
```
131567的上一级节点：
```
131567	|	1	|	no rank	|		|	8	|	1	|	1	|	1	|	0	|	1	|	1	|	0	|		|
```
在names.dmp文件中，1的注释是：
```
1	|	root	|		|	scientific name	|
```
表明已追溯到根节点，追溯停止。

（五）回溯

从根节点回溯。
根据各级节点``tax_id``，在``names.dmp``文件中找到对应的**scientific name**，即得到**Escherichia coli str. K-12 substr. MG1655**的各级分类名称。

归纳如下：

```
1	root
131567	cellular organisms
2	Bacteria
1224	Proteobacteria
1236	Gammaproteobacteria
91347	Enterobacteriales
543	Enterobacteriaceae
561	Escherichia
562	Escherichia coli
83333	Escherichia coli K-12
511145	Escherichia coli str. K-12 substr. MG1655
```

> http://blog.163.com/bioinfor_cnu/blog/static/194462237201502542012374/
