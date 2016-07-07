---
title: acl
---

# acl
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `acl`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `acl` (
 `requesterId` int(10) unsigned NOT NULL DEFAULT '0',
 `requesterType` enum('user','group') NOT NULL DEFAULT 'user',
 `resourceId` int(10) unsigned NOT NULL DEFAULT '0',
 `resourceType` enum('cart','job','uarray','scaffold','proteomic','interaction','taxonomyId') DEFAULT NULL,
 `read` tinyint(1) NOT NULL DEFAULT '0',
 `write` tinyint(1) NOT NULL DEFAULT '0',
 `admin` tinyint(1) NOT NULL DEFAULT '0',
 KEY `requesterId` (`requesterId`,`resourceId`),
 KEY `resourceId_key` (`resourceId`,`resourceType`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




