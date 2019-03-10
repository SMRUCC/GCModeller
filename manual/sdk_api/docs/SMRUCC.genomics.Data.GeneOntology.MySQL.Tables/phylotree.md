# phylotree
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `phylotree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `phylotree` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `name` varchar(255) NOT NULL DEFAULT '',
 `dbxref_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `dbxref_id` (`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree.GetDeleteSQL
```
```SQL
 DELETE FROM `phylotree` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree.GetInsertSQL
```
```SQL
 INSERT INTO `phylotree` (`name`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree.GetReplaceSQL
```
```SQL
 REPLACE INTO `phylotree` (`name`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree.GetUpdateSQL
```
```SQL
 UPDATE `phylotree` SET `id`='{0}', `name`='{1}', `dbxref_id`='{2}' WHERE `id` = '{3}';
 ```


