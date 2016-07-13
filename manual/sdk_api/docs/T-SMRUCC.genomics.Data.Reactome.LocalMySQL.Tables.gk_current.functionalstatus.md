---
title: functionalstatus
---

# functionalstatus
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `functionalstatus`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `functionalstatus` (
 `DB_ID` int(10) unsigned NOT NULL,
 `functionalStatusType` int(10) unsigned DEFAULT NULL,
 `functionalStatusType_class` varchar(64) DEFAULT NULL,
 `structuralVariant` int(10) unsigned DEFAULT NULL,
 `structuralVariant_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `functionalStatusType` (`functionalStatusType`),
 KEY `structuralVariant` (`structuralVariant`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




