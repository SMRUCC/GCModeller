# term_subset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_subset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_subset` (
 `term_id` int(11) NOT NULL,
 `subset_id` int(11) NOT NULL,
 KEY `tss1` (`term_id`),
 KEY `tss2` (`subset_id`),
 KEY `tss3` (`term_id`,`subset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_subset.GetDeleteSQL
```
```SQL
 DELETE FROM `term_subset` WHERE `term_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_subset.GetInsertSQL
```
```SQL
 INSERT INTO `term_subset` (`term_id`, `subset_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_subset.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_subset` (`term_id`, `subset_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_subset.GetUpdateSQL
```
```SQL
 UPDATE `term_subset` SET `term_id`='{0}', `subset_id`='{1}' WHERE `term_id` = '{2}';
 ```


