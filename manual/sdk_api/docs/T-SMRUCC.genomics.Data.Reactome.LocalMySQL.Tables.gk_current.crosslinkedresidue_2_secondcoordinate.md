---
title: crosslinkedresidue_2_secondcoordinate
---

# crosslinkedresidue_2_secondcoordinate
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `crosslinkedresidue_2_secondcoordinate`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `crosslinkedresidue_2_secondcoordinate` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `secondCoordinate_rank` int(10) unsigned DEFAULT NULL,
 `secondCoordinate` int(10) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `secondCoordinate` (`secondCoordinate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




