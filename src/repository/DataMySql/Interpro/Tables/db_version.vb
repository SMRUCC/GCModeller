#Region "Microsoft.VisualBasic::e9ee2194bcbfa282288bcc6d9fce610b, ..\GCModeller\analysis\annoTools\DataMySql\Interpro\Tables\db_version.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db_version", Database:="interpro")>
Public Class db_version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property version As String
    <DatabaseField("entry_count"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property entry_count As Long
    <DatabaseField("file_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property file_date As Date
    <DatabaseField("load_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property load_date As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `db_version` WHERE `dbcode` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `db_version` SET `dbcode`='{0}', `version`='{1}', `entry_count`='{2}', `file_date`='{3}', `load_date`='{4}' WHERE `dbcode` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, dbcode)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date), dbcode)
    End Function
#End Region
End Class


End Namespace
