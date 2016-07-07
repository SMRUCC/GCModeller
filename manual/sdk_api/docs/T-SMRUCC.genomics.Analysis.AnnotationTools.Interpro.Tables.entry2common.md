---
title: entry2common
---

# entry2common
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry2common`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry2common` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `order_in` int(3) NOT NULL,
 PRIMARY KEY (`entry_ac`,`ann_id`,`order_in`),
 KEY `fk_entry2common$ann_id` (`ann_id`),
 CONSTRAINT `fk_entry2common$ann_id` FOREIGN KEY (`ann_id`) REFERENCES `common_annotation` (`ann_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry2common$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




