---
title: term_subset
---

# term_subset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `term_subset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_subset` (
 `term_id` int(11) NOT NULL,
 `subset_id` int(11) NOT NULL,
 KEY `tss1` (`term_id`),
 KEY `tss2` (`subset_id`),
 KEY `tss3` (`term_id`,`subset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




