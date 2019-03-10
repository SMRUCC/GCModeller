# association_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `association_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_property` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL,
 `relationship_type_id` int(11) NOT NULL,
 `term_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `association_id` (`association_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term_id` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_property.GetDeleteSQL
```
```SQL
 DELETE FROM `association_property` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_property.GetInsertSQL
```
```SQL
 INSERT INTO `association_property` (`association_id`, `relationship_type_id`, `term_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_property.GetReplaceSQL
```
```SQL
 REPLACE INTO `association_property` (`association_id`, `relationship_type_id`, `term_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_property.GetUpdateSQL
```
```SQL
 UPDATE `association_property` SET `id`='{0}', `association_id`='{1}', `relationship_type_id`='{2}', `term_id`='{3}' WHERE `id` = '{4}';
 ```


