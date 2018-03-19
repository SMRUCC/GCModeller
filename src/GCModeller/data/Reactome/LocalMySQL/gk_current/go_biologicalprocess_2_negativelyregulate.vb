#Region "Microsoft.VisualBasic::9ac49206560ed7143d70aba80c98d926, data\Reactome\LocalMySQL\gk_current\go_biologicalprocess_2_negativelyregulate.vb"

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

    ' Class go_biologicalprocess_2_negativelyregulate
    ' 
    '     Properties: DB_ID, negativelyRegulate, negativelyRegulate_class, negativelyRegulate_rank
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:21 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `go_biologicalprocess_2_negativelyregulate`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `go_biologicalprocess_2_negativelyregulate` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `negativelyRegulate_rank` int(10) unsigned DEFAULT NULL,
'''   `negativelyRegulate` int(10) unsigned DEFAULT NULL,
'''   `negativelyRegulate_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `negativelyRegulate` (`negativelyRegulate`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("go_biologicalprocess_2_negativelyregulate", Database:="gk_current", SchemaSQL:="
CREATE TABLE `go_biologicalprocess_2_negativelyregulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_rank` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `negativelyRegulate` (`negativelyRegulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class go_biologicalprocess_2_negativelyregulate: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("negativelyRegulate_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="negativelyRegulate_rank")> Public Property negativelyRegulate_rank As Long
    <DatabaseField("negativelyRegulate"), DataType(MySqlDbType.Int64, "10"), Column(Name:="negativelyRegulate")> Public Property negativelyRegulate As Long
    <DatabaseField("negativelyRegulate_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="negativelyRegulate_class")> Public Property negativelyRegulate_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `go_biologicalprocess_2_negativelyregulate` (`DB_ID`, `negativelyRegulate_rank`, `negativelyRegulate`, `negativelyRegulate_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `go_biologicalprocess_2_negativelyregulate` (`DB_ID`, `negativelyRegulate_rank`, `negativelyRegulate`, `negativelyRegulate_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `go_biologicalprocess_2_negativelyregulate` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `go_biologicalprocess_2_negativelyregulate` SET `DB_ID`='{0}', `negativelyRegulate_rank`='{1}', `negativelyRegulate`='{2}', `negativelyRegulate_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `go_biologicalprocess_2_negativelyregulate` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `go_biologicalprocess_2_negativelyregulate` (`DB_ID`, `negativelyRegulate_rank`, `negativelyRegulate`, `negativelyRegulate_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, negativelyRegulate_rank, negativelyRegulate, negativelyRegulate_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{negativelyRegulate_rank}', '{negativelyRegulate}', '{negativelyRegulate_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `go_biologicalprocess_2_negativelyregulate` (`DB_ID`, `negativelyRegulate_rank`, `negativelyRegulate`, `negativelyRegulate_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, negativelyRegulate_rank, negativelyRegulate, negativelyRegulate_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `go_biologicalprocess_2_negativelyregulate` SET `DB_ID`='{0}', `negativelyRegulate_rank`='{1}', `negativelyRegulate`='{2}', `negativelyRegulate_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, negativelyRegulate_rank, negativelyRegulate, negativelyRegulate_class, DB_ID)
    End Function
#End Region
Public Function Clone() As go_biologicalprocess_2_negativelyregulate
                  Return DirectCast(MyClass.MemberwiseClone, go_biologicalprocess_2_negativelyregulate)
              End Function
End Class


End Namespace

