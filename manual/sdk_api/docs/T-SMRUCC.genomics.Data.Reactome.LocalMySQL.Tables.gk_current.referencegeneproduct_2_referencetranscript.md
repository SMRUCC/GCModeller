---
title: referencegeneproduct_2_referencetranscript
---

# referencegeneproduct_2_referencetranscript
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referencegeneproduct_2_referencetranscript`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referencegeneproduct_2_referencetranscript` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `referenceTranscript_rank` int(10) unsigned DEFAULT NULL,
 `referenceTranscript` int(10) unsigned DEFAULT NULL,
 `referenceTranscript_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `referenceTranscript` (`referenceTranscript`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




