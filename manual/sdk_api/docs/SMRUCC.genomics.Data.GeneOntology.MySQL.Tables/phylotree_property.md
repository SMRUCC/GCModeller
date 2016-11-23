# phylotree_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `phylotree_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `phylotree_property` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `phylotree_id` int(11) NOT NULL,
 `property_key` varchar(64) NOT NULL,
 `property_val` mediumtext,
 PRIMARY KEY (`id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree_property.GetDeleteSQL
```
```SQL
 DELETE FROM `phylotree_property` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree_property.GetInsertSQL
```
```SQL
 INSERT INTO `phylotree_property` (`phylotree_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree_property.GetReplaceSQL
```
```SQL
 REPLACE INTO `phylotree_property` (`phylotree_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.phylotree_property.GetUpdateSQL
```
```SQL
 UPDATE `phylotree_property` SET `id`='{0}', `phylotree_id`='{1}', `property_key`='{2}', `property_val`='{3}' WHERE `id` = '{4}';
 ```


