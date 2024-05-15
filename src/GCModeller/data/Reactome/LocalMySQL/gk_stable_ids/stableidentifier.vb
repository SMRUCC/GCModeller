#Region "Microsoft.VisualBasic::569a8c462305d25e182bc34bf38be8db, data\Reactome\LocalMySQL\gk_stable_ids\stableidentifier.vb"

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

    '   Total Lines: 176
    '    Code Lines: 79
    ' Comment Lines: 75
    '   Blank Lines: 22
    '     File Size: 7.20 KB


    ' Class stableidentifier
    ' 
    '     Properties: DB_ID, identifier, identifierVersion, instanceId
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

REM  Dump @2018/5/23 13:13:42


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_stable_ids

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `stableidentifier`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `stableidentifier` (
'''   `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
'''   `identifier` varchar(32) DEFAULT NULL,
'''   `identifierVersion` int(4) DEFAULT NULL,
'''   `instanceId` int(12) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `identifier` (`identifier`(12))
''' ) ENGINE=MyISAM AUTO_INCREMENT=1792857 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'gk_stable_ids'
''' --
''' 
''' --
''' -- Dumping routines for database 'gk_stable_ids'
''' --
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
''' -- Dump completed on 2017-03-29 21:35:20
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("stableidentifier", Database:="gk_stable_ids", SchemaSQL:="
CREATE TABLE `stableidentifier` (
  `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
  `identifier` varchar(32) DEFAULT NULL,
  `identifierVersion` int(4) DEFAULT NULL,
  `instanceId` int(12) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `identifier` (`identifier`(12))
) ENGINE=MyISAM AUTO_INCREMENT=1792857 DEFAULT CHARSET=latin1;")>
Public Class stableidentifier: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "12"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("identifier"), DataType(MySqlDbType.VarChar, "32"), Column(Name:="identifier")> Public Property identifier As String
    <DatabaseField("identifierVersion"), DataType(MySqlDbType.Int64, "4"), Column(Name:="identifierVersion")> Public Property identifierVersion As Long
    <DatabaseField("instanceId"), DataType(MySqlDbType.Int64, "12"), Column(Name:="instanceId")> Public Property instanceId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `stableidentifier` (`identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `stableidentifier` (`identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `stableidentifier` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `stableidentifier` SET `DB_ID`='{0}', `identifier`='{1}', `identifierVersion`='{2}', `instanceId`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `stableidentifier` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, identifier, identifierVersion, instanceId)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, identifier, identifierVersion, instanceId)
        Else
        Return String.Format(INSERT_SQL, identifier, identifierVersion, instanceId)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{identifier}', '{identifierVersion}', '{instanceId}')"
        Else
            Return $"('{identifier}', '{identifierVersion}', '{instanceId}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, identifier, identifierVersion, instanceId)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `stableidentifier` (`DB_ID`, `identifier`, `identifierVersion`, `instanceId`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, identifier, identifierVersion, instanceId)
        Else
        Return String.Format(REPLACE_SQL, identifier, identifierVersion, instanceId)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `stableidentifier` SET `DB_ID`='{0}', `identifier`='{1}', `identifierVersion`='{2}', `instanceId`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, identifier, identifierVersion, instanceId, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As stableidentifier
                         Return DirectCast(MyClass.MemberwiseClone, stableidentifier)
                     End Function
End Class


End Namespace
