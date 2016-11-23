# gene_product_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_dbxref` (
 `gene_product_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 UNIQUE KEY `gpx3` (`gene_product_id`,`dbxref_id`),
 KEY `gpx1` (`gene_product_id`),
 KEY `gpx2` (`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_dbxref.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_dbxref` WHERE `gene_product_id`='{0}' and `dbxref_id`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_dbxref.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_dbxref` (`gene_product_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_dbxref.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_dbxref` (`gene_product_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_dbxref.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_dbxref` SET `gene_product_id`='{0}', `dbxref_id`='{1}' WHERE `gene_product_id`='{2}' and `dbxref_id`='{3}';
 ```


