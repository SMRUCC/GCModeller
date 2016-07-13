---
title: fragmentmodification
---

# fragmentmodification
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `fragmentmodification`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `fragmentmodification` (
 `DB_ID` int(10) unsigned NOT NULL,
 `endPositionInReferenceSequence` int(10) DEFAULT NULL,
 `startPositionInReferenceSequence` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `endPositionInReferenceSequence` (`endPositionInReferenceSequence`),
 KEY `startPositionInReferenceSequence` (`startPositionInReferenceSequence`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




