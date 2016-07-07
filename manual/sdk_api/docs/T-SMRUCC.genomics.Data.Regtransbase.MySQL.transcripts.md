---
title: transcripts
---

# transcripts
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `transcripts`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcripts` (
 `transcript_guid` int(11) NOT NULL DEFAULT '0',
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(50) DEFAULT NULL,
 `fl_real_name` int(1) DEFAULT NULL,
 `genome_guid` int(11) DEFAULT NULL,
 `pos_from` int(11) DEFAULT NULL,
 `pos_from_guid` int(11) DEFAULT NULL,
 `pfo_type_id` int(11) DEFAULT NULL,
 `pfo_side_guid` int(11) DEFAULT NULL,
 `pos_to` int(11) DEFAULT NULL,
 `pos_to_guid` int(11) DEFAULT NULL,
 `pto_type_id` int(11) DEFAULT NULL,
 `pto_side_guid` int(11) DEFAULT NULL,
 `tr_len` int(11) DEFAULT NULL,
 `descript` blob,
 `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`transcript_guid`),
 KEY `FK_transcripts-pkg_guid` (`pkg_guid`),
 KEY `FK_transcripts-art_guid` (`art_guid`),
 KEY `FK_transcripts-genome_guid` (`genome_guid`),
 CONSTRAINT `FK_transcripts-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_transcripts-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
 CONSTRAINT `FK_transcripts-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-10-09 1:50:00




