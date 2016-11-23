# species
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `species`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `species` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `ncbi_taxa_id` int(11) DEFAULT NULL,
 `common_name` varchar(255) DEFAULT NULL,
 `lineage_string` text,
 `genus` varchar(55) DEFAULT NULL,
 `species` varchar(255) DEFAULT NULL,
 `parent_id` int(11) DEFAULT NULL,
 `left_value` int(11) DEFAULT NULL,
 `right_value` int(11) DEFAULT NULL,
 `taxonomic_rank` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `sp0` (`id`),
 UNIQUE KEY `ncbi_taxa_id` (`ncbi_taxa_id`),
 KEY `sp1` (`ncbi_taxa_id`),
 KEY `sp2` (`common_name`),
 KEY `sp3` (`genus`),
 KEY `sp4` (`species`),
 KEY `sp5` (`genus`,`species`),
 KEY `sp6` (`id`,`ncbi_taxa_id`),
 KEY `sp7` (`id`,`ncbi_taxa_id`,`genus`,`species`),
 KEY `sp8` (`parent_id`),
 KEY `sp9` (`left_value`),
 KEY `sp10` (`right_value`),
 KEY `sp11` (`left_value`,`right_value`),
 KEY `sp12` (`id`,`left_value`),
 KEY `sp13` (`genus`,`left_value`,`right_value`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.species.GetDeleteSQL
```
```SQL
 DELETE FROM `species` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.species.GetInsertSQL
```
```SQL
 INSERT INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.species.GetReplaceSQL
```
```SQL
 REPLACE INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.species.GetUpdateSQL
```
```SQL
 UPDATE `species` SET `id`='{0}', `ncbi_taxa_id`='{1}', `common_name`='{2}', `lineage_string`='{3}', `genus`='{4}', `species`='{5}', `parent_id`='{6}', `left_value`='{7}', `right_value`='{8}', `taxonomic_rank`='{9}' WHERE `id` = '{10}';
 ```


