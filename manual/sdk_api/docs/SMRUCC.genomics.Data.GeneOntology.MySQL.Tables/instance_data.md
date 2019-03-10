# instance_data
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `instance_data`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `instance_data` (
 `release_name` varchar(255) DEFAULT NULL,
 `release_type` varchar(255) DEFAULT NULL,
 `release_notes` text,
 `ontology_data_version` varchar(255) DEFAULT NULL,
 UNIQUE KEY `release_name` (`release_name`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.instance_data.GetDeleteSQL
```
```SQL
 DELETE FROM `instance_data` WHERE `release_name` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.instance_data.GetInsertSQL
```
```SQL
 INSERT INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.instance_data.GetReplaceSQL
```
```SQL
 REPLACE INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.instance_data.GetUpdateSQL
```
```SQL
 UPDATE `instance_data` SET `release_name`='{0}', `release_type`='{1}', `release_notes`='{2}', `ontology_data_version`='{3}' WHERE `release_name` = '{4}';
 ```


