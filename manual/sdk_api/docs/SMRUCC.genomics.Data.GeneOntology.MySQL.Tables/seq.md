# seq
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `seq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seq` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `display_id` varchar(64) DEFAULT NULL,
 `description` varchar(255) DEFAULT NULL,
 `seq` mediumtext,
 `seq_len` int(11) DEFAULT NULL,
 `md5checksum` varchar(32) DEFAULT NULL,
 `moltype` varchar(25) DEFAULT NULL,
 `timestamp` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `seq0` (`id`),
 UNIQUE KEY `display_id` (`display_id`,`md5checksum`),
 KEY `seq1` (`display_id`),
 KEY `seq2` (`md5checksum`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq.GetDeleteSQL
```
```SQL
 DELETE FROM `seq` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq.GetInsertSQL
```
```SQL
 INSERT INTO `seq` (`display_id`, `description`, `seq`, `seq_len`, `md5checksum`, `moltype`, `timestamp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq.GetReplaceSQL
```
```SQL
 REPLACE INTO `seq` (`display_id`, `description`, `seq`, `seq_len`, `md5checksum`, `moltype`, `timestamp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.seq.GetUpdateSQL
```
```SQL
 UPDATE `seq` SET `id`='{0}', `display_id`='{1}', `description`='{2}', `seq`='{3}', `seq_len`='{4}', `md5checksum`='{5}', `moltype`='{6}', `timestamp`='{7}' WHERE `id` = '{8}';
 ```


