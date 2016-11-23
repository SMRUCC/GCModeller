# term_synonym
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_synonym` (
 `term_id` int(11) NOT NULL,
 `term_synonym` varchar(996) DEFAULT NULL,
 `acc_synonym` varchar(255) DEFAULT NULL,
 `synonym_type_id` int(11) NOT NULL,
 `synonym_category_id` int(11) DEFAULT NULL,
 UNIQUE KEY `term_id` (`term_id`,`term_synonym`),
 KEY `synonym_type_id` (`synonym_type_id`),
 KEY `synonym_category_id` (`synonym_category_id`),
 KEY `ts1` (`term_id`),
 KEY `ts2` (`term_synonym`),
 KEY `ts3` (`term_id`,`term_synonym`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 20:47:31
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_synonym.GetDeleteSQL
```
```SQL
 DELETE FROM `term_synonym` WHERE `term_id`='{0}' and `term_synonym`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_synonym.GetInsertSQL
```
```SQL
 INSERT INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_synonym.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_synonym.GetUpdateSQL
```
```SQL
 UPDATE `term_synonym` SET `term_id`='{0}', `term_synonym`='{1}', `acc_synonym`='{2}', `synonym_type_id`='{3}', `synonym_category_id`='{4}' WHERE `term_id`='{5}' and `term_synonym`='{6}';
 ```


