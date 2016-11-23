# gene_product_homology
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_homology`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_homology` (
 `gene_product1_id` int(11) NOT NULL,
 `gene_product2_id` int(11) NOT NULL,
 `relationship_type_id` int(11) NOT NULL,
 KEY `gene_product1_id` (`gene_product1_id`),
 KEY `gene_product2_id` (`gene_product2_id`),
 KEY `relationship_type_id` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homology.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_homology` WHERE `gene_product1_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homology.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homology.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_homology.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_homology` SET `gene_product1_id`='{0}', `gene_product2_id`='{1}', `relationship_type_id`='{2}' WHERE `gene_product1_id` = '{3}';
 ```


