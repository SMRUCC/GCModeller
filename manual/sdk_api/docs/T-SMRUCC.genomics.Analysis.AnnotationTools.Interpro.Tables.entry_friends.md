---
title: entry_friends
---

# entry_friends
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry_friends`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry_friends` (
 `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `s` int(3) NOT NULL,
 `p1` int(7) NOT NULL,
 `p2` int(7) NOT NULL,
 `pb` int(7) NOT NULL,
 `a1` int(5) NOT NULL,
 `a2` int(5) NOT NULL,
 `ab` int(5) NOT NULL,
 PRIMARY KEY (`entry1_ac`,`entry2_ac`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




