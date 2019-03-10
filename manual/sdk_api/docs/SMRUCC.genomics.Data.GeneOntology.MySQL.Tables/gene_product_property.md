# gene_product_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_property` (
 `gene_product_id` int(11) NOT NULL,
 `property_key` varchar(64) NOT NULL,
 `property_val` varchar(255) DEFAULT NULL,
 UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
 KEY `gpp1` (`gene_product_id`),
 KEY `gpp2` (`property_key`),
 KEY `gpp3` (`property_val`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_property.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_property` WHERE `gene_product_id`='{0}' and `property_key`='{1}' and `property_val`='{2}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_property.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_property.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_property.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_property` SET `gene_product_id`='{0}', `property_key`='{1}', `property_val`='{2}' WHERE `gene_product_id`='{3}' and `property_key`='{4}' and `property_val`='{5}';
 ```


