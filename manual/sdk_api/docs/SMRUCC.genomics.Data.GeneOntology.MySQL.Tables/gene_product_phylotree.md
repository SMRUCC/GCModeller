# gene_product_phylotree
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_phylotree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_phylotree` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `gene_product_id` int(11) NOT NULL,
 `phylotree_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `gene_product_id` (`gene_product_id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_phylotree.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_phylotree` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_phylotree.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_phylotree` (`gene_product_id`, `phylotree_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_phylotree.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_phylotree` (`gene_product_id`, `phylotree_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_phylotree.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_phylotree` SET `id`='{0}', `gene_product_id`='{1}', `phylotree_id`='{2}' WHERE `id` = '{3}';
 ```


