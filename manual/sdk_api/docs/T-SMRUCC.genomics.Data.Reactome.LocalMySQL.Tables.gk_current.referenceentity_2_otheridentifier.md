---
title: referenceentity_2_otheridentifier
---

# referenceentity_2_otheridentifier
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referenceentity_2_otheridentifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referenceentity_2_otheridentifier` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `otherIdentifier_rank` int(10) unsigned DEFAULT NULL,
 `otherIdentifier` text,
 KEY `DB_ID` (`DB_ID`),
 FULLTEXT KEY `otherIdentifier` (`otherIdentifier`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




