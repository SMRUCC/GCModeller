# term
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `name` varchar(255) NOT NULL DEFAULT '',
 `term_type` varchar(55) NOT NULL,
 `acc` varchar(255) NOT NULL,
 `is_obsolete` int(11) NOT NULL DEFAULT '0',
 `is_root` int(11) NOT NULL DEFAULT '0',
 `is_relation` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `acc` (`acc`),
 UNIQUE KEY `t0` (`id`),
 KEY `t1` (`name`),
 KEY `t2` (`term_type`),
 KEY `t3` (`acc`),
 KEY `t4` (`id`,`acc`),
 KEY `t5` (`id`,`name`),
 KEY `t6` (`id`,`term_type`),
 KEY `t7` (`id`,`acc`,`name`,`term_type`)
 ) ENGINE=MyISAM AUTO_INCREMENT=43827 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term.GetDeleteSQL
```
```SQL
 DELETE FROM `term` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term.GetInsertSQL
```
```SQL
 INSERT INTO `term` (`name`, `term_type`, `acc`, `is_obsolete`, `is_root`, `is_relation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term.GetReplaceSQL
```
```SQL
 REPLACE INTO `term` (`name`, `term_type`, `acc`, `is_obsolete`, `is_root`, `is_relation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term.GetUpdateSQL
```
```SQL
 UPDATE `term` SET `id`='{0}', `name`='{1}', `term_type`='{2}', `acc`='{3}', `is_obsolete`='{4}', `is_root`='{5}', `is_relation`='{6}' WHERE `id` = '{7}';
 ```


