# association_qualifier
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `association_qualifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_qualifier` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL,
 `term_id` int(11) NOT NULL,
 `value` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 KEY `term_id` (`term_id`),
 KEY `aq1` (`association_id`,`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_qualifier.GetDeleteSQL
```
```SQL
 DELETE FROM `association_qualifier` WHERE `id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_qualifier.GetInsertSQL
```
```SQL
 INSERT INTO `association_qualifier` (`association_id`, `term_id`, `value`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_qualifier.GetReplaceSQL
```
```SQL
 REPLACE INTO `association_qualifier` (`association_id`, `term_id`, `value`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.association_qualifier.GetUpdateSQL
```
```SQL
 UPDATE `association_qualifier` SET `id`='{0}', `association_id`='{1}', `term_id`='{2}', `value`='{3}' WHERE `id` = '{4}';
 ```


