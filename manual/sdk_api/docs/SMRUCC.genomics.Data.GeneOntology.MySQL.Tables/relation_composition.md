# relation_composition
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `relation_composition`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relation_composition` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relation1_id` int(11) NOT NULL,
 `relation2_id` int(11) NOT NULL,
 `inferred_relation_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `relation1_id` (`relation1_id`,`relation2_id`,`inferred_relation_id`),
 KEY `rc1` (`relation1_id`),
 KEY `rc2` (`relation2_id`),
 KEY `rc3` (`inferred_relation_id`),
 KEY `rc4` (`relation1_id`,`relation2_id`,`inferred_relation_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_composition.GetDeleteSQL
```
```SQL
 DELETE FROM `relation_composition` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_composition.GetInsertSQL
```
```SQL
 INSERT INTO `relation_composition` (`relation1_id`, `relation2_id`, `inferred_relation_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_composition.GetReplaceSQL
```
```SQL
 REPLACE INTO `relation_composition` (`relation1_id`, `relation2_id`, `inferred_relation_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_composition.GetUpdateSQL
```
```SQL
 UPDATE `relation_composition` SET `id`='{0}', `relation1_id`='{1}', `relation2_id`='{2}', `inferred_relation_id`='{3}' WHERE `id` = '{4}';
 ```


