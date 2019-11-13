#Region "Microsoft.VisualBasic::ca8ceb7dc5411a771121295ed166160f, data\KEGG\jp_kegg2\meta_class_br08201.vb"

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

    ' Class meta_class_br08201
    ' 
    '     Properties: EC, level1, level2, level3, uid
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
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `meta_class_br08201`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `meta_class_br08201` (
'''   `uid` int(11) NOT NULL,
'''   `EC` varchar(45) DEFAULT NULL,
'''   `level1` varchar(45) DEFAULT NULL,
'''   `level2` varchar(45) DEFAULT NULL,
'''   `level3` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("meta_class_br08201", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `meta_class_br08201` (
  `uid` int(11) NOT NULL,
  `EC` varchar(45) DEFAULT NULL,
  `level1` varchar(45) DEFAULT NULL,
  `level2` varchar(45) DEFAULT NULL,
  `level3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class meta_class_br08201: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("EC"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="EC")> Public Property EC As String
    <DatabaseField("level1"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level1")> Public Property level1 As String
    <DatabaseField("level2"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level2")> Public Property level2 As String
    <DatabaseField("level3"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level3")> Public Property level3 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `meta_class_br08201` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `meta_class_br08201` SET `uid`='{0}', `EC`='{1}', `level1`='{2}', `level2`='{3}', `level3`='{4}' WHERE `uid` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `meta_class_br08201` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, EC, level1, level2, level3)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, EC, level1, level2, level3)
        Else
        Return String.Format(INSERT_SQL, uid, EC, level1, level2, level3)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{EC}', '{level1}', '{level2}', '{level3}')"
        Else
            Return $"('{uid}', '{EC}', '{level1}', '{level2}', '{level3}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, EC, level1, level2, level3)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `meta_class_br08201` (`uid`, `EC`, `level1`, `level2`, `level3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, EC, level1, level2, level3)
        Else
        Return String.Format(REPLACE_SQL, uid, EC, level1, level2, level3)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `meta_class_br08201` SET `uid`='{0}', `EC`='{1}', `level1`='{2}', `level2`='{3}', `level3`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, EC, level1, level2, level3, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As meta_class_br08201
                         Return DirectCast(MyClass.MemberwiseClone, meta_class_br08201)
                     End Function
End Class


End Namespace
