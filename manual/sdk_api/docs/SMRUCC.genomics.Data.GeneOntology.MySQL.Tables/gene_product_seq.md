# gene_product_seq
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `gene_product_seq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_seq` (
 `gene_product_id` int(11) NOT NULL,
 `seq_id` int(11) NOT NULL,
 `is_primary_seq` int(11) DEFAULT NULL,
 KEY `gpseq1` (`gene_product_id`),
 KEY `gpseq2` (`seq_id`),
 KEY `gpseq3` (`seq_id`,`gene_product_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_seq.GetDeleteSQL
```
```SQL
 DELETE FROM `gene_product_seq` WHERE `gene_product_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_seq.GetInsertSQL
```
```SQL
 INSERT INTO `gene_product_seq` (`gene_product_id`, `seq_id`, `is_primary_seq`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_seq.GetReplaceSQL
```
```SQL
 REPLACE INTO `gene_product_seq` (`gene_product_id`, `seq_id`, `is_primary_seq`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.gene_product_seq.GetUpdateSQL
```
```SQL
 UPDATE `gene_product_seq` SET `gene_product_id`='{0}', `seq_id`='{1}', `is_primary_seq`='{2}' WHERE `gene_product_id` = '{3}';
 ```


