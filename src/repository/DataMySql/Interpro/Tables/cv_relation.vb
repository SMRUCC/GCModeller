#Region "Microsoft.VisualBasic::9ce01cb428a633240f3f041271f1b1d9, DataMySql\Interpro\Tables\cv_relation.vb"

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

    ' Class cv_relation
    ' 
    '     Properties: abbrev, code, description, forward, reverse
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
''' DROP TABLE IF EXISTS `cv_relation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `cv_relation` (
'''   `code` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `abbrev` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
'''   `forward` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `reverse` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   PRIMARY KEY (`code`),
'''   UNIQUE KEY `uq_relation$abbrev` (`abbrev`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cv_relation", Database:="interpro", SchemaSQL:="
CREATE TABLE `cv_relation` (
  `code` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abbrev` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `forward` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `reverse` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`code`),
  UNIQUE KEY `uq_relation$abbrev` (`abbrev`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class cv_relation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("code"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "2"), Column(Name:="code"), XmlAttribute> Public Property code As String
    <DatabaseField("abbrev"), NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="abbrev")> Public Property abbrev As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("forward"), NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="forward")> Public Property forward As String
    <DatabaseField("reverse"), NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="reverse")> Public Property reverse As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `cv_relation` WHERE `code` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `cv_relation` SET `code`='{0}', `abbrev`='{1}', `description`='{2}', `forward`='{3}', `reverse`='{4}' WHERE `code` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `cv_relation` WHERE `code` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, code, abbrev, description, forward, reverse)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, code, abbrev, description, forward, reverse)
        Else
        Return String.Format(INSERT_SQL, code, abbrev, description, forward, reverse)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{code}', '{abbrev}', '{description}', '{forward}', '{reverse}')"
        Else
            Return $"('{code}', '{abbrev}', '{description}', '{forward}', '{reverse}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, code, abbrev, description, forward, reverse)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `cv_relation` (`code`, `abbrev`, `description`, `forward`, `reverse`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, code, abbrev, description, forward, reverse)
        Else
        Return String.Format(REPLACE_SQL, code, abbrev, description, forward, reverse)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `cv_relation` SET `code`='{0}', `abbrev`='{1}', `description`='{2}', `forward`='{3}', `reverse`='{4}' WHERE `code` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, code, abbrev, description, forward, reverse, code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As cv_relation
                         Return DirectCast(MyClass.MemberwiseClone, cv_relation)
                     End Function
End Class


End Namespace
