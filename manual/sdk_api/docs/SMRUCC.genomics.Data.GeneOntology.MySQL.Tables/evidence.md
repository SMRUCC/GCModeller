# evidence
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `evidence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `code` varchar(8) NOT NULL,
 `association_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 `seq_acc` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `association_id` (`association_id`,`dbxref_id`,`code`),
 UNIQUE KEY `ev0` (`id`),
 UNIQUE KEY `ev5` (`id`,`association_id`),
 UNIQUE KEY `ev6` (`id`,`code`,`association_id`),
 KEY `ev1` (`association_id`),
 KEY `ev2` (`code`),
 KEY `ev3` (`dbxref_id`),
 KEY `ev4` (`association_id`,`code`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence.GetDeleteSQL
```
```SQL
 DELETE FROM `evidence` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence.GetInsertSQL
```
```SQL
 INSERT INTO `evidence` (`code`, `association_id`, `dbxref_id`, `seq_acc`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence.GetReplaceSQL
```
```SQL
 REPLACE INTO `evidence` (`code`, `association_id`, `dbxref_id`, `seq_acc`) VALUES ('{0}', '{1}', '{2}', '{3}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.evidence.GetUpdateSQL
```
```SQL
 UPDATE `evidence` SET `id`='{0}', `code`='{1}', `association_id`='{2}', `dbxref_id`='{3}', `seq_acc`='{4}' WHERE `id` = '{5}';
 ```


