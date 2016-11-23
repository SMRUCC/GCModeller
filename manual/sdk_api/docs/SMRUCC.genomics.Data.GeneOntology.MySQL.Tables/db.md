# db
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `db`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `db` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `name` varchar(55) DEFAULT NULL,
 `fullname` varchar(255) DEFAULT NULL,
 `datatype` varchar(255) DEFAULT NULL,
 `generic_url` varchar(255) DEFAULT NULL,
 `url_syntax` varchar(255) DEFAULT NULL,
 `url_example` varchar(255) DEFAULT NULL,
 `uri_prefix` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `db0` (`id`),
 UNIQUE KEY `name` (`name`),
 KEY `db1` (`name`),
 KEY `db2` (`fullname`),
 KEY `db3` (`datatype`)
 ) ENGINE=MyISAM AUTO_INCREMENT=262 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.db.GetDeleteSQL
```
```SQL
 DELETE FROM `db` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.db.GetInsertSQL
```
```SQL
 INSERT INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.db.GetReplaceSQL
```
```SQL
 REPLACE INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.db.GetUpdateSQL
```
```SQL
 UPDATE `db` SET `id`='{0}', `name`='{1}', `fullname`='{2}', `datatype`='{3}', `generic_url`='{4}', `url_syntax`='{5}', `url_example`='{6}', `uri_prefix`='{7}' WHERE `id` = '{8}';
 ```


