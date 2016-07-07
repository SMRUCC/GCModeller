---
title: blackboxevent
---

# blackboxevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `blackboxevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `blackboxevent` (
 `DB_ID` int(10) unsigned NOT NULL,
 `templateEvent` int(10) unsigned DEFAULT NULL,
 `templateEvent_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `templateEvent` (`templateEvent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




