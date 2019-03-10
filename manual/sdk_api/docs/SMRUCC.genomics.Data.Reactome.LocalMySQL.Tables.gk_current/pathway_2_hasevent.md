﻿# pathway_2_hasevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `pathway_2_hasevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway_2_hasevent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasEvent_rank` int(10) unsigned DEFAULT NULL,
 `hasEvent` int(10) unsigned DEFAULT NULL,
 `hasEvent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasEvent` (`hasEvent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




