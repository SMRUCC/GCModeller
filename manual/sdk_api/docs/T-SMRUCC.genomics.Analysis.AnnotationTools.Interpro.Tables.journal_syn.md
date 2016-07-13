---
title: journal_syn
---

# journal_syn
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `journal_syn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `journal_syn` (
 `issn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `text` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `uppercase` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`issn`,`text`,`code`),
 KEY `fk_journal_syn$code` (`code`),
 CONSTRAINT `fk_journal_syn$code` FOREIGN KEY (`code`) REFERENCES `cv_synonym` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_journal_syn$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




