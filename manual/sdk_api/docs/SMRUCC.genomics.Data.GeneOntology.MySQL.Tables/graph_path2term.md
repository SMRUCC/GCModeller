# graph_path2term
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](./index.md)_

```SQL
 
 --
 
 DROP TABLE IF EXISTS `graph_path2term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `graph_path2term` (
 `graph_path_id` int(11) NOT NULL,
 `term_id` int(11) NOT NULL,
 `rank` int(11) NOT NULL,
 KEY `graph_path_id` (`graph_path_id`),
 KEY `term_id` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 
 ```



### Methods

#### GetDeleteSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.graph_path2term.GetDeleteSQL
```
```SQL
 DELETE FROM `graph_path2term` WHERE `graph_path_id` = '{0}';
 ```

#### GetInsertSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.graph_path2term.GetInsertSQL
```
```SQL
 INSERT INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetReplaceSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.graph_path2term.GetReplaceSQL
```
```SQL
 REPLACE INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');
 ```

#### GetUpdateSQL
```csharp
SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.graph_path2term.GetUpdateSQL
```
```SQL
 UPDATE `graph_path2term` SET `graph_path_id`='{0}', `term_id`='{1}', `rank`='{2}' WHERE `graph_path_id` = '{3}';
 ```


