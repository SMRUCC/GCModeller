# intersection_of
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `intersection_of`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `intersection_of` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL,
 `term1_id` int(11) NOT NULL,
 `term2_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term2_id` (`term2_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.intersection_of.GetDeleteSQL
```
```SQL
 DELETE FROM `intersection_of` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.intersection_of.GetInsertSQL
```
```SQL
 INSERT INTO `intersection_of` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.intersection_of.GetReplaceSQL
```
```SQL
 REPLACE INTO `intersection_of` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.intersection_of.GetUpdateSQL
```
```SQL
 UPDATE `intersection_of` SET `id`='{0}', `relationship_type_id`='{1}', `term1_id`='{2}', `term2_id`='{3}' WHERE `id` = '{4}';
 ```


