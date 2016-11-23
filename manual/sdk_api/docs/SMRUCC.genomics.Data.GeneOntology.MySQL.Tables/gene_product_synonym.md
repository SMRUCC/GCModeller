# gene_product_synonym
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_synonym` (
 `gene_product_id` int(11) NOT NULL,
 `product_synonym` varchar(255) NOT NULL,
 UNIQUE KEY `gene_product_id` (`gene_product_id`,`product_synonym`),
 KEY `gs1` (`gene_product_id`),
 KEY `gs2` (`product_synonym`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_synonym.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_synonym` WHERE `gene_product_id`='{0}' and `product_synonym`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_synonym.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_synonym` (`gene_product_id`, `product_synonym`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_synonym.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_synonym` (`gene_product_id`, `product_synonym`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_synonym.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_synonym` SET `gene_product_id`='{0}', `product_synonym`='{1}' WHERE `gene_product_id`='{2}' and `product_synonym`='{3}';
 ```


