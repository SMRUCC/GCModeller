#Region "Microsoft.VisualBasic::739bb3f6e4ac4c59674ef0ac9e082881, DataMySql\Interpro\Tables\organism.vb"

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

    ' Class organism
    ' 
    '     Properties: full_name, italics_name, name, oscode, tax_code
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
''' DROP TABLE IF EXISTS `organism`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `organism` (
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `tax_code` decimal(38,0) DEFAULT NULL,
'''   PRIMARY KEY (`oscode`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("organism", Database:="interpro", SchemaSQL:="
CREATE TABLE `organism` (
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_code` decimal(38,0) DEFAULT NULL,
  PRIMARY KEY (`oscode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class organism: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="oscode"), XmlAttribute> Public Property oscode As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="name")> Public Property name As String
    <DatabaseField("italics_name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="italics_name")> Public Property italics_name As String
    <DatabaseField("full_name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="full_name")> Public Property full_name As String
    <DatabaseField("tax_code"), DataType(MySqlDbType.Decimal), Column(Name:="tax_code")> Public Property tax_code As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `organism` WHERE `oscode` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `organism` SET `oscode`='{0}', `name`='{1}', `italics_name`='{2}', `full_name`='{3}', `tax_code`='{4}' WHERE `oscode` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `organism` WHERE `oscode` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, oscode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, oscode, name, italics_name, full_name, tax_code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, oscode, name, italics_name, full_name, tax_code)
        Else
        Return String.Format(INSERT_SQL, oscode, name, italics_name, full_name, tax_code)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{oscode}', '{name}', '{italics_name}', '{full_name}', '{tax_code}')"
        Else
            Return $"('{oscode}', '{name}', '{italics_name}', '{full_name}', '{tax_code}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, oscode, name, italics_name, full_name, tax_code)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, oscode, name, italics_name, full_name, tax_code)
        Else
        Return String.Format(REPLACE_SQL, oscode, name, italics_name, full_name, tax_code)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `organism` SET `oscode`='{0}', `name`='{1}', `italics_name`='{2}', `full_name`='{3}', `tax_code`='{4}' WHERE `oscode` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, oscode, name, italics_name, full_name, tax_code, oscode)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As organism
                         Return DirectCast(MyClass.MemberwiseClone, organism)
                     End Function
End Class


End Namespace
