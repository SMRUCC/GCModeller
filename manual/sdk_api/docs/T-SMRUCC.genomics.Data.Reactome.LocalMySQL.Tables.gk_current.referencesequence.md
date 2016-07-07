---
title: referencesequence
---

# referencesequence
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referencesequence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referencesequence` (
 `DB_ID` int(10) unsigned NOT NULL,
 `checksum` text,
 `isSequenceChanged` text,
 `sequenceLength` int(10) DEFAULT NULL,
 `species` int(10) unsigned DEFAULT NULL,
 `species_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `sequenceLength` (`sequenceLength`),
 KEY `species` (`species`),
 FULLTEXT KEY `checksum` (`checksum`),
 FULLTEXT KEY `isSequenceChanged` (`isSequenceChanged`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




