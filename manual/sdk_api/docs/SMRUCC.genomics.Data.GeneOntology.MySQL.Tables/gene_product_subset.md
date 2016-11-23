# gene_product_subset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_subset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_subset` (
 `gene_product_id` int(11) NOT NULL,
 `subset_id` int(11) NOT NULL,
 UNIQUE KEY `gps3` (`gene_product_id`,`subset_id`),
 KEY `gps1` (`gene_product_id`),
 KEY `gps2` (`subset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_subset.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_subset` WHERE `gene_product_id`='{0}' and `subset_id`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_subset.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_subset` (`gene_product_id`, `subset_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_subset.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_subset` (`gene_product_id`, `subset_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_subset.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_subset` SET `gene_product_id`='{0}', `subset_id`='{1}' WHERE `gene_product_id`='{2}' and `subset_id`='{3}';
 ```


