-- ----------------------------------------------------------------------
-- MySQL Migration Toolkit
-- SQL Create Script
-- ----------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;

CREATE DATABASE IF NOT EXISTS `interpro`
  CHARACTER SET latin1 COLLATE latin1_swedish_ci;
-- -------------------------------------
-- Tables

DROP TABLE IF EXISTS `interpro`.`abstract`;
CREATE TABLE `interpro`.`abstract` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `abstract` MEDIUMTEXT BINARY NOT NULL,
  `comments` VARCHAR(100) BINARY NULL,
  PRIMARY KEY (`entry_ac`),
  CONSTRAINT `fk_abstract$entry_ac` FOREIGN KEY `fk_abstract$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`author`;
CREATE TABLE `interpro`.`author` (
  `author_id` INT(9) NOT NULL,
  `name` VARCHAR(80) BINARY NOT NULL,
  `uppercase` VARCHAR(80) BINARY NOT NULL,
  PRIMARY KEY (`author_id`),
  UNIQUE INDEX `ui_author$id$name` (`author_id`, `name`(80)),
  UNIQUE INDEX `uq_author$name` (`name`(80))
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`book`;
CREATE TABLE `interpro`.`book` (
  `isbn` VARCHAR(10) BINARY NOT NULL,
  `title` MEDIUMTEXT BINARY NOT NULL,
  `edition` INT(3) NULL,
  `publisher` VARCHAR(200) BINARY NULL,
  `pubplace` VARCHAR(50) BINARY NULL,
  PRIMARY KEY (`isbn`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`book2author`;
CREATE TABLE `interpro`.`book2author` (
  `isbn` VARCHAR(10) BINARY NOT NULL,
  `author_id` INT(9) NOT NULL,
  `order_in` INT(3) NOT NULL,
  PRIMARY KEY (`isbn`, `order_in`, `author_id`),
  INDEX `i_book2author$fk_author_id` (`author_id`),
  UNIQUE INDEX `uq_book2author$1` (`isbn`(10), `order_in`),
  CONSTRAINT `fk_book2author$author_id` FOREIGN KEY `fk_book2author$author_id` (`author_id`)
    REFERENCES `interpro`.`author` (`author_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_book2author$isbn` FOREIGN KEY `fk_book2author$isbn` (`isbn`)
    REFERENCES `interpro`.`book` (`isbn`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`common_annotation`;
CREATE TABLE `interpro`.`common_annotation` (
  `ann_id` VARCHAR(7) BINARY NOT NULL,
  `name` VARCHAR(50) BINARY NULL,
  `text` MEDIUMTEXT BINARY NOT NULL,
  `comments` VARCHAR(100) BINARY NULL,
  PRIMARY KEY (`ann_id`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_database`;
CREATE TABLE `interpro`.`cv_database` (
  `dbcode` CHAR(1) BINARY NOT NULL,
  `dbname` VARCHAR(20) BINARY NOT NULL,
  `dborder` INT(5) NOT NULL,
  `dbshort` VARCHAR(10) BINARY NOT NULL,
  PRIMARY KEY (`dbcode`),
  UNIQUE INDEX `uq_cv_database$database` (`dbname`(20)),
  UNIQUE INDEX `uq_cv_database$dborder` (`dborder`),
  UNIQUE INDEX `uq_cv_database$dbshort` (`dbshort`(10))
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_entry_type`;
CREATE TABLE `interpro`.`cv_entry_type` (
  `code` CHAR(1) BINARY NOT NULL,
  `abbrev` VARCHAR(20) BINARY NOT NULL,
  `description` MEDIUMTEXT BINARY NULL,
  PRIMARY KEY (`code`),
  UNIQUE INDEX `uq_entry_type$abbrev` (`abbrev`(20))
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_evidence`;
CREATE TABLE `interpro`.`cv_evidence` (
  `code` CHAR(3) BINARY NOT NULL,
  `abbrev` VARCHAR(10) BINARY NOT NULL,
  `description` MEDIUMTEXT BINARY NULL,
  PRIMARY KEY (`code`),
  UNIQUE INDEX `uq_evidence$abbrev` (`abbrev`(10))
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_rank`;
CREATE TABLE `interpro`.`cv_rank` (
  `rank` VARCHAR(20) BINARY NULL,
  `seq` BIGINT(15) NOT NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`interpro2go`;
CREATE TABLE `interpro`.`interpro2go` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `go_id` VARCHAR(10) BINARY NOT NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_relation`;
CREATE TABLE `interpro`.`cv_relation` (
  `code` CHAR(2) BINARY NOT NULL,
  `abbrev` VARCHAR(10) BINARY NOT NULL,
  `description` MEDIUMTEXT BINARY NULL,
  `forward` VARCHAR(30) BINARY NOT NULL,
  `reverse` VARCHAR(30) BINARY NOT NULL,
  PRIMARY KEY (`code`),
  UNIQUE INDEX `uq_relation$abbrev` (`abbrev`(10))
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`cv_synonym`;
CREATE TABLE `interpro`.`cv_synonym` (
  `code` CHAR(4) BINARY NOT NULL,
  `description` VARCHAR(80) BINARY NOT NULL,
  PRIMARY KEY (`code`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`db_version`;
CREATE TABLE `interpro`.`db_version` (
  `dbcode` CHAR(1) BINARY NOT NULL,
  `version` VARCHAR(20) BINARY NOT NULL,
  `entry_count` BIGINT(10) NOT NULL,
  `file_date` DATETIME NOT NULL,
  `load_date` DATETIME NOT NULL,
  PRIMARY KEY (`dbcode`),
  CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY `fk_db_version$dbcode` (`dbcode`)
    REFERENCES `interpro`.`cv_database` (`dbcode`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry`;
CREATE TABLE `interpro`.`entry` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `entry_type` CHAR(1) BINARY NOT NULL,
  `name` VARCHAR(100) BINARY NULL,
  `created` DATETIME NOT NULL,
  `short_name` VARCHAR(30) BINARY NULL,
  PRIMARY KEY (`entry_ac`),
  INDEX `i_fk_entry$entry_type` (`entry_type`(1)),
  CONSTRAINT `fk_entry$entry_type` FOREIGN KEY `fk_entry$entry_type` (`entry_type`)
    REFERENCES `interpro`.`cv_entry_type` (`code`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry_accpair`;
CREATE TABLE `interpro`.`entry_accpair` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `secondary_ac` VARCHAR(9) BINARY NOT NULL,
  PRIMARY KEY (`entry_ac`, `secondary_ac`),
  CONSTRAINT `fk_entry_accpair$ac1` FOREIGN KEY `fk_entry_accpair$ac1` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry_deleted`;
CREATE TABLE `interpro`.`entry_deleted` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `remark` VARCHAR(50) BINARY NULL,
  PRIMARY KEY (`entry_ac`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry_friends`;
CREATE TABLE `interpro`.`entry_friends` (
  `entry1_ac` VARCHAR(9) BINARY NOT NULL,
  `entry2_ac` VARCHAR(9) BINARY NOT NULL,
  `s` INT(3) NOT NULL,
  `p1` INT(7) NOT NULL,
  `p2` INT(7) NOT NULL,
  `pb` INT(7) NOT NULL,
  `a1` INT(5) NOT NULL,
  `a2` INT(5) NOT NULL,
  `ab` INT(5) NOT NULL,
  PRIMARY KEY (`entry1_ac`, `entry2_ac`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry_xref`;
CREATE TABLE `interpro`.`entry_xref` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `dbcode` CHAR(1) BINARY NOT NULL,
  `ac` VARCHAR(30) BINARY NOT NULL,
  `name` VARCHAR(70) BINARY NULL,
  PRIMARY KEY (`entry_ac`, `dbcode`, `ac`),
  CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY `fk_entry_xref$dbcode` (`dbcode`)
    REFERENCES `interpro`.`cv_database` (`dbcode`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY `fk_entry_xref$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry2common`;
CREATE TABLE `interpro`.`entry2common` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `ann_id` VARCHAR(7) BINARY NOT NULL,
  `order_in` INT(3) NOT NULL,
  PRIMARY KEY (`entry_ac`, `ann_id`, `order_in`),
  CONSTRAINT `fk_entry2common$ann_id` FOREIGN KEY `fk_entry2common$ann_id` (`ann_id`)
    REFERENCES `interpro`.`common_annotation` (`ann_id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2common$entry_ac` FOREIGN KEY `fk_entry2common$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry2comp`;
CREATE TABLE `interpro`.`entry2comp` (
  `entry1_ac` VARCHAR(9) BINARY NOT NULL,
  `entry2_ac` VARCHAR(9) BINARY NOT NULL,
  `relation` CHAR(2) BINARY NOT NULL,
  PRIMARY KEY (`entry1_ac`, `entry2_ac`),
  CONSTRAINT `fk_entry2comp$relation` FOREIGN KEY `fk_entry2comp$relation` (`relation`)
    REFERENCES `interpro`.`cv_relation` (`code`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2comp$1` FOREIGN KEY `fk_entry2comp$1` (`entry1_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2comp$2` FOREIGN KEY `fk_entry2comp$2` (`entry2_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry2entry`;
CREATE TABLE `interpro`.`entry2entry` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `parent_ac` VARCHAR(9) BINARY NOT NULL,
  `relation` CHAR(2) BINARY NOT NULL,
  PRIMARY KEY (`entry_ac`, `parent_ac`),
  CONSTRAINT `fk_entry2entry$child` FOREIGN KEY `fk_entry2entry$child` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2entry$parent` FOREIGN KEY `fk_entry2entry$parent` (`parent_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2entry$relation` FOREIGN KEY `fk_entry2entry$relation` (`relation`)
    REFERENCES `interpro`.`cv_relation` (`code`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry2method`;
CREATE TABLE `interpro`.`entry2method` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `evidence` CHAR(3) BINARY NOT NULL,
  `ida` CHAR(1) BINARY NULL,
  PRIMARY KEY (`entry_ac`, `method_ac`),
  CONSTRAINT `fk_entry2method$entry` FOREIGN KEY `fk_entry2method$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$evidence` FOREIGN KEY `fk_entry2method$evidence` (`evidence`)
    REFERENCES `interpro`.`cv_evidence` (`code`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$method` FOREIGN KEY `fk_entry2method$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`entry2pub`;
CREATE TABLE `interpro`.`entry2pub` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `order_in` INT(3) NOT NULL,
  `pub_id` VARCHAR(11) BINARY NOT NULL,
  PRIMARY KEY (`entry_ac`, `pub_id`),
  CONSTRAINT `fk_entry2pub$entry` FOREIGN KEY `fk_entry2pub$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2pub$pub` FOREIGN KEY `fk_entry2pub$pub` (`pub_id`)
    REFERENCES `interpro`.`pub` (`pub_id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`etaxi`;
CREATE TABLE `interpro`.`etaxi` (
  `tax_id` BIGINT(15) NOT NULL,
  `parent_id` BIGINT(15) NULL,
  `scientific_name` VARCHAR(100) BINARY NOT NULL,
  `complete_genome_flag` CHAR(1) BINARY NOT NULL,
  `rank` VARCHAR(20) BINARY NULL,
  `hidden` INT(3) NOT NULL,
  `left_number` BIGINT(15) NULL,
  `right_number` BIGINT(15) NULL,
  `annotation_source` VARCHAR(30) BINARY NOT NULL,
  `full_name` MEDIUMTEXT BINARY NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`example`;
CREATE TABLE `interpro`.`example` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  PRIMARY KEY (`entry_ac`, `protein_ac`),
  CONSTRAINT `fk_example$entry_ac` FOREIGN KEY `fk_example$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_example$protein_ac` FOREIGN KEY `fk_example$protein_ac` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`example_auto`;
CREATE TABLE `interpro`.`example_auto` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `protein_ac` VARCHAR(6) BINARY NOT NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`intact_data`;
CREATE TABLE `interpro`.`intact_data` (
  `uniprot_id` VARCHAR(20) BINARY NOT NULL,
  `protein_ac` VARCHAR(30) BINARY NOT NULL,
  `undetermined` CHAR(1) BINARY NULL,
  `intact_id` VARCHAR(30) BINARY NOT NULL,
  `interacts_with` VARCHAR(30) BINARY NOT NULL,
  `type` VARCHAR(20) BINARY NULL,
  `entry_ac` VARCHAR(30) BINARY NOT NULL,
  `pubmed_id` VARCHAR(30) BINARY NULL,
  PRIMARY KEY (`entry_ac`, `intact_id`, `interacts_with`, `protein_ac`),
  CONSTRAINT `fk_intact_data$entry_ac` FOREIGN KEY `fk_intact_data$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`journal`;
CREATE TABLE `interpro`.`journal` (
  `issn` VARCHAR(9) BINARY NOT NULL,
  `abbrev` VARCHAR(60) BINARY NOT NULL,
  `uppercase` VARCHAR(60) BINARY NULL,
  `start_date` DATETIME NULL,
  `end_date` DATETIME NULL,
  PRIMARY KEY (`issn`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`journal_syn`;
CREATE TABLE `interpro`.`journal_syn` (
  `issn` VARCHAR(10) BINARY NOT NULL,
  `code` CHAR(4) BINARY NOT NULL,
  `text` VARCHAR(200) BINARY NOT NULL,
  `uppercase` VARCHAR(200) BINARY NULL,
  PRIMARY KEY (`issn`, `text`, `code`),
  CONSTRAINT `fk_journal_syn$code` FOREIGN KEY `fk_journal_syn$code` (`code`)
    REFERENCES `interpro`.`cv_synonym` (`code`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_journal_syn$issn` FOREIGN KEY `fk_journal_syn$issn` (`issn`)
    REFERENCES `interpro`.`journal` (`issn`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`match_struct`;
CREATE TABLE `interpro`.`match_struct` (
  `protein_ac` VARCHAR(10) BINARY NOT NULL,
  `domain_id` VARCHAR(14) BINARY NOT NULL,
  `pos_from` INT(5) NOT NULL,
  `pos_to` INT(5) NULL,
  `dbcode` VARCHAR(1) BINARY NULL,
  PRIMARY KEY (`protein_ac`, `domain_id`, `pos_from`),
  CONSTRAINT `fk_match_struct` FOREIGN KEY `fk_match_struct` (`domain_id`)
    REFERENCES `interpro`.`struct_class` (`domain_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`method`;
CREATE TABLE `interpro`.`method` (
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `name` VARCHAR(30) BINARY NOT NULL,
  `dbcode` CHAR(1) BINARY NOT NULL,
  `method_date` DATETIME NOT NULL,
  `skip_flag` CHAR(1) BINARY NOT NULL DEFAULT 'N',
  `candidate` CHAR(1) BINARY NULL,
  PRIMARY KEY (`method_ac`),
  CONSTRAINT `fk_method$dbcode` FOREIGN KEY `fk_method$dbcode` (`dbcode`)
    REFERENCES `interpro`.`cv_database` (`dbcode`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`matches`;
CREATE TABLE `interpro`.`matches` (
  `protein_ac` VARCHAR(12) BINARY NOT NULL,
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `pos_from` INT(5) NULL,
  `pos_to` INT(5) NULL,
  `status` CHAR(1) BINARY NOT NULL,
  `evidence` CHAR(3) BINARY NULL,
  `match_date` DATETIME NOT NULL,
  `seq_date` DATETIME NOT NULL,
  `score` DOUBLE NULL,
  CONSTRAINT `fk_matches$evidence` FOREIGN KEY `fk_matches$evidence` (`evidence`)
    REFERENCES `interpro`.`cv_evidence` (`code`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_matches$method` FOREIGN KEY `fk_matches$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;


DROP TABLE IF EXISTS `interpro`.`method2pub`;
CREATE TABLE `interpro`.`method2pub` (
  `pub_id` VARCHAR(11) BINARY NOT NULL,
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  PRIMARY KEY (`method_ac`, `pub_id`),
  CONSTRAINT `fk_method2pub$method` FOREIGN KEY `fk_method2pub$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_method2pub$pub_id` FOREIGN KEY `fk_method2pub$pub_id` (`pub_id`)
    REFERENCES `interpro`.`pub` (`pub_id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_entry_match`;
CREATE TABLE `interpro`.`mv_entry_match` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `protein_count` INT(7) NOT NULL,
  `match_count` INT(7) NOT NULL,
  PRIMARY KEY (`entry_ac`),
  CONSTRAINT `fk_mv_entry_match$entry` FOREIGN KEY `fk_mv_entry_match$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_entry2protein`;
CREATE TABLE `interpro`.`mv_entry2protein` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `match_count` INT(7) NOT NULL,
  PRIMARY KEY (`entry_ac`, `protein_ac`),
  CONSTRAINT `fk_mv_entry2protein$entry` FOREIGN KEY `fk_mv_entry2protein$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_entry2protein$protein` FOREIGN KEY `fk_mv_entry2protein$protein` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_entry2protein_true`;
CREATE TABLE `interpro`.`mv_entry2protein_true` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `match_count` INT(7) NOT NULL,
  PRIMARY KEY (`entry_ac`, `protein_ac`),
  CONSTRAINT `fk_mv_entry2protein_true$e` FOREIGN KEY `fk_mv_entry2protein_true$e` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_entry2protein_true$p` FOREIGN KEY `fk_mv_entry2protein_true$p` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_method_match`;
CREATE TABLE `interpro`.`mv_method_match` (
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `protein_count` INT(7) NOT NULL,
  `match_count` INT(7) NOT NULL,
  PRIMARY KEY (`method_ac`),
  CONSTRAINT `fk_mv_method_match$method` FOREIGN KEY `fk_mv_method_match$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_method2protein`;
CREATE TABLE `interpro`.`mv_method2protein` (
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `match_count` INT(7) NOT NULL,
  PRIMARY KEY (`method_ac`, `protein_ac`),
  CONSTRAINT `fk_mv_method2protein$protein` FOREIGN KEY `fk_mv_method2protein$protein` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_method2prot$method` FOREIGN KEY `fk_mv_method2prot$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_proteome_count`;
CREATE TABLE `interpro`.`mv_proteome_count` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `name` VARCHAR(100) BINARY NOT NULL,
  `oscode` VARCHAR(5) BINARY NOT NULL,
  `protein_count` INT(7) NOT NULL,
  `method_count` INT(7) NOT NULL,
  PRIMARY KEY (`entry_ac`, `oscode`),
  CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY `fk_mv_proteome_count$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY `fk_mv_proteome_count$oscode` (`oscode`)
    REFERENCES `interpro`.`organism` (`oscode`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_secondary`;
CREATE TABLE `interpro`.`mv_secondary` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `secondary_ac` VARCHAR(9) BINARY NOT NULL,
  `method_ac` VARCHAR(25) BINARY NOT NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`mv_tax_entry_count`;
CREATE TABLE `interpro`.`mv_tax_entry_count` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `tax_id` DECIMAL(22, 0) NOT NULL,
  `count` DECIMAL(22, 0) NOT NULL,
  PRIMARY KEY (`entry_ac`, `tax_id`, `count`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`organism`;
CREATE TABLE `interpro`.`organism` (
  `oscode` VARCHAR(5) BINARY NOT NULL,
  `name` VARCHAR(100) BINARY NOT NULL,
  `italics_name` VARCHAR(100) BINARY NOT NULL,
  `full_name` VARCHAR(100) BINARY NULL,
  `tax_code` DECIMAL(38, 0) NULL,
  PRIMARY KEY (`oscode`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`pdb_pub_additional`;
CREATE TABLE `interpro`.`pdb_pub_additional` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `pub_id` VARCHAR(11) BINARY NOT NULL,
  CONSTRAINT `fk_pdbpubadd$entry_ac` FOREIGN KEY `fk_pdbpubadd$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_pdbpubadd$pub_id` FOREIGN KEY `fk_pdbpubadd$pub_id` (`pub_id`)
    REFERENCES `interpro`.`pub` (`pub_id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`pfam_clan`;
CREATE TABLE `interpro`.`pfam_clan` (
  `clan_id` VARCHAR(15) BINARY NULL,
  `method_ac` VARCHAR(25) BINARY NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`pfam_clan_data`;
CREATE TABLE `interpro`.`pfam_clan_data` (
  `clan_id` VARCHAR(15) BINARY NOT NULL,
  `name` VARCHAR(50) BINARY NOT NULL,
  `description` VARCHAR(75) BINARY NULL,
  PRIMARY KEY (`clan_id`, `name`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`protein`;
CREATE TABLE `interpro`.`protein` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `name` VARCHAR(12) BINARY NOT NULL,
  `dbcode` CHAR(1) BINARY NOT NULL,
  `crc64` CHAR(16) BINARY NOT NULL,
  `len` INT(5) NOT NULL,
  `fragment` CHAR(1) BINARY NOT NULL,
  `struct_flag` CHAR(1) BINARY NOT NULL,
  `tax_id` BIGINT(15) NULL,
  PRIMARY KEY (`protein_ac`),
  CONSTRAINT `fk_protein$dbcode` FOREIGN KEY `fk_protein$dbcode` (`dbcode`)
    REFERENCES `interpro`.`cv_database` (`dbcode`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`protein_accpair`;
CREATE TABLE `interpro`.`protein_accpair` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `secondary_ac` VARCHAR(6) BINARY NOT NULL,
  PRIMARY KEY (`protein_ac`, `secondary_ac`),
  CONSTRAINT `fk_accpair$primary` FOREIGN KEY `fk_accpair$primary` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`protein_ida`;
CREATE TABLE `interpro`.`protein_ida` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `ida` MEDIUMTEXT BINARY NULL,
  PRIMARY KEY (`protein_ac`),
  CONSTRAINT `fk_protein_ida_p` FOREIGN KEY `fk_protein_ida_p` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`protein2genome`;
CREATE TABLE `interpro`.`protein2genome` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `oscode` VARCHAR(5) BINARY NOT NULL,
  PRIMARY KEY (`oscode`, `protein_ac`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`proteome_rank`;
CREATE TABLE `interpro`.`proteome_rank` (
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `oscode` VARCHAR(5) BINARY NOT NULL,
  `rank` INT(7) NOT NULL,
  PRIMARY KEY (`entry_ac`, `oscode`),
  CONSTRAINT `fk_proteome_rank$entry` FOREIGN KEY `fk_proteome_rank$entry` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_proteome_rank$oscode` FOREIGN KEY `fk_proteome_rank$oscode` (`oscode`)
    REFERENCES `interpro`.`organism` (`oscode`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`pub`;
CREATE TABLE `interpro`.`pub` (
  `pub_id` VARCHAR(11) BINARY NOT NULL,
  `pub_type` CHAR(1) BINARY NOT NULL,
  `medline_id` INT(9) NULL,
  `issn` VARCHAR(9) BINARY NULL,
  `isbn` VARCHAR(10) BINARY NULL,
  `volume` VARCHAR(5) BINARY NULL,
  `issue` VARCHAR(5) BINARY NULL,
  `firstpage` INT(6) NULL,
  `lastpage` INT(6) NULL,
  `year` INT(4) NOT NULL,
  `title` MEDIUMTEXT BINARY NULL,
  `url` MEDIUMTEXT BINARY NULL,
  `rawpages` VARCHAR(30) BINARY NULL,
  `pubmed_id` BIGINT(10) NULL,
  PRIMARY KEY (`pub_id`),
  CONSTRAINT `fk_pub$issn` FOREIGN KEY `fk_pub$issn` (`issn`)
    REFERENCES `interpro`.`journal` (`issn`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`pub2author`;
CREATE TABLE `interpro`.`pub2author` (
  `pub_id` VARCHAR(11) BINARY NOT NULL,
  `author_id` INT(9) NOT NULL,
  `order_in` INT(3) NOT NULL,
  PRIMARY KEY (`pub_id`, `author_id`, `order_in`),
  CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY `fk_pub2author$author_id` (`author_id`)
    REFERENCES `interpro`.`author` (`author_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY `fk_pub2author$pub_id` (`pub_id`)
    REFERENCES `interpro`.`pub` (`pub_id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`struct_class`;
CREATE TABLE `interpro`.`struct_class` (
  `domain_id` VARCHAR(14) BINARY NOT NULL,
  `fam_id` VARCHAR(20) BINARY NULL,
  `dbcode` VARCHAR(1) BINARY NULL,
  PRIMARY KEY (`domain_id`)
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`supermatch`;
CREATE TABLE `interpro`.`supermatch` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `pos_from` INT(5) NOT NULL,
  `pos_to` INT(5) NOT NULL,
  PRIMARY KEY (`protein_ac`, `entry_ac`, `pos_to`, `pos_from`),
  CONSTRAINT `fkv_supermatch$entry_ac` FOREIGN KEY `fkv_supermatch$entry_ac` (`entry_ac`)
    REFERENCES `interpro`.`entry` (`entry_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fkv_supermatch$protein_ac` FOREIGN KEY `fkv_supermatch$protein_ac` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`tax_entry_count`;
CREATE TABLE `interpro`.`tax_entry_count` (
  `entry_ac` VARCHAR(9) BINARY NULL,
  `tax_name` VARCHAR(30) BINARY NULL,
  `count` INT(5) NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`tax_name_to_id`;
CREATE TABLE `interpro`.`tax_name_to_id` (
  `tax_name` VARCHAR(30) BINARY NULL,
  `tax_id` BIGINT(15) NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`taxonomy2protein`;
CREATE TABLE `interpro`.`taxonomy2protein` (
  `protein_ac` VARCHAR(6) BINARY NOT NULL,
  `tax_id` BIGINT(15) NOT NULL,
  `hierarchy` VARCHAR(200) BINARY NULL,
  `tax_name_concat` VARCHAR(80) BINARY NULL,
  PRIMARY KEY (`protein_ac`, `tax_id`),
  CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY `fk_taxonomy2protein$p` (`protein_ac`)
    REFERENCES `interpro`.`protein` (`protein_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`text_index_entry`;
CREATE TABLE `interpro`.`text_index_entry` (
  `id` VARCHAR(9) BINARY NULL,
  `field` VARCHAR(20) BINARY NULL,
  `text` VARCHAR(100) BINARY NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`uniprot_taxonomy`;
CREATE TABLE `interpro`.`uniprot_taxonomy` (
  `protein_ac` VARCHAR(10) BINARY NOT NULL,
  `tax_id` BIGINT(15) NOT NULL,
  `left_number` BIGINT(15) NOT NULL,
  `right_number` BIGINT(15) NOT NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`varsplic_master`;
CREATE TABLE `interpro`.`varsplic_master` (
  `protein_ac` VARCHAR(6) BINARY NULL,
  `variant` INT(3) NULL,
  `crc64` VARCHAR(16) BINARY NULL,
  `length` INT(5) NULL
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`varsplic_match`;
CREATE TABLE `interpro`.`varsplic_match` (
  `protein_ac` VARCHAR(12) BINARY NOT NULL,
  `method_ac` VARCHAR(25) BINARY NOT NULL,
  `pos_from` INT(5) NULL,
  `pos_to` INT(5) NULL,
  `status` CHAR(1) BINARY NOT NULL,
  `dbcode` CHAR(1) BINARY NOT NULL,
  `evidence` CHAR(3) BINARY NULL,
  `seq_date` DATETIME NOT NULL,
  `match_date` DATETIME NOT NULL,
  `score` DOUBLE NULL,
  CONSTRAINT `fk_varsplic_match$dbcode` FOREIGN KEY `fk_varsplic_match$dbcode` (`dbcode`)
    REFERENCES `interpro`.`cv_database` (`dbcode`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$evidence` FOREIGN KEY `fk_varsplic_match$evidence` (`evidence`)
    REFERENCES `interpro`.`cv_evidence` (`code`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$method` FOREIGN KEY `fk_varsplic_match$method` (`method_ac`)
    REFERENCES `interpro`.`method` (`method_ac`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = INNODB;

DROP TABLE IF EXISTS `interpro`.`varsplic_supermatch`;
CREATE TABLE `interpro`.`varsplic_supermatch` (
  `protein_ac` VARCHAR(12) BINARY NOT NULL,
  `entry_ac` VARCHAR(9) BINARY NOT NULL,
  `pos_from` INT(5) NOT NULL,
  `pos_to` INT(5) NOT NULL,
  PRIMARY KEY (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`)
)
ENGINE = INNODB;



SET FOREIGN_KEY_CHECKS = 1;

-- ----------------------------------------------------------------------
-- EOF

