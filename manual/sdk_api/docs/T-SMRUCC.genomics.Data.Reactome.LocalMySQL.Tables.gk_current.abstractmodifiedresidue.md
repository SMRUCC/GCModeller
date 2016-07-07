---
title: abstractmodifiedresidue
---

# abstractmodifiedresidue
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `abstractmodifiedresidue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `abstractmodifiedresidue` (
 `DB_ID` int(10) unsigned NOT NULL,
 `referenceSequence` int(10) unsigned DEFAULT NULL,
 `referenceSequence_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `referenceSequence` (`referenceSequence`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




