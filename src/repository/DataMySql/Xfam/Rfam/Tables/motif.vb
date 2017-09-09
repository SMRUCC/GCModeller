#Region "Microsoft.VisualBasic::50c2090fdbaee4e0949d1c7d97ed4ff7, ..\repository\DataMySql\Xfam\Rfam\Tables\motif.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `motif`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif` (
'''   `motif_acc` varchar(7) NOT NULL,
'''   `motif_id` varchar(40) DEFAULT NULL,
'''   `description` varchar(75) DEFAULT NULL,
'''   `author` tinytext,
'''   `seed_source` tinytext,
'''   `gathering_cutoff` double(5,2) DEFAULT NULL,
'''   `trusted_cutoff` double(5,2) DEFAULT NULL,
'''   `noise_cutoff` double(5,2) DEFAULT NULL,
'''   `cmbuild` tinytext,
'''   `cmcalibrate` tinytext,
'''   `type` varchar(50) DEFAULT NULL,
'''   `num_seed` bigint(20) DEFAULT NULL,
'''   `average_id` double(5,2) DEFAULT NULL,
'''   `average_sqlen` double(7,2) DEFAULT NULL,
'''   `ecmli_lambda` double(10,5) DEFAULT NULL,
'''   `ecmli_mu` double(10,5) DEFAULT NULL,
'''   `ecmli_cal_db` mediumint(9) DEFAULT '0',
'''   `ecmli_cal_hits` mediumint(9) DEFAULT '0',
'''   `maxl` mediumint(9) DEFAULT '0',
'''   `clen` mediumint(9) DEFAULT '0',
'''   `match_pair_node` tinyint(1) DEFAULT '0',
'''   `hmm_tau` double(10,5) DEFAULT NULL,
'''   `hmm_lambda` double(10,5) DEFAULT NULL,
'''   `wiki` varchar(80) DEFAULT NULL,
'''   `created` datetime NOT NULL,
'''   `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`motif_acc`),
'''   KEY `motif_id` (`motif_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif` (
  `motif_acc` varchar(7) NOT NULL,
  `motif_id` varchar(40) DEFAULT NULL,
  `description` varchar(75) DEFAULT NULL,
  `author` tinytext,
  `seed_source` tinytext,
  `gathering_cutoff` double(5,2) DEFAULT NULL,
  `trusted_cutoff` double(5,2) DEFAULT NULL,
  `noise_cutoff` double(5,2) DEFAULT NULL,
  `cmbuild` tinytext,
  `cmcalibrate` tinytext,
  `type` varchar(50) DEFAULT NULL,
  `num_seed` bigint(20) DEFAULT NULL,
  `average_id` double(5,2) DEFAULT NULL,
  `average_sqlen` double(7,2) DEFAULT NULL,
  `ecmli_lambda` double(10,5) DEFAULT NULL,
  `ecmli_mu` double(10,5) DEFAULT NULL,
  `ecmli_cal_db` mediumint(9) DEFAULT '0',
  `ecmli_cal_hits` mediumint(9) DEFAULT '0',
  `maxl` mediumint(9) DEFAULT '0',
  `clen` mediumint(9) DEFAULT '0',
  `match_pair_node` tinyint(1) DEFAULT '0',
  `hmm_tau` double(10,5) DEFAULT NULL,
  `hmm_lambda` double(10,5) DEFAULT NULL,
  `wiki` varchar(80) DEFAULT NULL,
  `created` datetime NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`motif_acc`),
  KEY `motif_id` (`motif_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property motif_acc As String
    <DatabaseField("motif_id"), DataType(MySqlDbType.VarChar, "40")> Public Property motif_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "75")> Public Property description As String
    <DatabaseField("author"), DataType(MySqlDbType.Text)> Public Property author As String
    <DatabaseField("seed_source"), DataType(MySqlDbType.Text)> Public Property seed_source As String
    <DatabaseField("gathering_cutoff"), DataType(MySqlDbType.Double)> Public Property gathering_cutoff As Double
    <DatabaseField("trusted_cutoff"), DataType(MySqlDbType.Double)> Public Property trusted_cutoff As Double
    <DatabaseField("noise_cutoff"), DataType(MySqlDbType.Double)> Public Property noise_cutoff As Double
    <DatabaseField("cmbuild"), DataType(MySqlDbType.Text)> Public Property cmbuild As String
    <DatabaseField("cmcalibrate"), DataType(MySqlDbType.Text)> Public Property cmcalibrate As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "50")> Public Property type As String
    <DatabaseField("num_seed"), DataType(MySqlDbType.Int64, "20")> Public Property num_seed As Long
    <DatabaseField("average_id"), DataType(MySqlDbType.Double)> Public Property average_id As Double
    <DatabaseField("average_sqlen"), DataType(MySqlDbType.Double)> Public Property average_sqlen As Double
    <DatabaseField("ecmli_lambda"), DataType(MySqlDbType.Double)> Public Property ecmli_lambda As Double
    <DatabaseField("ecmli_mu"), DataType(MySqlDbType.Double)> Public Property ecmli_mu As Double
    <DatabaseField("ecmli_cal_db"), DataType(MySqlDbType.Int64, "9")> Public Property ecmli_cal_db As Long
    <DatabaseField("ecmli_cal_hits"), DataType(MySqlDbType.Int64, "9")> Public Property ecmli_cal_hits As Long
    <DatabaseField("maxl"), DataType(MySqlDbType.Int64, "9")> Public Property maxl As Long
    <DatabaseField("clen"), DataType(MySqlDbType.Int64, "9")> Public Property clen As Long
    <DatabaseField("match_pair_node"), DataType(MySqlDbType.Int64, "1")> Public Property match_pair_node As Long
    <DatabaseField("hmm_tau"), DataType(MySqlDbType.Double)> Public Property hmm_tau As Double
    <DatabaseField("hmm_lambda"), DataType(MySqlDbType.Double)> Public Property hmm_lambda As Double
    <DatabaseField("wiki"), DataType(MySqlDbType.VarChar, "80")> Public Property wiki As String
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("updated"), NotNull, DataType(MySqlDbType.DateTime)> Public Property updated As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif` WHERE `motif_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif` SET `motif_acc`='{0}', `motif_id`='{1}', `description`='{2}', `author`='{3}', `seed_source`='{4}', `gathering_cutoff`='{5}', `trusted_cutoff`='{6}', `noise_cutoff`='{7}', `cmbuild`='{8}', `cmcalibrate`='{9}', `type`='{10}', `num_seed`='{11}', `average_id`='{12}', `average_sqlen`='{13}', `ecmli_lambda`='{14}', `ecmli_mu`='{15}', `ecmli_cal_db`='{16}', `ecmli_cal_hits`='{17}', `maxl`='{18}', `clen`='{19}', `match_pair_node`='{20}', `hmm_tau`='{21}', `hmm_lambda`='{22}', `wiki`='{23}', `created`='{24}', `updated`='{25}' WHERE `motif_acc` = '{26}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `motif` WHERE `motif_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, motif_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{motif_acc}', '{motif_id}', '{description}', '{author}', '{seed_source}', '{gathering_cutoff}', '{trusted_cutoff}', '{noise_cutoff}', '{cmbuild}', '{cmcalibrate}', '{type}', '{num_seed}', '{average_id}', '{average_sqlen}', '{ecmli_lambda}', '{ecmli_mu}', '{ecmli_cal_db}', '{ecmli_cal_hits}', '{maxl}', '{clen}', '{match_pair_node}', '{hmm_tau}', '{hmm_lambda}', '{wiki}', '{created}', '{updated}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `motif` SET `motif_acc`='{0}', `motif_id`='{1}', `description`='{2}', `author`='{3}', `seed_source`='{4}', `gathering_cutoff`='{5}', `trusted_cutoff`='{6}', `noise_cutoff`='{7}', `cmbuild`='{8}', `cmcalibrate`='{9}', `type`='{10}', `num_seed`='{11}', `average_id`='{12}', `average_sqlen`='{13}', `ecmli_lambda`='{14}', `ecmli_mu`='{15}', `ecmli_cal_db`='{16}', `ecmli_cal_hits`='{17}', `maxl`='{18}', `clen`='{19}', `match_pair_node`='{20}', `hmm_tau`='{21}', `hmm_lambda`='{22}', `wiki`='{23}', `created`='{24}', `updated`='{25}' WHERE `motif_acc` = '{26}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated), motif_acc)
    End Function
#End Region
End Class


End Namespace

