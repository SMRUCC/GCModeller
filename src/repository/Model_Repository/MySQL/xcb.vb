#Region "Microsoft.VisualBasic::38e22b7dcb0a431aeb47f797086cbdc6, Model_Repository\MySQL\xcb.vb"

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

    ' Class xcb
    ' 
    '     Properties: g1_entity, g2_entity, pcc, spcc, uid
    '                 wgcna_weight
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

REM  Dump @2018/5/23 13:13:35


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xcb`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xcb` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `g1_entity` varchar(45) NOT NULL,
'''   `g2_entity` varchar(45) NOT NULL,
'''   `pcc` double DEFAULT '0',
'''   `spcc` double DEFAULT '0',
'''   `wgcna_weight` double DEFAULT '0',
'''   PRIMARY KEY (`g1_entity`,`g2_entity`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=18275626 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-12-03 21:01:55
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xcb", Database:="correlations", SchemaSQL:="
CREATE TABLE `xcb` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `g1_entity` varchar(45) NOT NULL,
  `g2_entity` varchar(45) NOT NULL,
  `pcc` double DEFAULT '0',
  `spcc` double DEFAULT '0',
  `wgcna_weight` double DEFAULT '0',
  PRIMARY KEY (`g1_entity`,`g2_entity`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=18275626 DEFAULT CHARSET=utf8;")>
Public Class xcb: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
    <DatabaseField("g1_entity"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="g1_entity"), XmlAttribute> Public Property g1_entity As String
    <DatabaseField("g2_entity"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="g2_entity"), XmlAttribute> Public Property g2_entity As String
    <DatabaseField("pcc"), DataType(MySqlDbType.Double), Column(Name:="pcc")> Public Property pcc As Double
    <DatabaseField("spcc"), DataType(MySqlDbType.Double), Column(Name:="spcc")> Public Property spcc As Double
    <DatabaseField("wgcna_weight"), DataType(MySqlDbType.Double), Column(Name:="wgcna_weight")> Public Property wgcna_weight As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `xcb` (`g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `xcb` (`g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `xcb` WHERE `g1_entity`='{0}' and `g2_entity`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `xcb` SET `uid`='{0}', `g1_entity`='{1}', `g2_entity`='{2}', `pcc`='{3}', `spcc`='{4}', `wgcna_weight`='{5}' WHERE `g1_entity`='{6}' and `g2_entity`='{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `xcb` WHERE `g1_entity`='{0}' and `g2_entity`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, g1_entity, g2_entity)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
        Else
        Return String.Format(INSERT_SQL, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{g1_entity}', '{g2_entity}', '{pcc}', '{spcc}', '{wgcna_weight}')"
        Else
            Return $"('{g1_entity}', '{g2_entity}', '{pcc}', '{spcc}', '{wgcna_weight}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `xcb` (`uid`, `g1_entity`, `g2_entity`, `pcc`, `spcc`, `wgcna_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
        Else
        Return String.Format(REPLACE_SQL, g1_entity, g2_entity, pcc, spcc, wgcna_weight)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `xcb` SET `uid`='{0}', `g1_entity`='{1}', `g2_entity`='{2}', `pcc`='{3}', `spcc`='{4}', `wgcna_weight`='{5}' WHERE `g1_entity`='{6}' and `g2_entity`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, g1_entity, g2_entity, pcc, spcc, wgcna_weight, g1_entity, g2_entity)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As xcb
                         Return DirectCast(MyClass.MemberwiseClone, xcb)
                     End Function
End Class


End Namespace
