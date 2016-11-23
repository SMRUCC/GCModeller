# term2term
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term2term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term2term` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL,
 `term1_id` int(11) NOT NULL,
 `term2_id` int(11) NOT NULL,
 `complete` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`),
 KEY `tt1` (`term1_id`),
 KEY `tt2` (`term2_id`),
 KEY `tt3` (`term1_id`,`term2_id`),
 KEY `tt4` (`relationship_type_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=89342 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term.GetDeleteSQL
```
```SQL
 DELETE FROM `term2term` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term.GetInsertSQL
```
```SQL
 INSERT INTO `term2term` (`relationship_type_id`, `term1_id`, `term2_id`, `complete`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term.GetReplaceSQL
```
```SQL
 REPLACE INTO `term2term` (`relationship_type_id`, `term1_id`, `term2_id`, `complete`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term2term.GetUpdateSQL
```
```SQL
 UPDATE `term2term` SET `id`='{0}', `relationship_type_id`='{1}', `term1_id`='{2}', `term2_id`='{3}', `complete`='{4}' WHERE `id` = '{5}';
 ```


