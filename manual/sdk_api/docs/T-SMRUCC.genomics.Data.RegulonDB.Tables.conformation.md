---
title: conformation
---

# conformation
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `conformation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `conformation` (
 `conformation_id` char(12) NOT NULL,
 `transcription_factor_id` char(12) NOT NULL,
 `final_state` varchar(2000) DEFAULT NULL,
 `conformation_note` varchar(2000) DEFAULT NULL,
 `interaction_type` varchar(250) DEFAULT NULL,
 `conformation_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




