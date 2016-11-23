# seq_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `seq_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seq_dbxref` (
 `seq_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 UNIQUE KEY `seq_id` (`seq_id`,`dbxref_id`),
 KEY `seqx0` (`seq_id`),
 KEY `seqx1` (`dbxref_id`),
 KEY `seqx2` (`seq_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq_dbxref.GetDeleteSQL
```
```SQL
 DELETE FROM `seq_dbxref` WHERE `seq_id`='{0}' and `dbxref_id`='{1}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq_dbxref.GetInsertSQL
```
```SQL
 INSERT INTO `seq_dbxref` (`seq_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq_dbxref.GetReplaceSQL
```
```SQL
 REPLACE INTO `seq_dbxref` (`seq_id`, `dbxref_id`) VALUES ('{0}', '{1}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq_dbxref.GetUpdateSQL
```
```SQL
 UPDATE `seq_dbxref` SET `seq_id`='{0}', `dbxref_id`='{1}' WHERE `seq_id`='{2}' and `dbxref_id`='{3}';
 ```


