#Region "Microsoft.VisualBasic::4315b63f88631f0f5928187a0bc5533b, DataMySql\Xfam\Rfam\Tables\motif.vb"

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

    ' Class motif
    ' 
    '     Properties: author, average_id, average_sqlen, clen, cmbuild
    '                 cmcalibrate, created, description, ecmli_cal_db, ecmli_cal_hits
    '                 ecmli_lambda, ecmli_mu, gathering_cutoff, hmm_lambda, hmm_tau
    '                 match_pair_node, maxl, motif_acc, motif_id, noise_cutoff
    '                 num_seed, seed_source, trusted_cutoff, type, updated
    '                 wiki
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc"), XmlAttribute> Public Property motif_acc As String
    <DatabaseField("motif_id"), DataType(MySqlDbType.VarChar, "40"), Column(Name:="motif_id")> Public Property motif_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "75"), Column(Name:="description")> Public Property description As String
    <DatabaseField("author"), DataType(MySqlDbType.Text), Column(Name:="author")> Public Property author As String
    <DatabaseField("seed_source"), DataType(MySqlDbType.Text), Column(Name:="seed_source")> Public Property seed_source As String
    <DatabaseField("gathering_cutoff"), DataType(MySqlDbType.Double), Column(Name:="gathering_cutoff")> Public Property gathering_cutoff As Double
    <DatabaseField("trusted_cutoff"), DataType(MySqlDbType.Double), Column(Name:="trusted_cutoff")> Public Property trusted_cutoff As Double
    <DatabaseField("noise_cutoff"), DataType(MySqlDbType.Double), Column(Name:="noise_cutoff")> Public Property noise_cutoff As Double
    <DatabaseField("cmbuild"), DataType(MySqlDbType.Text), Column(Name:="cmbuild")> Public Property cmbuild As String
    <DatabaseField("cmcalibrate"), DataType(MySqlDbType.Text), Column(Name:="cmcalibrate")> Public Property cmcalibrate As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="type")> Public Property type As String
    <DatabaseField("num_seed"), DataType(MySqlDbType.Int64, "20"), Column(Name:="num_seed")> Public Property num_seed As Long
    <DatabaseField("average_id"), DataType(MySqlDbType.Double), Column(Name:="average_id")> Public Property average_id As Double
    <DatabaseField("average_sqlen"), DataType(MySqlDbType.Double), Column(Name:="average_sqlen")> Public Property average_sqlen As Double
    <DatabaseField("ecmli_lambda"), DataType(MySqlDbType.Double), Column(Name:="ecmli_lambda")> Public Property ecmli_lambda As Double
    <DatabaseField("ecmli_mu"), DataType(MySqlDbType.Double), Column(Name:="ecmli_mu")> Public Property ecmli_mu As Double
    <DatabaseField("ecmli_cal_db"), DataType(MySqlDbType.Int64, "9"), Column(Name:="ecmli_cal_db")> Public Property ecmli_cal_db As Long
    <DatabaseField("ecmli_cal_hits"), DataType(MySqlDbType.Int64, "9"), Column(Name:="ecmli_cal_hits")> Public Property ecmli_cal_hits As Long
    <DatabaseField("maxl"), DataType(MySqlDbType.Int64, "9"), Column(Name:="maxl")> Public Property maxl As Long
    <DatabaseField("clen"), DataType(MySqlDbType.Int64, "9"), Column(Name:="clen")> Public Property clen As Long
    <DatabaseField("match_pair_node"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="match_pair_node")> Public Property match_pair_node As Boolean
    <DatabaseField("hmm_tau"), DataType(MySqlDbType.Double), Column(Name:="hmm_tau")> Public Property hmm_tau As Double
    <DatabaseField("hmm_lambda"), DataType(MySqlDbType.Double), Column(Name:="hmm_lambda")> Public Property hmm_lambda As Double
    <DatabaseField("wiki"), DataType(MySqlDbType.VarChar, "80"), Column(Name:="wiki")> Public Property wiki As String
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created")> Public Property created As Date
    <DatabaseField("updated"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated")> Public Property updated As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif` WHERE `motif_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif` SET `motif_acc`='{0}', `motif_id`='{1}', `description`='{2}', `author`='{3}', `seed_source`='{4}', `gathering_cutoff`='{5}', `trusted_cutoff`='{6}', `noise_cutoff`='{7}', `cmbuild`='{8}', `cmcalibrate`='{9}', `type`='{10}', `num_seed`='{11}', `average_id`='{12}', `average_sqlen`='{13}', `ecmli_lambda`='{14}', `ecmli_mu`='{15}', `ecmli_cal_db`='{16}', `ecmli_cal_hits`='{17}', `maxl`='{18}', `clen`='{19}', `match_pair_node`='{20}', `hmm_tau`='{21}', `hmm_lambda`='{22}', `wiki`='{23}', `created`='{24}', `updated`='{25}' WHERE `motif_acc` = '{26}';</SQL>

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
        Return String.Format(INSERT_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        Else
        Return String.Format(INSERT_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{motif_acc}', '{motif_id}', '{description}', '{author}', '{seed_source}', '{gathering_cutoff}', '{trusted_cutoff}', '{noise_cutoff}', '{cmbuild}', '{cmcalibrate}', '{type}', '{num_seed}', '{average_id}', '{average_sqlen}', '{ecmli_lambda}', '{ecmli_mu}', '{ecmli_cal_db}', '{ecmli_cal_hits}', '{maxl}', '{clen}', '{match_pair_node}', '{hmm_tau}', '{hmm_lambda}', '{wiki}', '{created}', '{updated}')"
        Else
            Return $"('{motif_acc}', '{motif_id}', '{description}', '{author}', '{seed_source}', '{gathering_cutoff}', '{trusted_cutoff}', '{noise_cutoff}', '{cmbuild}', '{cmcalibrate}', '{type}', '{num_seed}', '{average_id}', '{average_sqlen}', '{ecmli_lambda}', '{ecmli_mu}', '{ecmli_cal_db}', '{ecmli_cal_hits}', '{maxl}', '{clen}', '{match_pair_node}', '{hmm_tau}', '{hmm_lambda}', '{wiki}', '{created}', '{updated}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `motif` (`motif_acc`, `motif_id`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `cmbuild`, `cmcalibrate`, `type`, `num_seed`, `average_id`, `average_sqlen`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `wiki`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        Else
        Return String.Format(REPLACE_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `motif` SET `motif_acc`='{0}', `motif_id`='{1}', `description`='{2}', `author`='{3}', `seed_source`='{4}', `gathering_cutoff`='{5}', `trusted_cutoff`='{6}', `noise_cutoff`='{7}', `cmbuild`='{8}', `cmcalibrate`='{9}', `type`='{10}', `num_seed`='{11}', `average_id`='{12}', `average_sqlen`='{13}', `ecmli_lambda`='{14}', `ecmli_mu`='{15}', `ecmli_cal_db`='{16}', `ecmli_cal_hits`='{17}', `maxl`='{18}', `clen`='{19}', `match_pair_node`='{20}', `hmm_tau`='{21}', `hmm_lambda`='{22}', `wiki`='{23}', `created`='{24}', `updated`='{25}' WHERE `motif_acc` = '{26}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, motif_id, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, cmbuild, cmcalibrate, type, num_seed, average_id, average_sqlen, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, wiki, MySqlScript.ToMySqlDateTimeString(created), MySqlScript.ToMySqlDateTimeString(updated), motif_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif
                         Return DirectCast(MyClass.MemberwiseClone, motif)
                     End Function
End Class


End Namespace
