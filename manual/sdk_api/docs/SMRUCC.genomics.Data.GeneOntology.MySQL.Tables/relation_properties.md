# relation_properties
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `relation_properties`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relation_properties` (
 `relationship_type_id` int(11) NOT NULL,
 `is_transitive` int(11) DEFAULT NULL,
 `is_symmetric` int(11) DEFAULT NULL,
 `is_anti_symmetric` int(11) DEFAULT NULL,
 `is_cyclic` int(11) DEFAULT NULL,
 `is_reflexive` int(11) DEFAULT NULL,
 `is_metadata_tag` int(11) DEFAULT NULL,
 UNIQUE KEY `relationship_type_id` (`relationship_type_id`),
 UNIQUE KEY `rp1` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_properties.GetDeleteSQL
```
```SQL
 DELETE FROM `relation_properties` WHERE `relationship_type_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_properties.GetInsertSQL
```
```SQL
 INSERT INTO `relation_properties` (`relationship_type_id`, `is_transitive`, `is_symmetric`, `is_anti_symmetric`, `is_cyclic`, `is_reflexive`, `is_metadata_tag`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_properties.GetReplaceSQL
```
```SQL
 REPLACE INTO `relation_properties` (`relationship_type_id`, `is_transitive`, `is_symmetric`, `is_anti_symmetric`, `is_cyclic`, `is_reflexive`, `is_metadata_tag`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.relation_properties.GetUpdateSQL
```
```SQL
 UPDATE `relation_properties` SET `relationship_type_id`='{0}', `is_transitive`='{1}', `is_symmetric`='{2}', `is_anti_symmetric`='{3}', `is_cyclic`='{4}', `is_reflexive`='{5}', `is_metadata_tag`='{6}' WHERE `relationship_type_id` = '{7}';
 ```


