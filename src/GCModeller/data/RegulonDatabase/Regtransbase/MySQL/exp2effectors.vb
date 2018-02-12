#Region "Microsoft.VisualBasic::50dc485925387cd810bc0f08c2ac74a2, data\RegulonDatabase\Regtransbase\MySQL\exp2effectors.vb"

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

    ' Class exp2effectors
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:54:58 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

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
''' 
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class exp2effectors: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("exp_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property exp_guid As Long
    <DatabaseField("effector_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property effector_guid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `exp2effectors` (`pkg_guid`, `art_guid`, `exp_guid`, `effector_guid`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `exp2effectors` WHERE `exp_guid`='{0}' and `effector_guid`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `exp2effectors` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `effector_guid`='{3}' WHERE `exp_guid`='{4}' and `effector_guid`='{5}';</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{effector_guid}')"
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
''' UPDATE `exp2effectors` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `effector_guid`='{3}' WHERE `exp_guid`='{4}' and `effector_guid`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, exp_guid, effector_guid, exp_guid, effector_guid)
    End Function
#End Region
End Class


End Namespace
