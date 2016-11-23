# dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dbxref` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `xref_dbname` varchar(55) NOT NULL,
 `xref_key` varchar(255) NOT NULL,
 `xref_keytype` varchar(32) DEFAULT NULL,
 `xref_desc` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `xref_key` (`xref_key`,`xref_dbname`),
 UNIQUE KEY `dx0` (`id`),
 UNIQUE KEY `dx6` (`xref_key`,`xref_dbname`),
 KEY `dx1` (`xref_dbname`),
 KEY `dx2` (`xref_key`),
 KEY `dx3` (`id`,`xref_dbname`),
 KEY `dx4` (`id`,`xref_key`,`xref_dbname`),
 KEY `dx5` (`id`,`xref_key`)
 ) ENGINE=MyISAM AUTO_INCREMENT=85803 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.dbxref.GetDeleteSQL
```
```SQL
 DELETE FROM `dbxref` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.dbxref.GetInsertSQL
```
```SQL
 INSERT INTO `dbxref` (`xref_dbname`, `xref_key`, `xref_keytype`, `xref_desc`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.dbxref.GetReplaceSQL
```
```SQL
 REPLACE INTO `dbxref` (`xref_dbname`, `xref_key`, `xref_keytype`, `xref_desc`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.dbxref.GetUpdateSQL
```
```SQL
 UPDATE `dbxref` SET `id`='{0}', `xref_dbname`='{1}', `xref_key`='{2}', `xref_keytype`='{3}', `xref_desc`='{4}' WHERE `id` = '{5}';
 ```


