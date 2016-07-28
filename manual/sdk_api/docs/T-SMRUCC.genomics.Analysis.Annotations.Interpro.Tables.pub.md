---
title: pub
---

# pub
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `pub`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pub` (
 `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pub_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `medline_id` int(9) DEFAULT NULL,
 `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `volume` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `issue` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `firstpage` int(6) DEFAULT NULL,
 `lastpage` int(6) DEFAULT NULL,
 `year` int(4) NOT NULL,
 `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
 `url` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
 `rawpages` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `pubmed_id` bigint(10) DEFAULT NULL,
 PRIMARY KEY (`pub_id`),
 KEY `fk_pub$issn` (`issn`),
 CONSTRAINT `fk_pub$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




