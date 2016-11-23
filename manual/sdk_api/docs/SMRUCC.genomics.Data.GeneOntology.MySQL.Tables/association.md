# association
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `association`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `term_id` int(11) NOT NULL,
 `gene_product_id` int(11) NOT NULL,
 `is_not` int(11) DEFAULT NULL,
 `role_group` int(11) DEFAULT NULL,
 `assocdate` int(11) DEFAULT NULL,
 `source_db_id` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `a0` (`id`),
 KEY `source_db_id` (`source_db_id`),
 KEY `a1` (`term_id`),
 KEY `a2` (`gene_product_id`),
 KEY `a3` (`term_id`,`gene_product_id`),
 KEY `a4` (`id`,`term_id`,`gene_product_id`),
 KEY `a5` (`id`,`gene_product_id`),
 KEY `a6` (`is_not`,`term_id`,`gene_product_id`),
 KEY `a7` (`assocdate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association.GetDeleteSQL
```
```SQL
 DELETE FROM `association` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association.GetInsertSQL
```
```SQL
 INSERT INTO `association` (`term_id`, `gene_product_id`, `is_not`, `role_group`, `assocdate`, `source_db_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association.GetReplaceSQL
```
```SQL
 REPLACE INTO `association` (`term_id`, `gene_product_id`, `is_not`, `role_group`, `assocdate`, `source_db_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association.GetUpdateSQL
```
```SQL
 UPDATE `association` SET `id`='{0}', `term_id`='{1}', `gene_product_id`='{2}', `is_not`='{3}', `role_group`='{4}', `assocdate`='{5}', `source_db_id`='{6}' WHERE `id` = '{7}';
 ```


