# homolset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `homolset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `homolset` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `symbol` varchar(128) DEFAULT NULL,
 `dbxref_id` int(11) DEFAULT NULL,
 `target_gene_product_id` int(11) DEFAULT NULL,
 `taxon_id` int(11) DEFAULT NULL,
 `type_id` int(11) DEFAULT NULL,
 `description` text,
 PRIMARY KEY (`id`),
 UNIQUE KEY `dbxref_id` (`dbxref_id`),
 KEY `target_gene_product_id` (`target_gene_product_id`),
 KEY `taxon_id` (`taxon_id`),
 KEY `type_id` (`type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.homolset.GetDeleteSQL
```
```SQL
 DELETE FROM `homolset` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.homolset.GetInsertSQL
```
```SQL
 INSERT INTO `homolset` (`symbol`, `dbxref_id`, `target_gene_product_id`, `taxon_id`, `type_id`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.homolset.GetReplaceSQL
```
```SQL
 REPLACE INTO `homolset` (`symbol`, `dbxref_id`, `target_gene_product_id`, `taxon_id`, `type_id`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.homolset.GetUpdateSQL
```
```SQL
 UPDATE `homolset` SET `id`='{0}', `symbol`='{1}', `dbxref_id`='{2}', `target_gene_product_id`='{3}', `taxon_id`='{4}', `type_id`='{5}', `description`='{6}' WHERE `id` = '{7}';
 ```


