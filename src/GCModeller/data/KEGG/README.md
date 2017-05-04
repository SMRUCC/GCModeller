# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## class_br08201_reaction
KEGG enzymic reaction catagory

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|rn|VarChar (45)|||
|name|VarChar (45)|||
|EC|VarChar (45)||level4|
|level1|VarChar (45)|||
|level2|VarChar (45)|||
|level3|VarChar (45)|||

```SQL
CREATE TABLE `class_br08201_reaction` (
  `uid` int(11) NOT NULL,
  `rn` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `EC` varchar(45) DEFAULT NULL COMMENT 'level4',
  `level1` varchar(45) DEFAULT NULL,
  `level2` varchar(45) DEFAULT NULL,
  `level3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG enzymic reaction catagory';
```



## class_ko00001_orthology
KEGG的基因同源分类

|field|type|attributes|description|
|-----|----|----------|-----------|
|Orthology|Int64 (11)|``NN``|``data_orthology``基因同源数据表之中的唯一数字编号|
|KEGG|VarChar (45)||当前的这个基因同源的KO编号|
|name|VarChar (45)||基因名|
|function|VarChar (45)||功能描述|
|level_A|VarChar (45)||代谢途径大分类|
|level_B|VarChar (45)||代谢途径小分类|
|level_C|VarChar (45)||KEGG pathway.当前的这个参考基因同源所处的代谢途径|

```SQL
CREATE TABLE `class_ko00001_orthology` (
  `Orthology` int(11) NOT NULL COMMENT '``data_orthology``基因同源数据表之中的唯一数字编号',
  `KEGG` varchar(45) DEFAULT NULL COMMENT '当前的这个基因同源的KO编号',
  `name` varchar(45) DEFAULT NULL COMMENT '基因名',
  `function` varchar(45) DEFAULT NULL COMMENT '功能描述',
  `level_A` varchar(45) DEFAULT NULL COMMENT '代谢途径大分类',
  `level_B` varchar(45) DEFAULT NULL COMMENT '代谢途径小分类',
  `level_C` varchar(45) DEFAULT NULL COMMENT 'KEGG pathway.当前的这个参考基因同源所处的代谢途径',
  PRIMARY KEY (`Orthology`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG的基因同源分类';
```



## class_ko00001_pathway


|field|type|attributes|description|
|-----|----|----------|-----------|
|pathway|Int64 (11)|``NN``||
|KEGG|VarChar (45)|||
|level_A|VarChar (45)|||
|level_B|VarChar (45)|||
|name|VarChar (45)|||

```SQL
CREATE TABLE `class_ko00001_pathway` (
  `pathway` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `level_A` varchar(45) DEFAULT NULL,
  `level_B` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## class_orthology_genes
这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|orthology|Int64 (11)|``NN``|直系同源表的数字编号|
|locus_tag|VarChar (45)|``NN``|基因号|
|geneName|VarChar (45)||基因名，因为有些基因还是没有名称的，所以在这里可以为空|
|organism|VarChar (45)|``NN``|KEGG物种简写编号|

```SQL
CREATE TABLE `class_orthology_genes` (
  `uid` int(11) NOT NULL,
  `orthology` int(11) NOT NULL COMMENT '直系同源表的数字编号',
  `locus_tag` varchar(45) NOT NULL COMMENT '基因号',
  `geneName` varchar(45) DEFAULT NULL COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
  `organism` varchar(45) NOT NULL COMMENT 'KEGG物种简写编号',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系';
```



## data_compounds


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|KEGG|VarChar (45)|``NN``|KEGG代谢物编号|
|names|VarChar (45)|||
|formula|VarChar (45)||分子式|
|mass|VarChar (45)||物质质量|
|mol_weight|VarChar (45)||分子质量|

```SQL
CREATE TABLE `data_compounds` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'KEGG代谢物编号',
  `names` varchar(45) DEFAULT NULL,
  `formula` varchar(45) DEFAULT NULL COMMENT '分子式',
  `mass` varchar(45) DEFAULT NULL COMMENT '物质质量',
  `mol_weight` varchar(45) DEFAULT NULL COMMENT '分子质量',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## data_enzyme
酶

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|EC|VarChar (45)|``NN``|EC编号|
|name|VarChar (45)||酶名称|
|sysname|VarChar (45)||生物酶的系统名称|
|Reaction(KEGG)_uid|VarChar (45)||``data_reactions``表之中的数字编号|
|Reaction(KEGG)|VarChar (45)||KEGG之中所能够被催化的生物过程的编号|
|Reaction(IUBMB)|VarChar (45)|||
|Substrate|VarChar (45)|||
|Product|VarChar (45)|||
|Comment|VarChar (45)|||

```SQL
CREATE TABLE `data_enzyme` (
  `uid` int(11) NOT NULL,
  `EC` varchar(45) NOT NULL COMMENT 'EC编号',
  `name` varchar(45) DEFAULT NULL COMMENT '酶名称',
  `sysname` varchar(45) DEFAULT NULL COMMENT '生物酶的系统名称',
  `Reaction(KEGG)_uid` varchar(45) DEFAULT NULL COMMENT '``data_reactions``表之中的数字编号',
  `Reaction(KEGG)` varchar(45) DEFAULT NULL COMMENT 'KEGG之中所能够被催化的生物过程的编号',
  `Reaction(IUBMB)` varchar(45) DEFAULT NULL,
  `Substrate` varchar(45) DEFAULT NULL,
  `Product` varchar(45) DEFAULT NULL,
  `Comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='酶';
```



## data_modules


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|KEGG|VarChar (45)|||
|name|VarChar (45)|||
|definition|VarChar (45)|||
|map|VarChar (45)||image -> gzip -> base64 string|

```SQL
CREATE TABLE `data_modules` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  `map` varchar(45) DEFAULT NULL COMMENT 'image -> gzip -> base64 string',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## data_organisms
taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|KEGG_sp|VarChar (8)|``NN``||
|scientific name|VarChar (45)|||
|domain|VarChar (45)|||
|kingdom|VarChar (45)||界|
|phylum|VarChar (45)||门|
|class|VarChar (45)||纲|
|order|VarChar (45)||目|
|family|VarChar (45)||科|
|genus|VarChar (45)||属|
|species|VarChar (45)||种|

```SQL
CREATE TABLE `data_organisms` (
  `uid` int(11) NOT NULL,
  `KEGG_sp` varchar(8) NOT NULL,
  `scientific name` varchar(45) DEFAULT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL COMMENT '界',
  `phylum` varchar(45) DEFAULT NULL COMMENT '门',
  `class` varchar(45) DEFAULT NULL COMMENT '纲',
  `order` varchar(45) DEFAULT NULL COMMENT '目',
  `family` varchar(45) DEFAULT NULL COMMENT '科',
  `genus` varchar(45) DEFAULT NULL COMMENT '属',
  `species` varchar(45) DEFAULT NULL COMMENT '种',
  PRIMARY KEY (`KEGG_sp`,`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KEGG_sp_UNIQUE` (`KEGG_sp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。';
```



## data_orthology
KEGG基因直系同源数据

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|KEGG|VarChar (45)|``NN``|KO编号|
|name|VarChar (45)|||
|definition|VarChar (45)|||

```SQL
CREATE TABLE `data_orthology` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'KO编号',
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG基因直系同源数据';
```



## data_pathway
参考代谢途径的定义

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|KO|VarChar (45)|``NN``||
|description|VarChar (45)|||
|name|VarChar (45)|||
|map|VarChar (45)||image -> gzip -> base64 string|

```SQL
CREATE TABLE `data_pathway` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `KO` varchar(45) NOT NULL,
  `description` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `map` varchar(45) DEFAULT NULL COMMENT 'image -> gzip -> base64 string',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KO_UNIQUE` (`KO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='参考代谢途径的定义';
```



## data_reactions
KEGG之中的生物代谢反应的定义数据，这个表会包括非酶促反应过程和酶促反应过程

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|KEGG|VarChar (45)|``NN``|rn:R.... KEGG reaction id|
|EC|VarChar (45)|||
|name|VarChar (45)|||
|definition|VarChar (45)|||
|substrates|VarChar (45)||KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4,   ``data_compounds.uid``|
|products|VarChar (45)||KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4|
|comment|VarChar (45)|||

```SQL
CREATE TABLE `data_reactions` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'rn:R.... KEGG reaction id',
  `EC` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  `substrates` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4,   ``data_compounds.uid``',
  `products` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4',
  `comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG之中的生物代谢反应的定义数据，这个表会包括非酶促反应过程和酶促反应过程';
```



## data_references


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|pmid|Int64 (11)|``NN``||
|journal|VarChar (45)|||
|title|VarChar (45)|``NN``||
|authors|VarChar (45)|||

```SQL
CREATE TABLE `data_references` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `pmid` int(11) NOT NULL,
  `journal` varchar(45) DEFAULT NULL,
  `title` varchar(45) NOT NULL,
  `authors` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## link_enzymes
Enzyme in other external database

|field|type|attributes|description|
|-----|----|----------|-----------|
|enzyme|Int64 (11)|``NN``||
|EC|VarChar (45)|||
|database|VarChar (45)|||
|ID|VarChar (45)|||

```SQL
CREATE TABLE `link_enzymes` (
  `enzyme` int(11) NOT NULL,
  `EC` varchar(45) DEFAULT NULL,
  `database` varchar(45) DEFAULT NULL,
  `ID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`enzyme`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Enzyme in other external database';
```



## meta_class_br08201


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|EC|VarChar (45)|||
|level1|VarChar (45)|||
|level2|VarChar (45)|||
|level3|VarChar (45)|||

```SQL
CREATE TABLE `meta_class_br08201` (
  `uid` int(11) NOT NULL,
  `EC` varchar(45) DEFAULT NULL,
  `level1` varchar(45) DEFAULT NULL,
  `level2` varchar(45) DEFAULT NULL,
  `level3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## xref_ko_reactions
KEGG orthology links with reactions

|field|type|attributes|description|
|-----|----|----------|-----------|
|KO_uid|Int64 (11)|``NN``||
|rn|Int64 (11)|``NN``||
|KO|VarChar (45)|``NN``||
|name|VarChar (45)||KO orthology gene full name|

```SQL
CREATE TABLE `xref_ko_reactions` (
  `KO_uid` int(11) NOT NULL,
  `rn` int(11) NOT NULL,
  `KO` varchar(45) NOT NULL,
  `name` varchar(45) DEFAULT NULL COMMENT 'KO orthology gene full name',
  PRIMARY KEY (`KO_uid`,`rn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG orthology links with reactions';
```



## xref_module_reactions
代谢反应和生物模块之间的关系

|field|type|attributes|description|
|-----|----|----------|-----------|
|module|Int64 (11)|``NN``||
|reaction|Int64 (11)|``NN``||
|KEGG|VarChar (45)|``NN``|代谢反应的KEGG编号|

```SQL
CREATE TABLE `xref_module_reactions` (
  `module` int(11) NOT NULL,
  `reaction` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT '代谢反应的KEGG编号',
  PRIMARY KEY (`module`,`reaction`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢反应和生物模块之间的关系';
```



## xref_pathway_compounds
代谢途径之中所包含有的代谢物的列表

|field|type|attributes|description|
|-----|----|----------|-----------|
|pathway|Int64 (11)|``NN``||
|compound|Int64 (11)|``NN``|``data_compounds``表之中的唯一数字编号|
|KEGG|VarChar (45)|``NN``|KEGG compound id.(KEGG代谢物的编号)|
|name|VarChar (45)||代谢物的名称|

```SQL
CREATE TABLE `xref_pathway_compounds` (
  `pathway` int(11) NOT NULL,
  `compound` int(11) NOT NULL COMMENT '``data_compounds``表之中的唯一数字编号',
  `KEGG` varchar(45) NOT NULL COMMENT 'KEGG compound id.(KEGG代谢物的编号)',
  `name` varchar(45) DEFAULT NULL COMMENT '代谢物的名称',
  PRIMARY KEY (`pathway`,`compound`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径之中所包含有的代谢物的列表';
```



## xref_pathway_genes
代谢途径和所属于该代谢途径对象的基因之间的关系表

|field|type|attributes|description|
|-----|----|----------|-----------|
|pathway|Int64 (11)|``NN``||
|gene|Int64 (11)|``NN``||
|gene_KO|VarChar (45)||目标基因的KO分类编号|
|locus_tag|VarChar (45)||基因号|
|gene_name|VarChar (45)|||

```SQL
CREATE TABLE `xref_pathway_genes` (
  `pathway` int(11) NOT NULL,
  `gene` int(11) NOT NULL,
  `gene_KO` varchar(45) DEFAULT NULL COMMENT '目标基因的KO分类编号',
  `locus_tag` varchar(45) DEFAULT NULL COMMENT '基因号',
  `gene_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`gene`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径和所属于该代谢途径对象的基因之间的关系表';
```



## xref_pathway_modules


|field|type|attributes|description|
|-----|----|----------|-----------|
|pathway|Int64 (11)|``NN``||
|module|Int64 (11)|``NN``||
|KO|VarChar (45)|||
|name|VarChar (45)|||

```SQL
CREATE TABLE `xref_pathway_modules` (
  `pathway` int(11) NOT NULL,
  `module` int(11) NOT NULL,
  `KO` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## xref_pathway_references
代谢途径的参考文献

|field|type|attributes|description|
|-----|----|----------|-----------|
|pathway|Int64 (11)|``NN``||
|reference|Int64 (11)|``NN``||
|title|VarChar (45)||文献的标题|

```SQL
CREATE TABLE `xref_pathway_references` (
  `pathway` int(11) NOT NULL,
  `reference` int(11) NOT NULL,
  `title` varchar(45) DEFAULT NULL COMMENT '文献的标题',
  PRIMARY KEY (`pathway`,`reference`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径的参考文献';
```



