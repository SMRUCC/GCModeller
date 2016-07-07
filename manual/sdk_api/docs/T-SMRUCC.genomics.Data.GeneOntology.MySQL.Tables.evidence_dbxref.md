---
title: evidence_dbxref
---

# evidence_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `evidence_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence_dbxref` (
 `evidence_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 KEY `evx1` (`evidence_id`),
 KEY `evx2` (`dbxref_id`),
 KEY `evx3` (`evidence_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




