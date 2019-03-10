# source_audit
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `source_audit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `source_audit` (
 `source_id` varchar(255) DEFAULT NULL,
 `source_fullpath` varchar(255) DEFAULT NULL,
 `source_path` varchar(255) DEFAULT NULL,
 `source_type` varchar(255) DEFAULT NULL,
 `source_md5` char(32) DEFAULT NULL,
 `source_parsetime` int(11) DEFAULT NULL,
 `source_mtime` int(11) DEFAULT NULL,
 KEY `fa1` (`source_path`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.source_audit.GetDeleteSQL
```
```SQL
 DELETE FROM `source_audit` WHERE `source_path` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.source_audit.GetInsertSQL
```
```SQL
 INSERT INTO `source_audit` (`source_id`, `source_fullpath`, `source_path`, `source_type`, `source_md5`, `source_parsetime`, `source_mtime`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.source_audit.GetReplaceSQL
```
```SQL
 REPLACE INTO `source_audit` (`source_id`, `source_fullpath`, `source_path`, `source_type`, `source_md5`, `source_parsetime`, `source_mtime`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.source_audit.GetUpdateSQL
```
```SQL
 UPDATE `source_audit` SET `source_id`='{0}', `source_fullpath`='{1}', `source_path`='{2}', `source_type`='{3}', `source_md5`='{4}', `source_parsetime`='{5}', `source_mtime`='{6}' WHERE `source_path` = '{7}';
 ```


