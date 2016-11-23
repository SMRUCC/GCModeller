# term2term_metadata
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term2term_metadata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term2term_metadata` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL,
 `term1_id` int(11) NOT NULL,
 `term2_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term2_id` (`term2_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=2317 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term_metadata.GetDeleteSQL
```
```SQL
 DELETE FROM `term2term_metadata` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term_metadata.GetInsertSQL
```
```SQL
 INSERT INTO `term2term_metadata` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term_metadata.GetReplaceSQL
```
```SQL
 REPLACE INTO `term2term_metadata` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term_metadata.GetUpdateSQL
```
```SQL
 UPDATE `term2term_metadata` SET `id`='{0}', `relationship_type_id`='{1}', `term1_id`='{2}', `term2_id`='{3}' WHERE `id` = '{4}';
 ```


