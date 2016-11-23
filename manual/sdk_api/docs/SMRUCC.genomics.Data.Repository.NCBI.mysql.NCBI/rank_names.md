# rank_names
_namespace: [SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `rank_names`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `rank_names` (
 `id` int(11) NOT NULL,
 `name` varchar(45) DEFAULT NULL,
 PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.rank_names.GetDeleteSQL
```
```SQL
 DELETE FROM `rank_names` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.rank_names.GetInsertSQL
```
```SQL
 INSERT INTO `rank_names` (`id`, `name`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.rank_names.GetReplaceSQL
```
```SQL
 REPLACE INTO `rank_names` (`id`, `name`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.Repository.NCBI.mysql.NCBI.rank_names.GetUpdateSQL
```
```SQL
 UPDATE `rank_names` SET `id`='{0}', `name`='{1}' WHERE `id` = '{2}';
 ```


