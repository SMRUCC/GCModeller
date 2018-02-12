#Region "Microsoft.VisualBasic::2217f9805c882f65fba26cbe9641951d, data\Reactome\LocalMySQL\gk_current\go_biologicalprocess.vb"

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

    ' Class go_biologicalprocess
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:27 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `go_biologicalprocess`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `go_biologicalprocess` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `accession` text,
'''   `definition` text,
'''   `referenceDatabase` int(10) unsigned DEFAULT NULL,
'''   `referenceDatabase_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `referenceDatabase` (`referenceDatabase`),
'''   FULLTEXT KEY `accession` (`accession`),
'''   FULLTEXT KEY `definition` (`definition`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("go_biologicalprocess", Database:="gk_current", SchemaSQL:="
CREATE TABLE `go_biologicalprocess` (
  `DB_ID` int(10) unsigned NOT NULL,
  `accession` text,
  `definition` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `accession` (`accession`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class go_biologicalprocess: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("accession"), DataType(MySqlDbType.Text)> Public Property accession As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
    <DatabaseField("referenceDatabase"), DataType(MySqlDbType.Int64, "10")> Public Property referenceDatabase As Long
    <DatabaseField("referenceDatabase_class"), DataType(MySqlDbType.VarChar, "64")> Public Property referenceDatabase_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `go_biologicalprocess` (`DB_ID`, `accession`, `definition`, `referenceDatabase`, `referenceDatabase_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `go_biologicalprocess` (`DB_ID`, `accession`, `definition`, `referenceDatabase`, `referenceDatabase_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `go_biologicalprocess` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `go_biologicalprocess` SET `DB_ID`='{0}', `accession`='{1}', `definition`='{2}', `referenceDatabase`='{3}', `referenceDatabase_class`='{4}' WHERE `DB_ID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `go_biologicalprocess` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `go_biologicalprocess` (`DB_ID`, `accession`, `definition`, `referenceDatabase`, `referenceDatabase_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, accession, definition, referenceDatabase, referenceDatabase_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{accession}', '{definition}', '{referenceDatabase}', '{referenceDatabase_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `go_biologicalprocess` (`DB_ID`, `accession`, `definition`, `referenceDatabase`, `referenceDatabase_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, accession, definition, referenceDatabase, referenceDatabase_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `go_biologicalprocess` SET `DB_ID`='{0}', `accession`='{1}', `definition`='{2}', `referenceDatabase`='{3}', `referenceDatabase_class`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, accession, definition, referenceDatabase, referenceDatabase_class, DB_ID)
    End Function
#End Region
End Class


End Namespace
