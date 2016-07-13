---
title: pfam_clan_data
---

# pfam_clan_data
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `pfam_clan_data`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pfam_clan_data` (
 `clan_id` varchar(15) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `description` varchar(75) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`clan_id`,`name`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




