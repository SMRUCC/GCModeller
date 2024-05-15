#Region "Microsoft.VisualBasic::b2785ea315b41293159cd169b5245611, data\Reactome\LocalMySQL\gk_current\polymer.vb"

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

    '   Total Lines: 173
    '    Code Lines: 87
    ' Comment Lines: 64
    '   Blank Lines: 22
    '     File Size: 7.70 KB


    ' Class polymer
    ' 
    '     Properties: DB_ID, inferredProt, maxHomologues, maxUnitCount, minUnitCount
    '                 totalProt
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

REM  Dump @2018/5/23 13:13:41


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
''' DROP TABLE IF EXISTS `polymer`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `polymer` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `maxUnitCount` int(10) DEFAULT NULL,
'''   `minUnitCount` int(10) DEFAULT NULL,
'''   `totalProt` text,
'''   `maxHomologues` text,
'''   `inferredProt` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `maxUnitCount` (`maxUnitCount`),
'''   KEY `minUnitCount` (`minUnitCount`),
'''   FULLTEXT KEY `totalProt` (`totalProt`),
'''   FULLTEXT KEY `maxHomologues` (`maxHomologues`),
'''   FULLTEXT KEY `inferredProt` (`inferredProt`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("polymer", Database:="gk_current", SchemaSQL:="
CREATE TABLE `polymer` (
  `DB_ID` int(10) unsigned NOT NULL,
  `maxUnitCount` int(10) DEFAULT NULL,
  `minUnitCount` int(10) DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `maxUnitCount` (`maxUnitCount`),
  KEY `minUnitCount` (`minUnitCount`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class polymer: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("maxUnitCount"), DataType(MySqlDbType.Int64, "10"), Column(Name:="maxUnitCount")> Public Property maxUnitCount As Long
    <DatabaseField("minUnitCount"), DataType(MySqlDbType.Int64, "10"), Column(Name:="minUnitCount")> Public Property minUnitCount As Long
    <DatabaseField("totalProt"), DataType(MySqlDbType.Text), Column(Name:="totalProt")> Public Property totalProt As String
    <DatabaseField("maxHomologues"), DataType(MySqlDbType.Text), Column(Name:="maxHomologues")> Public Property maxHomologues As String
    <DatabaseField("inferredProt"), DataType(MySqlDbType.Text), Column(Name:="inferredProt")> Public Property inferredProt As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `polymer` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `polymer` SET `DB_ID`='{0}', `maxUnitCount`='{1}', `minUnitCount`='{2}', `totalProt`='{3}', `maxHomologues`='{4}', `inferredProt`='{5}' WHERE `DB_ID` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `polymer` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
        Else
        Return String.Format(INSERT_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{maxUnitCount}', '{minUnitCount}', '{totalProt}', '{maxHomologues}', '{inferredProt}')"
        Else
            Return $"('{DB_ID}', '{maxUnitCount}', '{minUnitCount}', '{totalProt}', '{maxHomologues}', '{inferredProt}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `polymer` (`DB_ID`, `maxUnitCount`, `minUnitCount`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `polymer` SET `DB_ID`='{0}', `maxUnitCount`='{1}', `minUnitCount`='{2}', `totalProt`='{3}', `maxHomologues`='{4}', `inferredProt`='{5}' WHERE `DB_ID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, maxUnitCount, minUnitCount, totalProt, maxHomologues, inferredProt, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As polymer
                         Return DirectCast(MyClass.MemberwiseClone, polymer)
                     End Function
End Class


End Namespace
