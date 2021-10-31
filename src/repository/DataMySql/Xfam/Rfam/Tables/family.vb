#Region "Microsoft.VisualBasic::3c63352a0c8720d555d2f69d45b33fb8, DataMySql\Xfam\Rfam\Tables\family.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class family
    ' 
    '     Properties: author, auto_wiki, clen, cmbuild, cmcalibrate
    '                 cmsearch, comment, created, description, ecmli_cal_db
    '                 ecmli_cal_hits, ecmli_lambda, ecmli_mu, gathering_cutoff, hmm_lambda
    '                 hmm_tau, match_pair_node, maxl, noise_cutoff, num_full
    '                 num_genome_seq, num_refseq, num_seed, number_3d_structures, number_of_species
    '                 previous_id, rfam_acc, rfam_id, seed_source, structure_source
    '                 tax_seed, trusted_cutoff, type, updated
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `family`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `family` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `rfam_id` varchar(40) NOT NULL,
'''   `auto_wiki` int(10) unsigned NOT NULL,
'''   `description` varchar(75) DEFAULT NULL,
'''   `author` tinytext,
'''   `seed_source` tinytext,
'''   `gathering_cutoff` double(5,2) DEFAULT NULL,
'''   `trusted_cutoff` double(5,2) DEFAULT NULL,
'''   `noise_cutoff` double(5,2) DEFAULT NULL,
'''   `comment` longtext,
'''   `previous_id` tinytext,
'''   `cmbuild` tinytext,
'''   `cmcalibrate` tinytext,
'''   `cmsearch` tinytext,
'''   `num_seed` bigint(20) DEFAULT NULL,
'''   `num_full` bigint(20) DEFAULT NULL,
'''   `num_genome_seq` bigint(20) DEFAULT NULL,
'''   `num_refseq` bigint(20) DEFAULT NULL,
'''   `type` varchar(50) DEFAULT NULL,
'''   `structure_source` tinytext,
'''   `number_of_species` bigint(20) DEFAULT NULL,
'''   `number_3d_structures` int(11) DEFAULT NULL,
'''   `tax_seed` mediumtext,
'''   `ecmli_lambda` double(10,5) DEFAULT NULL,
'''   `ecmli_mu` double(10,5) DEFAULT NULL,
'''   `ecmli_cal_db` mediumint(9) DEFAULT '0',
'''   `ecmli_cal_hits` mediumint(9) DEFAULT '0',
'''   `maxl` mediumint(9) DEFAULT '0',
'''   `clen` mediumint(9) DEFAULT '0',
'''   `match_pair_node` tinyint(1) DEFAULT '0',
'''   `hmm_tau` double(10,5) DEFAULT NULL,
'''   `hmm_lambda` double(10,5) DEFAULT NULL,
'''   `created` datetime NOT NULL,
'''   `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`rfam_acc`),
'''   UNIQUE KEY `rfam_acc` (`rfam_acc`),
'''   KEY `rfam_id` (`rfam_id`),
'''   KEY `fk_family_wikitext1_idx` (`auto_wiki`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("family", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `family` (
  `rfam_acc` varchar(7) NOT NULL,
  `rfam_id` varchar(40) NOT NULL,
  `auto_wiki` int(10) unsigned NOT NULL,
  `description` varchar(75) DEFAULT NULL,
  `author` tinytext,
  `seed_source` tinytext,
  `gathering_cutoff` double(5,2) DEFAULT NULL,
  `trusted_cutoff` double(5,2) DEFAULT NULL,
  `noise_cutoff` double(5,2) DEFAULT NULL,
  `comment` longtext,
  `previous_id` tinytext,
  `cmbuild` tinytext,
  `cmcalibrate` tinytext,
  `cmsearch` tinytext,
  `num_seed` bigint(20) DEFAULT NULL,
  `num_full` bigint(20) DEFAULT NULL,
  `num_genome_seq` bigint(20) DEFAULT NULL,
  `num_refseq` bigint(20) DEFAULT NULL,
  `type` varchar(50) DEFAULT NULL,
  `structure_source` tinytext,
  `number_of_species` bigint(20) DEFAULT NULL,
  `number_3d_structures` int(11) DEFAULT NULL,
  `tax_seed` mediumtext,
  `ecmli_lambda` double(10,5) DEFAULT NULL,
  `ecmli_mu` double(10,5) DEFAULT NULL,
  `ecmli_cal_db` mediumint(9) DEFAULT '0',
  `ecmli_cal_hits` mediumint(9) DEFAULT '0',
  `maxl` mediumint(9) DEFAULT '0',
  `clen` mediumint(9) DEFAULT '0',
  `match_pair_node` tinyint(1) DEFAULT '0',
  `hmm_tau` double(10,5) DEFAULT NULL,
  `hmm_lambda` double(10,5) DEFAULT NULL,
  `created` datetime NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`rfam_acc`),
  UNIQUE KEY `rfam_acc` (`rfam_acc`),
  KEY `rfam_id` (`rfam_id`),
  KEY `fk_family_wikitext1_idx` (`auto_wiki`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class family: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "40"), Column(Name:="rfam_id")> Public Property rfam_id As String
    <DatabaseField("auto_wiki"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="auto_wiki")> Public Property auto_wiki As Long
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "75"), Column(Name:="description")> Public Property description As String
    <DatabaseField("author"), DataType(MySqlDbType.Text), Column(Name:="author")> Public Property author As String
    <DatabaseField("seed_source"), DataType(MySqlDbType.Text), Column(Name:="seed_source")> Public Property seed_source As String
    <DatabaseField("gathering_cutoff"), DataType(MySqlDbType.Double), Column(Name:="gathering_cutoff")> Public Property gathering_cutoff As Double
    <DatabaseField("trusted_cutoff"), DataType(MySqlDbType.Double), Column(Name:="trusted_cutoff")> Public Property trusted_cutoff As Double
    <DatabaseField("noise_cutoff"), DataType(MySqlDbType.Double), Column(Name:="noise_cutoff")> Public Property noise_cutoff As Double
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
    <DatabaseField("previous_id"), DataType(MySqlDbType.Text), Column(Name:="previous_id")> Public Property previous_id As String
    <DatabaseField("cmbuild"), DataType(MySqlDbType.Text), Column(Name:="cmbuild")> Public Property cmbuild As String
    <DatabaseField("cmcalibrate"), DataType(MySqlDbType.Text), Column(Name:="cmcalibrate")> Public Property cmcalibrate As String
    <DatabaseField("cmsearch"), DataType(MySqlDbType.Text), Column(Name:="cmsearch")> Public Property cmsearch As String
    <DatabaseField("num_seed"), DataType(MySqlDbType.Int64, "20"), Column(Name:="num_seed")> Public Property num_seed As Long
    <DatabaseField("num_full"), DataType(MySqlDbType.Int64, "20"), Column(Name:="num_full")> Public Property num_full As Long
    <DatabaseField("num_genome_seq"), DataType(MySqlDbType.Int64, "20"), Column(Name:="num_genome_seq")> Public Property num_genome_seq As Long
    <DatabaseField("num_refseq"), DataType(MySqlDbType.Int64, "20"), Column(Name:="num_refseq")> Public Property num_refseq As Long
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="type")> Public Property type As String
    <DatabaseField("structure_source"), DataType(MySqlDbType.Text), Column(Name:="structure_source")> Public Property structure_source As String
    <DatabaseField("number_of_species"), DataType(MySqlDbType.Int64, "20"), Column(Name:="number_of_species")> Public Property number_of_species As Long
    <DatabaseField("number_3d_structures"), DataType(MySqlDbType.Int64, "11"), Column(Name:="number_3d_structures")> Public Property number_3d_structures As Long
    <DatabaseField("tax_seed"), DataType(MySqlDbType.Text), Column(Name:="tax_seed")> Public Property tax_seed As String
    <DatabaseField("ecmli_lambda"), DataType(MySqlDbType.Double), Column(Name:="ecmli_lambda")> Public Property ecmli_lambda As Double
    <DatabaseField("ecmli_mu"), DataType(MySqlDbType.Double), Column(Name:="ecmli_mu")> Public Property ecmli_mu As Double
    <DatabaseField("ecmli_cal_db"), DataType(MySqlDbType.Int64, "9"), Column(Name:="ecmli_cal_db")> Public Property ecmli_cal_db As Long
    <DatabaseField("ecmli_cal_hits"), DataType(MySqlDbType.Int64, "9"), Column(Name:="ecmli_cal_hits")> Public Property ecmli_cal_hits As Long
    <DatabaseField("maxl"), DataType(MySqlDbType.Int64, "9"), Column(Name:="maxl")> Public Property maxl As Long
    <DatabaseField("clen"), DataType(MySqlDbType.Int64, "9"), Column(Name:="clen")> Public Property clen As Long
    <DatabaseField("match_pair_node"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="match_pair_node")> Public Property match_pair_node As Boolean
    <DatabaseField("hmm_tau"), DataType(MySqlDbType.Double), Column(Name:="hmm_tau")> Public Property hmm_tau As Double
    <DatabaseField("hmm_lambda"), DataType(MySqlDbType.Double), Column(Name:="hmm_lambda")> Public Property hmm_lambda As Double
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created")> Public Property created As Date
    <DatabaseField("updated"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated")> Public Property updated As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `family` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `auto_wiki`='{2}', `description`='{3}', `author`='{4}', `seed_source`='{5}', `gathering_cutoff`='{6}', `trusted_cutoff`='{7}', `noise_cutoff`='{8}', `comment`='{9}', `previous_id`='{10}', `cmbuild`='{11}', `cmcalibrate`='{12}', `cmsearch`='{13}', `num_seed`='{14}', `num_full`='{15}', `num_genome_seq`='{16}', `num_refseq`='{17}', `type`='{18}', `structure_source`='{19}', `number_of_species`='{20}', `number_3d_structures`='{21}', `tax_seed`='{22}', `ecmli_lambda`='{23}', `ecmli_mu`='{24}', `ecmli_cal_db`='{25}', `ecmli_cal_hits`='{26}', `maxl`='{27}', `clen`='{28}', `match_pair_node`='{29}', `hmm_tau`='{30}', `hmm_lambda`='{31}', `created`='{32}', `updated`='{33}' WHERE `rfam_acc` = '{34}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `family` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        Else
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{rfam_id}', '{auto_wiki}', '{description}', '{author}', '{seed_source}', '{gathering_cutoff}', '{trusted_cutoff}', '{noise_cutoff}', '{comment}', '{previous_id}', '{cmbuild}', '{cmcalibrate}', '{cmsearch}', '{num_seed}', '{num_full}', '{num_genome_seq}', '{num_refseq}', '{type}', '{structure_source}', '{number_of_species}', '{number_3d_structures}', '{tax_seed}', '{ecmli_lambda}', '{ecmli_mu}', '{ecmli_cal_db}', '{ecmli_cal_hits}', '{maxl}', '{clen}', '{match_pair_node}', '{hmm_tau}', '{hmm_lambda}', '{created}', '{updated}')"
        Else
            Return $"('{rfam_acc}', '{rfam_id}', '{auto_wiki}', '{description}', '{author}', '{seed_source}', '{gathering_cutoff}', '{trusted_cutoff}', '{noise_cutoff}', '{comment}', '{previous_id}', '{cmbuild}', '{cmcalibrate}', '{cmsearch}', '{num_seed}', '{num_full}', '{num_genome_seq}', '{num_refseq}', '{type}', '{structure_source}', '{number_of_species}', '{number_3d_structures}', '{tax_seed}', '{ecmli_lambda}', '{ecmli_mu}', '{ecmli_cal_db}', '{ecmli_cal_hits}', '{maxl}', '{clen}', '{match_pair_node}', '{hmm_tau}', '{hmm_lambda}', '{created}', '{updated}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `auto_wiki`='{2}', `description`='{3}', `author`='{4}', `seed_source`='{5}', `gathering_cutoff`='{6}', `trusted_cutoff`='{7}', `noise_cutoff`='{8}', `comment`='{9}', `previous_id`='{10}', `cmbuild`='{11}', `cmcalibrate`='{12}', `cmsearch`='{13}', `num_seed`='{14}', `num_full`='{15}', `num_genome_seq`='{16}', `num_refseq`='{17}', `type`='{18}', `structure_source`='{19}', `number_of_species`='{20}', `number_3d_structures`='{21}', `tax_seed`='{22}', `ecmli_lambda`='{23}', `ecmli_mu`='{24}', `ecmli_cal_db`='{25}', `ecmli_cal_hits`='{26}', `maxl`='{27}', `clen`='{28}', `match_pair_node`='{29}', `hmm_tau`='{30}', `hmm_lambda`='{31}', `created`='{32}', `updated`='{33}' WHERE `rfam_acc` = '{34}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated), rfam_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As family
                         Return DirectCast(MyClass.MemberwiseClone, family)
                     End Function
End Class


End Namespace
