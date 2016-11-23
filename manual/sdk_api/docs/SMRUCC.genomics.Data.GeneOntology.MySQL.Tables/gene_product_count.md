# gene_product_count
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_count`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_count` (
 `term_id` int(11) NOT NULL,
 `code` varchar(8) DEFAULT NULL,
 `speciesdbname` varchar(55) DEFAULT NULL,
 `species_id` int(11) DEFAULT NULL,
 `product_count` int(11) NOT NULL,
 KEY `species_id` (`species_id`),
 KEY `gpc1` (`term_id`),
 KEY `gpc2` (`code`),
 KEY `gpc3` (`speciesdbname`),
 KEY `gpc4` (`term_id`,`code`,`speciesdbname`),
 KEY `gpc5` (`term_id`,`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_count.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_count` WHERE `species_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_count.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_count` (`term_id`, `code`, `speciesdbname`, `species_id`, `product_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_count.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_count` (`term_id`, `code`, `speciesdbname`, `species_id`, `product_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_count.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_count` SET `term_id`='{0}', `code`='{1}', `speciesdbname`='{2}', `species_id`='{3}', `product_count`='{4}' WHERE `species_id` = '{5}';
 ```


