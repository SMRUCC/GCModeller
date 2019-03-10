# nt
_namespace: [SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI](./index.md)_

```SQL
 nt sequence database
 
 --
 
 DROP TABLE IF EXISTS `nt`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `nt` (
 `gi` int(11) NOT NULL,
 `db` varchar(32) NOT NULL,
 `uid` varchar(32) NOT NULL,
 `description` tinytext NOT NULL,
 `taxid` int(11) NOT NULL COMMENT 'taxonomy id',
 PRIMARY KEY (`gi`),
 UNIQUE KEY `gi_UNIQUE` (`gi`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='nt sequence database';
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.nt.GetDeleteSQL
```
```SQL
 DELETE FROM `nt` WHERE `gi` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.nt.GetInsertSQL
```
```SQL
 INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.nt.GetReplaceSQL
```
```SQL
 REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.nt.GetUpdateSQL
```
```SQL
 UPDATE `nt` SET `gi`='{0}', `db`='{1}', `uid`='{2}', `description`='{3}', `taxid`='{4}' WHERE `gi` = '{5}';
 ```


### Properties

#### taxid
taxonomy id
