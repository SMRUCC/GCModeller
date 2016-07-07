---
title: _instancebeforechange
---

# _instancebeforechange
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_instancebeforechange`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_instancebeforechange` (
 `DB_ID` int(10) unsigned NOT NULL,
 `changedInstanceDB_ID` int(10) DEFAULT NULL,
 `instanceEdit` int(10) unsigned DEFAULT NULL,
 `instanceEdit_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `changedInstanceDB_ID` (`changedInstanceDB_ID`),
 KEY `instanceEdit` (`instanceEdit`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




