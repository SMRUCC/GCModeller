---
title: users
---

# users
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `users`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `users` (
 `userId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `name` varchar(40) NOT NULL DEFAULT '',
 `org` varchar(40) NOT NULL DEFAULT '',
 `email` varchar(80) NOT NULL DEFAULT '',
 `pwhash` varchar(32) NOT NULL DEFAULT '',
 `annotationTrust` smallint(6) NOT NULL DEFAULT '1',
 `emailDataRelease` int(1) NOT NULL DEFAULT '-1',
 `emailSiteMaintenance` int(1) NOT NULL DEFAULT '-1',
 `emailSoftware` int(1) NOT NULL DEFAULT '-1',
 `isSysAdmin` int(1) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`userId`),
 UNIQUE KEY `email` (`email`)
 ) ENGINE=MyISAM AUTO_INCREMENT=3896 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --
 -- Current Database: `biocyc`
 --
 
 CREATE DATABASE /*!32312 IF NOT EXISTS*/ `biocyc` /*!40100 DEFAULT CHARACTER SET utf8 */;
 
 USE `biocyc`;
 
 --




