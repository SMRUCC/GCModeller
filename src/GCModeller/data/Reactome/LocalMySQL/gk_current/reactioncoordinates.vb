#Region "Microsoft.VisualBasic::1fd8ef2aa562ad8ccb8f38e3d19d5666, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\reactioncoordinates.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `reactioncoordinates`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reactioncoordinates` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `locatedEvent` int(10) unsigned DEFAULT NULL,
'''   `locatedEvent_class` varchar(64) DEFAULT NULL,
'''   `locationContext` int(10) unsigned DEFAULT NULL,
'''   `locationContext_class` varchar(64) DEFAULT NULL,
'''   `sourceX` int(10) DEFAULT NULL,
'''   `sourceY` int(10) DEFAULT NULL,
'''   `targetX` int(10) DEFAULT NULL,
'''   `targetY` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `locatedEvent` (`locatedEvent`),
'''   KEY `locationContext` (`locationContext`),
'''   KEY `sourceX` (`sourceX`),
'''   KEY `sourceY` (`sourceY`),
'''   KEY `targetX` (`targetX`),
'''   KEY `targetY` (`targetY`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reactioncoordinates", Database:="gk_current")>
Public Class reactioncoordinates: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("locatedEvent"), DataType(MySqlDbType.Int64, "10")> Public Property locatedEvent As Long
    <DatabaseField("locatedEvent_class"), DataType(MySqlDbType.VarChar, "64")> Public Property locatedEvent_class As String
    <DatabaseField("locationContext"), DataType(MySqlDbType.Int64, "10")> Public Property locationContext As Long
    <DatabaseField("locationContext_class"), DataType(MySqlDbType.VarChar, "64")> Public Property locationContext_class As String
    <DatabaseField("sourceX"), DataType(MySqlDbType.Int64, "10")> Public Property sourceX As Long
    <DatabaseField("sourceY"), DataType(MySqlDbType.Int64, "10")> Public Property sourceY As Long
    <DatabaseField("targetX"), DataType(MySqlDbType.Int64, "10")> Public Property targetX As Long
    <DatabaseField("targetY"), DataType(MySqlDbType.Int64, "10")> Public Property targetY As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reactioncoordinates` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reactioncoordinates` SET `DB_ID`='{0}', `locatedEvent`='{1}', `locatedEvent_class`='{2}', `locationContext`='{3}', `locationContext_class`='{4}', `sourceX`='{5}', `sourceY`='{6}', `targetX`='{7}', `targetY`='{8}' WHERE `DB_ID` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY, DB_ID)
    End Function
#End Region
End Class


End Namespace
