# gene_product
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `symbol` varchar(128) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 `species_id` int(11) DEFAULT NULL,
 `type_id` int(11) DEFAULT NULL,
 `full_name` text,
 PRIMARY KEY (`id`),
 UNIQUE KEY `dbxref_id` (`dbxref_id`),
 UNIQUE KEY `g0` (`id`),
 KEY `type_id` (`type_id`),
 KEY `g1` (`symbol`),
 KEY `g2` (`dbxref_id`),
 KEY `g3` (`species_id`),
 KEY `g4` (`id`,`species_id`),
 KEY `g5` (`dbxref_id`,`species_id`),
 KEY `g6` (`id`,`dbxref_id`),
 KEY `g7` (`id`,`species_id`),
 KEY `g8` (`id`,`dbxref_id`,`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product` (`symbol`, `dbxref_id`, `species_id`, `type_id`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product` (`symbol`, `dbxref_id`, `species_id`, `type_id`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product.GetUpdateSQL
```
```SQL
 UPDATE `gene_product` SET `id`='{0}', `symbol`='{1}', `dbxref_id`='{2}', `species_id`='{3}', `type_id`='{4}', `full_name`='{5}' WHERE `id` = '{6}';
 ```


