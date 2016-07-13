---
title: ObjSynonyms
---

# ObjSynonyms
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_

All regulatory elements except Inductor (Effector) can have name synonyms. Name synonyms are stored 
 in ObjSynonym table.

> 
>  CREATE TABLE `obj_synonyms` (
>    `pkg_guid` int(11) NOT NULL DEFAULT '0',
>    `art_guid` int(11) NOT NULL DEFAULT '0',
>    `obj_guid` int(11) NOT NULL DEFAULT '0',
>    `syn_name` varchar(50) NOT NULL DEFAULT '',
>    `fl_real_name` int(1) DEFAULT NULL,
>    PRIMARY KEY (`obj_guid`,`syn_name`),
>    KEY `pkg_guid` (`pkg_guid`),
>    KEY `art_guid` (`art_guid`),
>    CONSTRAINT `obj_synonyms_ibfk_1` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
>    CONSTRAINT `obj_synonyms_ibfk_2` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `obj_synonyms_ibfk_3` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  



### Properties

#### obj_guid
obj_guid: id of object, for which the synonym is used
#### syn_name
syn_name: name synonym
