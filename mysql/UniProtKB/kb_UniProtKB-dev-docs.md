# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## alt_id
当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号

|field|type|attributes|description|
|-----|----|----------|-----------|
|primary_hashcode|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|alt_id|Int64 (10)|``NN``||
|name|VarChar (45)|||

```SQL
CREATE TABLE `alt_id` (
  `primary_hashcode` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`primary_hashcode`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号';
```



## feature_site_variation
序列的突变位点

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|original|VarChar (45)|||
|variation|VarChar (45)|||
|position|VarChar (45)|||

```SQL
CREATE TABLE `feature_site_variation` (
  `uid` int(10) unsigned NOT NULL,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `original` varchar(45) DEFAULT NULL,
  `variation` varchar(45) DEFAULT NULL,
  `position` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`,`hash_code`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='序列的突变位点';
```



## feature_types


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|type_name|VarChar (45)|``NN``||

```SQL
CREATE TABLE `feature_types` (
  `uid` int(10) unsigned NOT NULL,
  `type_name` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## gene_info


|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|gene_name|VarChar (45)|||
|ORF|VarChar (45)|||
|synonym1|VarChar (45)|||
|synonym2|VarChar (45)|||
|synonym3|VarChar (45)|||

```SQL
CREATE TABLE `gene_info` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  `ORF` varchar(45) DEFAULT NULL,
  `synonym1` varchar(45) DEFAULT NULL,
  `synonym2` varchar(45) DEFAULT NULL,
  `synonym3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## hash_table
这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了

|field|type|attributes|description|
|-----|----|----------|-----------|
|uniprot_id|VarChar (32)|``NN``|uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果|
|hash_code|Int64 (10)|``AI``, ``NN``|每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号|
|name|VarChar (45)|||

```SQL
CREATE TABLE `hash_table` (
  `uniprot_id` char(32) NOT NULL COMMENT 'uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果',
  `hash_code` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号',
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了';
```



## keywords


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``AI``, ``NN``||
|keyword|VarChar (45)|``NN``||

```SQL
CREATE TABLE `keywords` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `keyword` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## literature
文献报道数据

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``AI``, ``NN``||
|type|VarChar (45)|||
|date|VarChar (45)|||
|db|VarChar (45)|||
|title|VarChar (45)|||
|pubmed|VarChar (45)|||
|doi|VarChar (45)|||
|volume|VarChar (45)|||
|pages|VarChar (45)|||
|journal|VarChar (45)|||

```SQL
CREATE TABLE `literature` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `type` varchar(45) DEFAULT NULL,
  `date` varchar(45) DEFAULT NULL,
  `db` varchar(45) DEFAULT NULL,
  `title` varchar(45) DEFAULT NULL,
  `pubmed` varchar(45) DEFAULT NULL,
  `doi` varchar(45) DEFAULT NULL,
  `volume` varchar(45) DEFAULT NULL,
  `pages` varchar(45) DEFAULT NULL,
  `journal` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='文献报道数据';
```



## location_id


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|name|VarChar (45)|||

```SQL
CREATE TABLE `location_id` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## organism_code
物种信息简表

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``|在这里使用的是NCBI Taxonomy编号|
|organism_name|VarChar (100)|``NN``||
|domain|VarChar (45)|||
|kingdom|VarChar (45)|||
|phylum|VarChar (45)|||
|class|VarChar (45)|||
|order|VarChar (45)|||
|family|VarChar (45)|||
|genus|VarChar (45)|||
|species|VarChar (45)|||
|full|Text|``NN``|除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息|

```SQL
CREATE TABLE `organism_code` (
  `uid` int(10) unsigned NOT NULL COMMENT '在这里使用的是NCBI Taxonomy编号',
  `organism_name` varchar(100) NOT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL,
  `phylum` varchar(45) DEFAULT NULL,
  `class` varchar(45) DEFAULT NULL,
  `order` varchar(45) DEFAULT NULL,
  `family` varchar(45) DEFAULT NULL,
  `genus` varchar(45) DEFAULT NULL,
  `species` varchar(45) DEFAULT NULL,
  `full` mediumtext NOT NULL COMMENT '除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `organism_name_UNIQUE` (`organism_name`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='物种信息简表';
```



## organism_proteome
这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合

|field|type|attributes|description|
|-----|----|----------|-----------|
|org_id|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|id_hashcode|Int64 (10)|``NN``||
|gene_name|VarChar (45)|||

```SQL
CREATE TABLE `organism_proteome` (
  `org_id` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `id_hashcode` int(10) unsigned NOT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`org_id`,`id_hashcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合';
```



## peoples


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|name|VarChar (45)|``NN``||

```SQL
CREATE TABLE `peoples` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## protein_alternative_name


|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|``NN``||
|name|VarChar (45)|``NN``||
|fullName|VarChar (45)|||
|shortName1|VarChar (45)|||
|shortName2|VarChar (45)|||
|shortName3|VarChar (45)|||
|shortName4|VarChar (45)|||
|shortName5|VarChar (45)|||

```SQL
CREATE TABLE `protein_alternative_name` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `name` varchar(45) NOT NULL,
  `fullName` varchar(45) DEFAULT NULL,
  `shortName1` varchar(45) DEFAULT NULL,
  `shortName2` varchar(45) DEFAULT NULL,
  `shortName3` varchar(45) DEFAULT NULL,
  `shortName4` varchar(45) DEFAULT NULL,
  `shortName5` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## protein_feature_regions


|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|type_id|Int64 (10)|``NN``||
|type|VarChar (45)|||
|description|VarChar (45)|||
|begin|VarChar (45)|||
|end|VarChar (45)|||

```SQL
CREATE TABLE `protein_feature_regions` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `type_id` int(10) unsigned NOT NULL,
  `type` varchar(45) DEFAULT NULL,
  `description` varchar(45) DEFAULT NULL,
  `begin` varchar(45) DEFAULT NULL,
  `end` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## protein_feature_site


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|type_id|Int64 (10)|``NN``||
|type|VarChar (45)|||
|description|VarChar (45)|||
|position|VarChar (45)|||

```SQL
CREATE TABLE `protein_feature_site` (
  `uid` int(10) unsigned NOT NULL,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `type_id` int(10) unsigned NOT NULL,
  `type` varchar(45) DEFAULT NULL,
  `description` varchar(45) DEFAULT NULL,
  `position` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`,`hash_code`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## protein_functions
对蛋白质的名称以及功能方面的字符串描述

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|function|VarChar (45)||comment -> type = function|
|name|VarChar (45)|||
|full_name|VarChar (45)||recommendedName|
|short_name1|VarChar (45)||recommendedName|
|short_name2|VarChar (45)||recommendedName|
|short_name3|VarChar (45)||recommendedName|

```SQL
CREATE TABLE `protein_functions` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `function` varchar(45) DEFAULT NULL COMMENT 'comment -> type = function',
  `name` varchar(45) DEFAULT NULL,
  `full_name` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name1` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name2` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name3` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  PRIMARY KEY (`hash_code`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的名称以及功能方面的字符串描述';
```



## protein_go
对蛋白质的GO功能注释的信息关联表

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|``NN``||
|go_id|Int64 (10)|``NN``||
|namespace_id|Int64 (10)|``NN``||
|namespace|VarChar (45)|||

```SQL
CREATE TABLE `protein_go` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `go_id` int(10) unsigned NOT NULL,
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`go_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的GO功能注释的信息关联表';
```



## protein_keywords


|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|``NN``||
|keyword_id|Int64 (10)|``NN``||
|keyword|VarChar (45)|``NN``||

```SQL
CREATE TABLE `protein_keywords` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `keyword_id` int(10) unsigned NOT NULL,
  `keyword` varchar(45) NOT NULL,
  PRIMARY KEY (`hash_code`,`keyword_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## protein_ko
蛋白质的KEGG直系同源的注释信息表，uniprotKB库通过这个表连接kegg知识库

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|KO|Int64 (10)|``NN``||

```SQL
CREATE TABLE `protein_ko` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `KO` int(10) unsigned NOT NULL,
  PRIMARY KEY (`hash_code`,`KO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质的KEGG直系同源的注释信息表，uniprotKB库通过这个表连接kegg知识库';
```



## protein_reference
对这个蛋白质的文献报道数据

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|reference_id|Int64 (10)|``NN``||
|scope|VarChar (45)|||

```SQL
CREATE TABLE `protein_reference` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `reference_id` int(10) unsigned NOT NULL,
  `scope` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`reference_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对这个蛋白质的文献报道数据';
```



## protein_structures
主要是pdb结构记录数据

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|``NN``||
|pdb_id|VarChar (45)|||
|method|VarChar (45)|||
|resolution|VarChar (45)|||
|chains|VarChar (45)|||

```SQL
CREATE TABLE `protein_structures` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `pdb_id` varchar(45) DEFAULT NULL,
  `method` varchar(45) DEFAULT NULL,
  `resolution` varchar(45) DEFAULT NULL,
  `chains` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='主要是pdb结构记录数据';
```



## protein_subcellular_location
目标蛋白质在细胞质中的亚细胞定位结果

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|location|VarChar (45)|||
|location_id|Int64 (10)|||
|topology|VarChar (45)|||
|topology_id|Int64 (10)|||

```SQL
CREATE TABLE `protein_subcellular_location` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `location` varchar(45) DEFAULT NULL,
  `location_id` int(10) unsigned DEFAULT NULL,
  `topology` varchar(45) DEFAULT NULL,
  `topology_id` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='目标蛋白质在细胞质中的亚细胞定位结果';
```



## research_jobs


|field|type|attributes|description|
|-----|----|----------|-----------|
|person|Int64 (10)|``NN``||
|people_name|VarChar (45)|||
|literature_id|Int64 (10)|``NN``||
|literature_title|VarChar (45)|||

```SQL
CREATE TABLE `research_jobs` (
  `person` int(10) unsigned NOT NULL,
  `people_name` varchar(45) DEFAULT NULL,
  `literature_id` int(10) unsigned NOT NULL,
  `literature_title` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`person`,`literature_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## seq_archive
蛋白质序列存储表

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|``NN``|UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)|
|entry_name|VarChar (45)||EntryName Is the entry name of the UniProtKB entry.|
|organism_id|Int64 (10)|``NN``|OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.|
|organism_name|Text|``NN``|对organism_id校验所使用的|
|gn|VarChar (45)||GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.|
|pe|VarChar (45)||ProteinExistence Is the numerical value describing the evidence for the existence of the protein.|
|sv|VarChar (45)||SequenceVersion Is the version number of the sequence.|
|prot_name|Text||ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.|
|length|Int64 (11)||length of the protein sequence|
|sequence|Text||protein sequence|

```SQL
CREATE TABLE `seq_archive` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)',
  `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
  `organism_id` int(10) unsigned NOT NULL COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.',
  `organism_name` longtext NOT NULL COMMENT '对organism_id校验所使用的',
  `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
  `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
  `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
  `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
  `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
  `sequence` text COMMENT 'protein sequence',
  PRIMARY KEY (`hash_code`,`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质序列存储表';
```



## tissue_code
对某一个物种的组织进行编号

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``AI``, ``NN``||
|tissue_name|VarChar (45)|``NN``||
|org_id|Int64 (10)|||
|organism|VarChar (45)||物种名称|

```SQL
CREATE TABLE `tissue_code` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `tissue_name` varchar(45) NOT NULL,
  `org_id` int(10) unsigned DEFAULT NULL,
  `organism` varchar(45) DEFAULT NULL COMMENT '物种名称',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对某一个物种的组织进行编号';
```



## tissue_locations


|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|name|VarChar (45)|||
|tissue_id|Int64 (10)|``NN``||
|tissue_name|VarChar (45)|||

```SQL
CREATE TABLE `tissue_locations` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `tissue_id` int(10) unsigned NOT NULL,
  `tissue_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`tissue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## topology_id


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (10)|``NN``||
|name|VarChar (45)|||

```SQL
CREATE TABLE `topology_id` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## xref
某一个uniprot蛋白质记录对外部的链接信息

|field|type|attributes|description|
|-----|----|----------|-----------|
|hash_code|Int64 (10)|``NN``||
|uniprot_id|VarChar (45)|||
|xref|VarChar (45)|``NN``||
|external_id|VarChar (45)|``NN``||
|molecule_type|VarChar (45)|||
|protein_ID|VarChar (45)|||
|nucleotide_ID|VarChar (45)|||

```SQL
CREATE TABLE `xref` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `xref` varchar(45) NOT NULL,
  `external_id` varchar(45) NOT NULL,
  `molecule_type` varchar(45) DEFAULT NULL,
  `protein_ID` varchar(45) DEFAULT NULL,
  `nucleotide_ID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='某一个uniprot蛋白质记录对外部的链接信息';
```



