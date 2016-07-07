---
title: reactionlikeevent
---

# reactionlikeevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent` (
 `DB_ID` int(10) unsigned NOT NULL,
 `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
 `systematicName` text,
 PRIMARY KEY (`DB_ID`),
 KEY `isChimeric` (`isChimeric`),
 FULLTEXT KEY `systematicName` (`systematicName`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




