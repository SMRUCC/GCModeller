﻿#Region "Microsoft.VisualBasic::106c83a4904a2c006c640348f0eebdb3, data\RegulonDatabase\Regtransbase\MySQL\exp2effectors.vb"

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
    '    Code Lines: 85 (49.71%)
    ' Comment Lines: 64 (37.43%)
    '    - Xml Docs: 95.31%
    ' 
    '   Blank Lines: 22 (12.87%)
    '     File Size: 7.75 KB


    ' Class exp2effectors
    ' 
    '     Properties: art_guid, effector_guid, exp_guid, pkg_guid
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
''' DROP TABLE IF EXISTS `exp2effectors`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `exp2effectors` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `exp_guid` int(11) NOT NULL DEFAULT '0',
'''   `effector_guid` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`exp_guid`,`effector_guid`),
'''   KEY `FK_exp2effectors-pkg_guid` (`pkg_guid`),
'''   KEY `FK_exp2effectors-art_guid` (`art_guid`),
'''   KEY `FK_exp2effectors-effector_guid` (`effector_guid`),
'''   CONSTRAINT `FK_exp2effectors-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_exp2effectors-effector_guid` FOREIGN KEY (`effector_guid`) REFERENCES `dict_effectors` (`effector_guid`),
'''   CONSTRAINT `FK_exp2effectors-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
'''   CONSTRAINT `FK_exp2effectors-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("exp2effectors", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `exp2effectors` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `effector_guid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`exp_guid`,`effector_guid`),
  KEY `FK_exp2effectors-pkg_guid` (`pkg_guid`),
  KEY `FK_exp2effectors-art_guid` (`art_guid`),
  KEY `FK_exp2effectors-effector_guid` (`effector_guid`),
  CONSTRAINT `FK_exp2effectors-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp2effectors-effector_guid` FOREIGN KEY (`effector_guid`) REFERENCES `dict_effectors` (`effector_guid`),
  CONSTRAINT `FK_exp2effectors-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp2effectors-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class exp2effectors: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("exp_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="exp_guid"), XmlAttribute> Public Property exp_guid As Long
    <DatabaseField("effector_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="effector_guid"), XmlAttribute> Public Property effector_guid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `exp2effectors` WHERE `exp_guid`='{0}' and `effector_guid`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `exp2effectors` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `effector_guid`='{3}' WHERE `exp_guid`='{4}' and `effector_guid`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `exp2effectors` WHERE `exp_guid`='{0}' and `effector_guid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, exp_guid, effector_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{effector_guid}')"
        Else
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{effector_guid}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, effector_guid)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `exp2effectors` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `effector_guid`='{3}' WHERE `exp_guid`='{4}' and `effector_guid`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, exp_guid, effector_guid, exp_guid, effector_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As exp2effectors
                         Return DirectCast(MyClass.MemberwiseClone, exp2effectors)
                     End Function
End Class


End Namespace
