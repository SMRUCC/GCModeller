---
title: attenuator_terminator
---

# attenuator_terminator
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `attenuator_terminator`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `attenuator_terminator` (
 `a_terminator_id` varchar(12) NOT NULL,
 `a_terminator_type` varchar(25) DEFAULT NULL,
 `a_terminator_posleft` decimal(10,0) DEFAULT NULL,
 `a_terminator_posright` decimal(10,0) DEFAULT NULL,
 `a_terminator_energy` decimal(7,2) DEFAULT NULL,
 `a_terminator_sequence` varchar(200) DEFAULT NULL,
 `a_terminator_attenuator_id` varchar(12) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




