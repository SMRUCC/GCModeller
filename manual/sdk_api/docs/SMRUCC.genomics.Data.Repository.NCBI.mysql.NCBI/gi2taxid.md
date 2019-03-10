# gi2taxid
_namespace: [SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gi2taxid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gi2taxid` (
 `gi` int(11) NOT NULL,
 `taxid` int(11) NOT NULL,
 PRIMARY KEY (`gi`,`taxid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.gi2taxid.GetDeleteSQL
```
```SQL
 DELETE FROM `gi2taxid` WHERE `gi`='{0}' and `taxid`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.gi2taxid.GetInsertSQL
```
```SQL
 INSERT INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.gi2taxid.GetReplaceSQL
```
```SQL
 REPLACE INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.gi2taxid.GetUpdateSQL
```
```SQL
 UPDATE `gi2taxid` SET `gi`='{0}', `taxid`='{1}' WHERE `gi`='{2}' and `taxid`='{3}';
 ```


