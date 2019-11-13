#Region "Microsoft.VisualBasic::6e94c51dbe0502effc59ae2b3ae9e95d, data\KEGG\jp_kegg2\class_br08201_reaction.vb"

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

    ' Class class_br08201_reaction
    ' 
    '     Properties: EC, level1, level2, level3, name
    '                 rn, uid
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

REM  Dump @2018/5/23 13:16:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' KEGG enzymic reaction catagory
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `class_br08201_reaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_br08201_reaction` (
'''   `uid` int(11) NOT NULL,
'''   `rn` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   `EC` varchar(45) DEFAULT NULL COMMENT 'level4',
'''   `level1` varchar(45) DEFAULT NULL,
'''   `level2` varchar(45) DEFAULT NULL,
'''   `level3` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG enzymic reaction catagory';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class_br08201_reaction", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `class_br08201_reaction` (
  `uid` int(11) NOT NULL,
  `rn` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `EC` varchar(45) DEFAULT NULL COMMENT 'level4',
  `level1` varchar(45) DEFAULT NULL,
  `level2` varchar(45) DEFAULT NULL,
  `level3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG enzymic reaction catagory';")>
Public Class class_br08201_reaction: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("rn"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="rn")> Public Property rn As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
''' <summary>
''' level4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("EC"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="EC")> Public Property EC As String
    <DatabaseField("level1"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level1")> Public Property level1 As String
    <DatabaseField("level2"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level2")> Public Property level2 As String
    <DatabaseField("level3"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level3")> Public Property level3 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `class_br08201_reaction` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `class_br08201_reaction` SET `uid`='{0}', `rn`='{1}', `name`='{2}', `EC`='{3}', `level1`='{4}', `level2`='{5}', `level3`='{6}' WHERE `uid` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `class_br08201_reaction` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, rn, name, EC, level1, level2, level3)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, rn, name, EC, level1, level2, level3)
        Else
        Return String.Format(INSERT_SQL, uid, rn, name, EC, level1, level2, level3)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{rn}', '{name}', '{EC}', '{level1}', '{level2}', '{level3}')"
        Else
            Return $"('{uid}', '{rn}', '{name}', '{EC}', '{level1}', '{level2}', '{level3}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, rn, name, EC, level1, level2, level3)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `class_br08201_reaction` (`uid`, `rn`, `name`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, rn, name, EC, level1, level2, level3)
        Else
        Return String.Format(REPLACE_SQL, uid, rn, name, EC, level1, level2, level3)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `class_br08201_reaction` SET `uid`='{0}', `rn`='{1}', `name`='{2}', `EC`='{3}', `level1`='{4}', `level2`='{5}', `level3`='{6}' WHERE `uid` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, rn, name, EC, level1, level2, level3, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As class_br08201_reaction
                         Return DirectCast(MyClass.MemberwiseClone, class_br08201_reaction)
                     End Function
End Class


End Namespace
