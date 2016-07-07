---
title: instanceedit
---

# instanceedit
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `instanceedit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `instanceedit` (
 `DB_ID` int(10) unsigned NOT NULL,
 `_applyToAllEditedInstances` text,
 `dateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 `note` text,
 PRIMARY KEY (`DB_ID`),
 KEY `dateTime` (`dateTime`),
 FULLTEXT KEY `_applyToAllEditedInstances` (`_applyToAllEditedInstances`),
 FULLTEXT KEY `note` (`note`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




