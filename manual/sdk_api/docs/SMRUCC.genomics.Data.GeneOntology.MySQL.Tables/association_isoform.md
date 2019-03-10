# association_isoform
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `association_isoform`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_isoform` (
 `association_id` int(11) NOT NULL,
 `gene_product_id` int(11) NOT NULL,
 KEY `association_id` (`association_id`),
 KEY `gene_product_id` (`gene_product_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_isoform.GetDeleteSQL
```
```SQL
 DELETE FROM `association_isoform` WHERE `association_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_isoform.GetInsertSQL
```
```SQL
 INSERT INTO `association_isoform` (`association_id`, `gene_product_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_isoform.GetReplaceSQL
```
```SQL
 REPLACE INTO `association_isoform` (`association_id`, `gene_product_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_isoform.GetUpdateSQL
```
```SQL
 UPDATE `association_isoform` SET `association_id`='{0}', `gene_product_id`='{1}' WHERE `association_id` = '{2}';
 ```


