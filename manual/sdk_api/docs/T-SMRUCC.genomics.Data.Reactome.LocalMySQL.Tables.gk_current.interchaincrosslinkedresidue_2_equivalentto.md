---
title: interchaincrosslinkedresidue_2_equivalentto
---

# interchaincrosslinkedresidue_2_equivalentto
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `interchaincrosslinkedresidue_2_equivalentto`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `interchaincrosslinkedresidue_2_equivalentto` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `equivalentTo_rank` int(10) unsigned DEFAULT NULL,
 `equivalentTo` int(10) unsigned DEFAULT NULL,
 `equivalentTo_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `equivalentTo` (`equivalentTo`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




