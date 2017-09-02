# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## alt_id
GO_term的主编号和次级编号之间的关系

|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``||
|alt_id|Int64 (10)|``NN``||
|name|Text||The name field in the go_term|

```SQL
CREATE TABLE `alt_id` (
  `id` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` mediumtext COMMENT 'The name field in the go_term',
  PRIMARY KEY (`id`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的主编号和次级编号之间的关系';
```



## dag_relationship
由GO_term之间的相互关系所构成的有向无环图Directed Acyclic Graph（DAG）

|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``NN``|当前的term编号|
|relationship|VarChar (45)|||
|relationship_id|Int64 (10)|``NN``|二者之间的关系编号，由于可能会存在多种互做类型，所以只使用id+term_id的结构来做主键会出现重复entry的问题，在这里将作用的类型也加入进来|
|term_id|Int64 (10)|``NN``|与当前的term发生互做关系的另外的一个partner term的编号|
|name|VarChar (45)||发生关系的term的名字|

```SQL
CREATE TABLE `dag_relationship` (
  `id` int(10) unsigned NOT NULL COMMENT '当前的term编号',
  `relationship` varchar(45) DEFAULT NULL,
  `relationship_id` int(10) unsigned NOT NULL COMMENT '二者之间的关系编号，由于可能会存在多种互做类型，所以只使用id+term_id的结构来做主键会出现重复entry的问题，在这里将作用的类型也加入进来',
  `term_id` int(10) unsigned NOT NULL COMMENT '与当前的term发生互做关系的另外的一个partner term的编号',
  `name` varchar(45) DEFAULT NULL COMMENT '发生关系的term的名字',
  PRIMARY KEY (`id`,`term_id`,`relationship_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='由GO_term之间的相互关系所构成的有向无环图Directed Acyclic Graph（DAG）';
```



## go_terms
GO_term的具体的定义内容

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的具体的定义内容';
```



## relation_names
枚举所有的关系的名称

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='枚举所有的关系的名称';
```



## term_namespace
枚举三个命名空间

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='枚举三个命名空间';
```



## term_synonym
GO_term的同义词表

|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (10)|``AI``, ``NN``|自增编号|
|term_id|Int64 (10)|``NN``|当前的Go term的编号|
|synonym|Text|``NN``|同义名称|
|type|VarChar (45)||EXACT []  表示完全一样<br />RELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 |
|object|VarChar (45)||type所指向的类型，可以会为空|

```SQL
CREATE TABLE `term_synonym` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '自增编号',
  `term_id` int(10) unsigned NOT NULL COMMENT '当前的Go term的编号',
  `synonym` mediumtext NOT NULL COMMENT '同义名称',
  `type` varchar(45) DEFAULT NULL COMMENT 'EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 ',
  `object` varchar(45) DEFAULT NULL COMMENT 'type所指向的类型，可以会为空',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的同义词表';
```



## xref
GO_term与外部数据库之间的相互关联

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term与外部数据库之间的相互关联';
```



