# taxonomy
_namespace: [SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `taxonomy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxonomy` (
 `taxid` int(11) NOT NULL,
 `name` varchar(64) DEFAULT NULL,
 `rank` int(11) DEFAULT NULL,
 `parent` int(11) NOT NULL,
 `childs` mediumtext,
 PRIMARY KEY (`taxid`),
 UNIQUE KEY `taxid_UNIQUE` (`taxid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2016-10-04 20:02:09
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.taxonomy.GetDeleteSQL
```
```SQL
 DELETE FROM `taxonomy` WHERE `taxid` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.taxonomy.GetInsertSQL
```
```SQL
 INSERT INTO `taxonomy` (`taxid`, `name`, `rank`, `parent`, `childs`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.taxonomy.GetReplaceSQL
```
```SQL
 REPLACE INTO `taxonomy` (`taxid`, `name`, `rank`, `parent`, `childs`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.taxonomy.GetUpdateSQL
```
```SQL
 UPDATE `taxonomy` SET `taxid`='{0}', `name`='{1}', `rank`='{2}', `parent`='{3}', `childs`='{4}' WHERE `taxid` = '{5}';
 ```


