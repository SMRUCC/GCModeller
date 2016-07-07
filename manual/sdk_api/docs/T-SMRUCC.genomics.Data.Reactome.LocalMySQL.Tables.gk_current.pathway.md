---
title: pathway
---

# pathway
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `pathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway` (
 `DB_ID` int(10) unsigned NOT NULL,
 `doi` varchar(64) DEFAULT NULL,
 `isCanonical` enum('TRUE','FALSE') DEFAULT NULL,
 `normalPathway` int(10) unsigned DEFAULT NULL,
 `normalPathway_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `doi` (`doi`),
 KEY `isCanonical` (`isCanonical`),
 KEY `normalPathway` (`normalPathway`),
 FULLTEXT KEY `doi_fulltext` (`doi`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




