---
title: jobs
---

# jobs
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `jobs`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `jobs` (
 `jobId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `parentJobId` int(10) unsigned NOT NULL DEFAULT '0',
 `userId` int(10) unsigned NOT NULL DEFAULT '0',
 `cartId` int(10) unsigned NOT NULL DEFAULT '0',
 `jobName` varchar(32) NOT NULL DEFAULT '',
 `jobType` varchar(32) NOT NULL DEFAULT '',
 `jobData` text NOT NULL,
 `jobCmd` text NOT NULL,
 `status` int(2) unsigned NOT NULL DEFAULT '0',
 `time` int(10) unsigned NOT NULL DEFAULT '0',
 `doneTime` int(10) unsigned NOT NULL DEFAULT '0',
 `saved` int(1) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`jobId`),
 KEY `userId` (`userId`),
 KEY `cartId` (`cartId`),
 KEY `parentJobId` (`parentJobId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=12559 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




