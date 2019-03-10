# assoc_rel
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `assoc_rel`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `assoc_rel` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `from_id` int(11) NOT NULL,
 `to_id` int(11) NOT NULL,
 `relationship_type_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `from_id` (`from_id`),
 KEY `to_id` (`to_id`),
 KEY `relationship_type_id` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.assoc_rel.GetDeleteSQL
```
```SQL
 DELETE FROM `assoc_rel` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.assoc_rel.GetInsertSQL
```
```SQL
 INSERT INTO `assoc_rel` (`from_id`, `to_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.assoc_rel.GetReplaceSQL
```
```SQL
 REPLACE INTO `assoc_rel` (`from_id`, `to_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.assoc_rel.GetUpdateSQL
```
```SQL
 UPDATE `assoc_rel` SET `id`='{0}', `from_id`='{1}', `to_id`='{2}', `relationship_type_id`='{3}' WHERE `id` = '{4}';
 ```


