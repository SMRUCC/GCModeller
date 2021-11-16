#Region "Microsoft.VisualBasic::dbcc2f932037bfc0d59cd8449b207732, DataMySql\Xfam\Rfam\Tables\db_version.vb"

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

    ' Class db_version
    ' 
    '     Properties: embl_release, genome_collection_date, infernal_version, number_families, pdb_date
    '                 refseq_version, rfam_release, rfam_release_date
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
''' DROP TABLE IF EXISTS `db_version`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `db_version` (
'''   `rfam_release` double(4,1) NOT NULL,
'''   `rfam_release_date` datetime NOT NULL,
'''   `number_families` int(10) NOT NULL,
'''   `embl_release` tinytext NOT NULL,
'''   `genome_collection_date` datetime DEFAULT NULL,
'''   `refseq_version` int(11) DEFAULT NULL,
'''   `pdb_date` datetime DEFAULT NULL,
'''   `infernal_version` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`rfam_release`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db_version", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `db_version` (
  `rfam_release` double(4,1) NOT NULL,
  `rfam_release_date` datetime NOT NULL,
  `number_families` int(10) NOT NULL,
  `embl_release` tinytext NOT NULL,
  `genome_collection_date` datetime DEFAULT NULL,
  `refseq_version` int(11) DEFAULT NULL,
  `pdb_date` datetime DEFAULT NULL,
  `infernal_version` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`rfam_release`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class db_version: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_release"), PrimaryKey, NotNull, DataType(MySqlDbType.Double), Column(Name:="rfam_release"), XmlAttribute> Public Property rfam_release As Double
    <DatabaseField("rfam_release_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="rfam_release_date")> Public Property rfam_release_date As Date
    <DatabaseField("number_families"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="number_families")> Public Property number_families As Long
    <DatabaseField("embl_release"), NotNull, DataType(MySqlDbType.Text), Column(Name:="embl_release")> Public Property embl_release As String
    <DatabaseField("genome_collection_date"), DataType(MySqlDbType.DateTime), Column(Name:="genome_collection_date")> Public Property genome_collection_date As Date
    <DatabaseField("refseq_version"), DataType(MySqlDbType.Int64, "11"), Column(Name:="refseq_version")> Public Property refseq_version As Long
    <DatabaseField("pdb_date"), DataType(MySqlDbType.DateTime), Column(Name:="pdb_date")> Public Property pdb_date As Date
    <DatabaseField("infernal_version"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="infernal_version")> Public Property infernal_version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `db_version` WHERE `rfam_release` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `db_version` SET `rfam_release`='{0}', `rfam_release_date`='{1}', `number_families`='{2}', `embl_release`='{3}', `genome_collection_date`='{4}', `refseq_version`='{5}', `pdb_date`='{6}', `infernal_version`='{7}' WHERE `rfam_release` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `db_version` WHERE `rfam_release` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_release)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
        Else
        Return String.Format(INSERT_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_release}', '{rfam_release_date}', '{number_families}', '{embl_release}', '{genome_collection_date}', '{refseq_version}', '{pdb_date}', '{infernal_version}')"
        Else
            Return $"('{rfam_release}', '{rfam_release_date}', '{number_families}', '{embl_release}', '{genome_collection_date}', '{refseq_version}', '{pdb_date}', '{infernal_version}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
        Else
        Return String.Format(REPLACE_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `db_version` SET `rfam_release`='{0}', `rfam_release_date`='{1}', `number_families`='{2}', `embl_release`='{3}', `genome_collection_date`='{4}', `refseq_version`='{5}', `pdb_date`='{6}', `infernal_version`='{7}' WHERE `rfam_release` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_release, MySqlScript.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, MySqlScript.ToMySqlDateTimeString(genome_collection_date), refseq_version, MySqlScript.ToMySqlDateTimeString(pdb_date), infernal_version, rfam_release)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As db_version
                         Return DirectCast(MyClass.MemberwiseClone, db_version)
                     End Function
End Class


End Namespace
