# term_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_dbxref` (
 `term_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 `is_for_definition` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `term_id` (`term_id`,`dbxref_id`,`is_for_definition`),
 KEY `tx0` (`term_id`),
 KEY `tx1` (`dbxref_id`),
 KEY `tx2` (`term_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_dbxref.GetDeleteSQL
```
```SQL
 DELETE FROM `term_dbxref` WHERE `term_id`='{0}' and `dbxref_id`='{1}' and `is_for_definition`='{2}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_dbxref.GetInsertSQL
```
```SQL
 INSERT INTO `term_dbxref` (`term_id`, `dbxref_id`, `is_for_definition`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_dbxref.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_dbxref` (`term_id`, `dbxref_id`, `is_for_definition`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_dbxref.GetUpdateSQL
```
```SQL
 UPDATE `term_dbxref` SET `term_id`='{0}', `dbxref_id`='{1}', `is_for_definition`='{2}' WHERE `term_id`='{3}' and `dbxref_id`='{4}' and `is_for_definition`='{5}';
 ```


