# gene_product_ancestor
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_ancestor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_ancestor` (
 `gene_product_id` int(11) NOT NULL,
 `ancestor_id` int(11) NOT NULL,
 `phylotree_id` int(11) NOT NULL,
 `branch_length` float DEFAULT NULL,
 `is_transitive` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `gene_product_id` (`gene_product_id`,`ancestor_id`,`phylotree_id`),
 KEY `ancestor_id` (`ancestor_id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_ancestor.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_ancestor` WHERE `gene_product_id`='{0}' and `ancestor_id`='{1}' and `phylotree_id`='{2}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_ancestor.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_ancestor` (`gene_product_id`, `ancestor_id`, `phylotree_id`, `branch_length`, `is_transitive`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_ancestor.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_ancestor` (`gene_product_id`, `ancestor_id`, `phylotree_id`, `branch_length`, `is_transitive`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_ancestor.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_ancestor` SET `gene_product_id`='{0}', `ancestor_id`='{1}', `phylotree_id`='{2}', `branch_length`='{3}', `is_transitive`='{4}' WHERE `gene_product_id`='{5}' and `ancestor_id`='{6}' and `phylotree_id`='{7}';
 ```


