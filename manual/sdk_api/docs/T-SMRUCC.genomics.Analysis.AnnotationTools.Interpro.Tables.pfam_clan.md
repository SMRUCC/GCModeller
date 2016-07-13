---
title: pfam_clan
---

# pfam_clan
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `pfam_clan`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pfam_clan` (
 `clan_id` varchar(15) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




