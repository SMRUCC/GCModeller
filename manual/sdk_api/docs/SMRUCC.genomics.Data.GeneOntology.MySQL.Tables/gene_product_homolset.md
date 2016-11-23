# gene_product_homolset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_homolset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_homolset` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `gene_product_id` int(11) NOT NULL,
 `homolset_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `gene_product_id` (`gene_product_id`),
 KEY `homolset_id` (`homolset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homolset.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_homolset` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homolset.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homolset.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homolset.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_homolset` SET `id`='{0}', `gene_product_id`='{1}', `homolset_id`='{2}' WHERE `id` = '{3}';
 ```


