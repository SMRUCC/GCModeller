# term_audit
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `term_audit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_audit` (
 `term_id` int(11) NOT NULL,
 `term_loadtime` int(11) DEFAULT NULL,
 UNIQUE KEY `term_id` (`term_id`),
 KEY `ta1` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_audit.GetDeleteSQL
```
```SQL
 DELETE FROM `term_audit` WHERE `term_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_audit.GetInsertSQL
```
```SQL
 INSERT INTO `term_audit` (`term_id`, `term_loadtime`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_audit.GetReplaceSQL
```
```SQL
 REPLACE INTO `term_audit` (`term_id`, `term_loadtime`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.term_audit.GetUpdateSQL
```
```SQL
 UPDATE `term_audit` SET `term_id`='{0}', `term_loadtime`='{1}' WHERE `term_id` = '{2}';
 ```


