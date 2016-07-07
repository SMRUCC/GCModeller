---
title: entitywithaccessionedsequence_2_hasmodifiedresidue
---

# entitywithaccessionedsequence_2_hasmodifiedresidue
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `entitywithaccessionedsequence_2_hasmodifiedresidue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entitywithaccessionedsequence_2_hasmodifiedresidue` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasModifiedResidue_rank` int(10) unsigned DEFAULT NULL,
 `hasModifiedResidue` int(10) unsigned DEFAULT NULL,
 `hasModifiedResidue_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasModifiedResidue` (`hasModifiedResidue`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




