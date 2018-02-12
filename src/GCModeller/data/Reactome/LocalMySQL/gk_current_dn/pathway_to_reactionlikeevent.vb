#Region "Microsoft.VisualBasic::904e642212bf08008892beefbf0cb34f, data\Reactome\LocalMySQL\gk_current_dn\pathway_to_reactionlikeevent.vb"

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

    ' Class pathway_to_reactionlikeevent
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:30 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current_dn

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `pathway_to_reactionlikeevent`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathway_to_reactionlikeevent` (
'''   `pathwayId` int(32) NOT NULL DEFAULT '0',
'''   `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`pathwayId`,`reactionLikeEventId`),
'''   KEY `reactionLikeEventId` (`reactionLikeEventId`),
'''   CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`),
'''   CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_2` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathway_to_reactionlikeevent", Database:="gk_current_dn", SchemaSQL:="
CREATE TABLE `pathway_to_reactionlikeevent` (
  `pathwayId` int(32) NOT NULL DEFAULT '0',
  `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`pathwayId`,`reactionLikeEventId`),
  KEY `reactionLikeEventId` (`reactionLikeEventId`),
  CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`),
  CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_2` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pathway_to_reactionlikeevent: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathwayId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32")> Public Property pathwayId As Long
    <DatabaseField("reactionLikeEventId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32")> Public Property reactionLikeEventId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pathway_to_reactionlikeevent` (`pathwayId`, `reactionLikeEventId`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pathway_to_reactionlikeevent` (`pathwayId`, `reactionLikeEventId`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pathway_to_reactionlikeevent` WHERE `pathwayId`='{0}' and `reactionLikeEventId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pathway_to_reactionlikeevent` SET `pathwayId`='{0}', `reactionLikeEventId`='{1}' WHERE `pathwayId`='{2}' and `reactionLikeEventId`='{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `pathway_to_reactionlikeevent` WHERE `pathwayId`='{0}' and `reactionLikeEventId`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathwayId, reactionLikeEventId)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `pathway_to_reactionlikeevent` (`pathwayId`, `reactionLikeEventId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathwayId, reactionLikeEventId)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pathwayId}', '{reactionLikeEventId}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pathway_to_reactionlikeevent` (`pathwayId`, `reactionLikeEventId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathwayId, reactionLikeEventId)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `pathway_to_reactionlikeevent` SET `pathwayId`='{0}', `reactionLikeEventId`='{1}' WHERE `pathwayId`='{2}' and `reactionLikeEventId`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathwayId, reactionLikeEventId, pathwayId, reactionLikeEventId)
    End Function
#End Region
End Class


End Namespace
