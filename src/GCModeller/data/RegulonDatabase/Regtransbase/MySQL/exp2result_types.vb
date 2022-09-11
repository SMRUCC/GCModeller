#Region "Microsoft.VisualBasic::1b9633147b5db4a3148a13fe51b68913, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\exp2result_types.vb"

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


    ' Code Statistics:

    '   Total Lines: 171
    '    Code Lines: 85
    ' Comment Lines: 64
    '   Blank Lines: 22
    '     File Size: 8.15 KB


    ' Class exp2result_types
    ' 
    '     Properties: art_guid, exp_guid, exp_result_type_guid, pkg_guid
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `exp2result_types`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `exp2result_types` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `exp_guid` int(11) NOT NULL DEFAULT '0',
'''   `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`exp_guid`,`exp_result_type_guid`),
'''   KEY `FK_exp2result_types-pkg_guid` (`pkg_guid`),
'''   KEY `FK_exp2result_types-art_guid` (`art_guid`),
'''   KEY `FK_exp2result_types-exp_result_type_guid` (`exp_result_type_guid`),
'''   CONSTRAINT `FK_exp2result_types-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_exp2result_types-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
'''   CONSTRAINT `FK_exp2result_types-exp_result_type_guid` FOREIGN KEY (`exp_result_type_guid`) REFERENCES `dict_exp_result_types` (`exp_result_type_guid`),
'''   CONSTRAINT `FK_exp2result_types-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("exp2result_types", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `exp2result_types` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`exp_guid`,`exp_result_type_guid`),
  KEY `FK_exp2result_types-pkg_guid` (`pkg_guid`),
  KEY `FK_exp2result_types-art_guid` (`art_guid`),
  KEY `FK_exp2result_types-exp_result_type_guid` (`exp_result_type_guid`),
  CONSTRAINT `FK_exp2result_types-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp2result_types-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp2result_types-exp_result_type_guid` FOREIGN KEY (`exp_result_type_guid`) REFERENCES `dict_exp_result_types` (`exp_result_type_guid`),
  CONSTRAINT `FK_exp2result_types-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class exp2result_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("exp_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="exp_guid"), XmlAttribute> Public Property exp_guid As Long
    <DatabaseField("exp_result_type_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="exp_result_type_guid"), XmlAttribute> Public Property exp_result_type_guid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `exp2result_types` WHERE `exp_guid`='{0}' and `exp_result_type_guid`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `exp2result_types` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `exp_result_type_guid`='{3}' WHERE `exp_guid`='{4}' and `exp_result_type_guid`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `exp2result_types` WHERE `exp_guid`='{0}' and `exp_result_type_guid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, exp_guid, exp_result_type_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{exp_result_type_guid}')"
        Else
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{exp_result_type_guid}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `exp2result_types` (`pkg_guid`, `art_guid`, `exp_guid`, `exp_result_type_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `exp2result_types` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `exp_result_type_guid`='{3}' WHERE `exp_guid`='{4}' and `exp_result_type_guid`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, exp_guid, exp_result_type_guid, exp_guid, exp_result_type_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As exp2result_types
                         Return DirectCast(MyClass.MemberwiseClone, exp2result_types)
                     End Function
End Class


End Namespace
