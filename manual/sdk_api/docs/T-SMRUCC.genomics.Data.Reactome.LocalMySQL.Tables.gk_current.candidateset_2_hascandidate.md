---
title: candidateset_2_hascandidate
---

# candidateset_2_hascandidate
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `candidateset_2_hascandidate`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `candidateset_2_hascandidate` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasCandidate_rank` int(10) unsigned DEFAULT NULL,
 `hasCandidate` int(10) unsigned DEFAULT NULL,
 `hasCandidate_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasCandidate` (`hasCandidate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




