#Region "Microsoft.VisualBasic::fce25e22913855835e27481f8ce16b1a, DataMySql\Interpro\Tables\db_version.vb"

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
    '     Properties: dbcode, entry_count, file_date, load_date, version
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `db_version`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `db_version` (
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry_count` bigint(10) NOT NULL,
'''   `file_date` datetime NOT NULL,
'''   `load_date` datetime NOT NULL,
'''   PRIMARY KEY (`dbcode`),
'''   CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db_version", Database:="interpro", SchemaSQL:="
CREATE TABLE `db_version` (
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_count` bigint(10) NOT NULL,
  `file_date` datetime NOT NULL,
  `load_date` datetime NOT NULL,
  PRIMARY KEY (`dbcode`),
  CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class db_version: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode"), XmlAttribute> Public Property dbcode As String
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="version")> Public Property version As String
    <DatabaseField("entry_count"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="entry_count")> Public Property entry_count As Long
    <DatabaseField("file_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="file_date")> Public Property file_date As Date
    <DatabaseField("load_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="load_date")> Public Property load_date As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `db_version` WHERE `dbcode` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `db_version` SET `dbcode`='{0}', `version`='{1}', `entry_count`='{2}', `file_date`='{3}', `load_date`='{4}' WHERE `dbcode` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `db_version` WHERE `dbcode` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, dbcode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
        Else
        Return String.Format(INSERT_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{dbcode}', '{version}', '{entry_count}', '{file_date}', '{load_date}')"
        Else
            Return $"('{dbcode}', '{version}', '{entry_count}', '{file_date}', '{load_date}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
        Else
        Return String.Format(REPLACE_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `db_version` SET `dbcode`='{0}', `version`='{1}', `entry_count`='{2}', `file_date`='{3}', `load_date`='{4}' WHERE `dbcode` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, dbcode, version, entry_count, MySqlScript.ToMySqlDateTimeString(file_date), MySqlScript.ToMySqlDateTimeString(load_date), dbcode)
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
