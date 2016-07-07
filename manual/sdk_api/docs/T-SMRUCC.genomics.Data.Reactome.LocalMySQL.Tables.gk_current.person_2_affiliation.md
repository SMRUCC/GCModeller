---
title: person_2_affiliation
---

# person_2_affiliation
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `person_2_affiliation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `person_2_affiliation` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `affiliation_rank` int(10) unsigned DEFAULT NULL,
 `affiliation` int(10) unsigned DEFAULT NULL,
 `affiliation_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `affiliation` (`affiliation`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




