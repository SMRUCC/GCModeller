# association_species_qualifier
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `association_species_qualifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_species_qualifier` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL,
 `species_id` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 KEY `association_id` (`association_id`),
 KEY `species_id` (`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_species_qualifier.GetDeleteSQL
```
```SQL
 DELETE FROM `association_species_qualifier` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_species_qualifier.GetInsertSQL
```
```SQL
 INSERT INTO `association_species_qualifier` (`association_id`, `species_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_species_qualifier.GetReplaceSQL
```
```SQL
 REPLACE INTO `association_species_qualifier` (`association_id`, `species_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_species_qualifier.GetUpdateSQL
```
```SQL
 UPDATE `association_species_qualifier` SET `id`='{0}', `association_id`='{1}', `species_id`='{2}' WHERE `id` = '{3}';
 ```


