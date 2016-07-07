---
title: replacedresidue_2_psimod
---

# replacedresidue_2_psimod
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `replacedresidue_2_psimod`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `replacedresidue_2_psimod` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `psiMod_rank` int(10) unsigned DEFAULT NULL,
 `psiMod` int(10) unsigned DEFAULT NULL,
 `psiMod_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `psiMod` (`psiMod`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




