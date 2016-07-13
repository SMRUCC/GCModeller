---
title: _deleted
---

# _deleted
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_deleted`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_deleted` (
 `DB_ID` int(10) unsigned NOT NULL,
 `curatorComment` text,
 `reason` int(10) unsigned DEFAULT NULL,
 `reason_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `reason` (`reason`),
 FULLTEXT KEY `curatorComment` (`curatorComment`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




