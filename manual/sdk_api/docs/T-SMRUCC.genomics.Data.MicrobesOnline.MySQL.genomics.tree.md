---
title: tree
---

# tree
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `tree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tree` (
 `treeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `name` varchar(64) NOT NULL DEFAULT '',
 `type` varchar(30) NOT NULL DEFAULT '',
 `modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 `newick` longblob,
 PRIMARY KEY (`treeId`),
 KEY `name` (`name`),
 KEY `type` (`type`,`name`)
 ) ENGINE=MyISAM AUTO_INCREMENT=5962896 DEFAULT CHARSET=latin1 MAX_ROWS=1000000000 AVG_ROW_LENGTH=10000;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




