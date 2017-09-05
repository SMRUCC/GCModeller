#Region "Microsoft.VisualBasic::de02d2f933d32851a8b7bd41484ba31b, ..\repository\DataMySql\Interpro\Tables\method.vb"

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

REM  Dump @3/29/2017 10:21:21 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `method`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `method` (
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `method_date` datetime NOT NULL,
'''   `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
'''   `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`method_ac`),
'''   KEY `fk_method$dbcode` (`dbcode`),
'''   CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("method", Database:="interpro", SchemaSQL:="
CREATE TABLE `method` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_date` datetime NOT NULL,
  `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
  `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`method_ac`),
  KEY `fk_method$dbcode` (`dbcode`),
  CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class method: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("method_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25")> Public Property method_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property name As String
    <DatabaseField("dbcode"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
    <DatabaseField("method_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property method_date As Date
    <DatabaseField("skip_flag"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property skip_flag As String
    <DatabaseField("candidate"), DataType(MySqlDbType.VarChar, "1")> Public Property candidate As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `method` WHERE `method_ac` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `method` SET `method_ac`='{0}', `name`='{1}', `dbcode`='{2}', `method_date`='{3}', `skip_flag`='{4}', `candidate`='{5}' WHERE `method_ac` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `method` WHERE `method_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, method_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, method_ac, name, dbcode, DataType.ToMySqlDateTimeString(method_date), skip_flag, candidate)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{method_ac}', '{name}', '{dbcode}', '{method_date}', '{skip_flag}', '{candidate}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, method_ac, name, dbcode, DataType.ToMySqlDateTimeString(method_date), skip_flag, candidate)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `method` SET `method_ac`='{0}', `name`='{1}', `dbcode`='{2}', `method_date`='{3}', `skip_flag`='{4}', `candidate`='{5}' WHERE `method_ac` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, method_ac, name, dbcode, DataType.ToMySqlDateTimeString(method_date), skip_flag, candidate, method_ac)
    End Function
#End Region
End Class


End Namespace

