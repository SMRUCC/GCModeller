---
title: instanceedit_2_author
---

# instanceedit_2_author
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `instanceedit_2_author`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `instanceedit_2_author` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `author_rank` int(10) unsigned DEFAULT NULL,
 `author` int(10) unsigned DEFAULT NULL,
 `author_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `author` (`author`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




