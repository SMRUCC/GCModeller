---
title: amrxn
---

# amrxn
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `amrxn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `amrxn` (
 `id` bigint(20) NOT NULL AUTO_INCREMENT,
 `reversible` char(1) NOT NULL,
 `am_id` bigint(20) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `FK3B83B1C37422F70` (`am_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=4087 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




