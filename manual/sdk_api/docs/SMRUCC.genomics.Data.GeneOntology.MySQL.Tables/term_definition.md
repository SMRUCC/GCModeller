# term_definition
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_definition`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_definition` (
 `term_id` int(11) NOT NULL,
 `term_definition` text NOT NULL,
 `dbxref_id` int(11) DEFAULT NULL,
 `term_comment` mediumtext,
 `reference` varchar(255) DEFAULT NULL,
 UNIQUE KEY `term_id` (`term_id`),
 KEY `dbxref_id` (`dbxref_id`),
 KEY `td1` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_definition.GetDeleteSQL
```
```SQL
 DELETE FROM `term_definition` WHERE `term_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_definition.GetInsertSQL
```
```SQL
 INSERT INTO `term_definition` (`term_id`, `term_definition`, `dbxref_id`, `term_comment`, `reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_definition.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_definition` (`term_id`, `term_definition`, `dbxref_id`, `term_comment`, `reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_definition.GetUpdateSQL
```
```SQL
 UPDATE `term_definition` SET `term_id`='{0}', `term_definition`='{1}', `dbxref_id`='{2}', `term_comment`='{3}', `reference`='{4}' WHERE `term_id` = '{5}';
 ```


