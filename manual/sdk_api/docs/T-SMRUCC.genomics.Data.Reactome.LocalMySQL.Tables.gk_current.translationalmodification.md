---
title: translationalmodification
---

# translationalmodification
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `translationalmodification`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `translationalmodification` (
 `DB_ID` int(10) unsigned NOT NULL,
 `coordinate` int(10) DEFAULT NULL,
 `psiMod` int(10) unsigned DEFAULT NULL,
 `psiMod_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `coordinate` (`coordinate`),
 KEY `psiMod` (`psiMod`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




