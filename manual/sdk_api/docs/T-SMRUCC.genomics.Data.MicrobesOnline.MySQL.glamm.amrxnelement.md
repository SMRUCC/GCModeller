---
title: amrxnelement
---

# amrxnelement
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `amrxnelement`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `amrxnelement` (
 `id` bigint(20) NOT NULL AUTO_INCREMENT,
 `type` varchar(255) NOT NULL,
 `xrefId` varchar(255) NOT NULL,
 `amRxn_id` bigint(20) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `FK1F5C04406B73B23C` (`amRxn_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=12988 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




