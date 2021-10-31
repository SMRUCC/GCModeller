#Region "Microsoft.VisualBasic::ff3efe1407168f86d89173bf45807c10, DataMySql\Interpro\Tables\struct_class.vb"

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

    ' Class struct_class
    ' 
    '     Properties: dbcode, domain_id, fam_id
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
''' DROP TABLE IF EXISTS `struct_class`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `struct_class` (
'''   `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `fam_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`domain_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("struct_class", Database:="interpro", SchemaSQL:="
CREATE TABLE `struct_class` (
  `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `fam_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`domain_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class struct_class: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("domain_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "14"), Column(Name:="domain_id"), XmlAttribute> Public Property domain_id As String
    <DatabaseField("fam_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="fam_id")> Public Property fam_id As String
    <DatabaseField("dbcode"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode")> Public Property dbcode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `struct_class` WHERE `domain_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `struct_class` SET `domain_id`='{0}', `fam_id`='{1}', `dbcode`='{2}' WHERE `domain_id` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `struct_class` WHERE `domain_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, domain_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, domain_id, fam_id, dbcode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, domain_id, fam_id, dbcode)
        Else
        Return String.Format(INSERT_SQL, domain_id, fam_id, dbcode)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{domain_id}', '{fam_id}', '{dbcode}')"
        Else
            Return $"('{domain_id}', '{fam_id}', '{dbcode}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, domain_id, fam_id, dbcode)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, domain_id, fam_id, dbcode)
        Else
        Return String.Format(REPLACE_SQL, domain_id, fam_id, dbcode)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `struct_class` SET `domain_id`='{0}', `fam_id`='{1}', `dbcode`='{2}' WHERE `domain_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, domain_id, fam_id, dbcode, domain_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As struct_class
                         Return DirectCast(MyClass.MemberwiseClone, struct_class)
                     End Function
End Class


End Namespace
