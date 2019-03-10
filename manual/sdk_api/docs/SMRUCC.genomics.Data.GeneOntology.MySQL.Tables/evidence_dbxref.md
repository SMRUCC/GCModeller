# evidence_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `evidence_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence_dbxref` (
 `evidence_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 KEY `evx1` (`evidence_id`),
 KEY `evx2` (`dbxref_id`),
 KEY `evx3` (`evidence_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence_dbxref.GetDeleteSQL
```
```SQL
 DELETE FROM `evidence_dbxref` WHERE `evidence_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence_dbxref.GetInsertSQL
```
```SQL
 INSERT INTO `evidence_dbxref` (`evidence_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence_dbxref.GetReplaceSQL
```
```SQL
 REPLACE INTO `evidence_dbxref` (`evidence_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence_dbxref.GetUpdateSQL
```
```SQL
 UPDATE `evidence_dbxref` SET `evidence_id`='{0}', `dbxref_id`='{1}' WHERE `evidence_id` = '{2}';
 ```


