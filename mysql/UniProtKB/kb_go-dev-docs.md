# MySQL development docs

Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## alt_id


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``||
|alt_id|Int64 (10)|``NN``||
|name|Text|||

```SQL
CREATE TABLE `alt_id` (
  `id` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` mediumtext,
  PRIMARY KEY (`id`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## dag_relationship


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``||
|relationship|VarChar (45)|||
|relationship_id|Int64 (10)|``NN``||
|term_id|Int64 (10)|``NN``||
|name|VarChar (45)|||

```SQL
CREATE TABLE `dag_relationship` (
  `id` int(10) unsigned NOT NULL,
  `relationship` varchar(45) DEFAULT NULL,
  `relationship_id` int(10) unsigned NOT NULL,
  `term_id` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`,`term_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## go_terms


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``|其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字|
|term|VarChar (16)|``NN``|GO id|
|name|VarChar (45)|||
|namespace_id|Int64 (10)|``NN``||
|namespace|VarChar (45)|``NN``||
|def|Text|``NN``||
|is_obsolete|Int64 (4)|``NN``|0 为 False, 1 为 True|
|comment|Text|||

```SQL
CREATE TABLE `go_terms` (
  `id` int(10) unsigned NOT NULL COMMENT '其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字',
  `term` char(16) NOT NULL COMMENT 'GO id',
  `name` varchar(45) DEFAULT NULL,
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` varchar(45) NOT NULL,
  `def` longtext NOT NULL,
  `is_obsolete` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 为 False, 1 为 True',
  `comment` longtext,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## relation_names


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``AI``, ``NN``||
|name|Text|``NN``||

```SQL
CREATE TABLE `relation_names` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` mediumtext NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## term_namespace


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``||
|namespace|Text|``NN``|这个表里面只有三个值|

```SQL
CREATE TABLE `term_namespace` (
  `id` int(10) unsigned zerofill NOT NULL,
  `namespace` tinytext NOT NULL COMMENT '这个表里面只有三个值',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## term_synonym


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``AI``, ``NN``||
|term_id|Int64 (10)|``NN``||
|synonym|Text|``NN``||
|type|VarChar (45)||EXACT []  表示完全一样<br />RELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 |
|object|VarChar (45)|||

```SQL
CREATE TABLE `term_synonym` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `term_id` int(10) unsigned NOT NULL,
  `synonym` mediumtext NOT NULL,
  `type` varchar(45) DEFAULT NULL COMMENT 'EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 ',
  `object` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



## xref


|field|type|attributes|description|
|-----|----|----------|-----------|
|go_id|Int64 (10)|``NN``||
|xref|VarChar (45)|``NN``|外部数据库名称|
|external_id|VarChar (45)|``NN``|外部数据库编号|
|comment|Text|||

```SQL
CREATE TABLE `xref` (
  `go_id` int(10) unsigned NOT NULL,
  `xref` varchar(45) NOT NULL COMMENT '外部数据库名称',
  `external_id` varchar(45) NOT NULL COMMENT '外部数据库编号',
  `comment` longtext,
  PRIMARY KEY (`go_id`,`external_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



