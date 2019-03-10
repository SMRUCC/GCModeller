# term_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_property` (
 `term_id` int(11) NOT NULL,
 `property_key` varchar(64) NOT NULL,
 `property_val` varchar(255) DEFAULT NULL,
 KEY `term_id` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_property.GetDeleteSQL
```
```SQL
 DELETE FROM `term_property` WHERE `term_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_property.GetInsertSQL
```
```SQL
 INSERT INTO `term_property` (`term_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_property.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_property` (`term_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_property.GetUpdateSQL
```
```SQL
 UPDATE `term_property` SET `term_id`='{0}', `property_key`='{1}', `property_val`='{2}' WHERE `term_id` = '{3}';
 ```


