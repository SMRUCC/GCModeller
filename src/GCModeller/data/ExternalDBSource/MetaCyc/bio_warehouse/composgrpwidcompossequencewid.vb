#Region "Microsoft.VisualBasic::f290427e6bcf21432c3b9b3f9c310763, ExternalDBSource\MetaCyc\bio_warehouse\composgrpwidcompossequencewid.vb"

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

    ' Class composgrpwidcompossequencewid
    ' 
    '     Properties: CompositeGroupWID, CompositeSequenceWID
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `composgrpwidcompossequencewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `composgrpwidcompossequencewid` (
'''   `CompositeGroupWID` bigint(20) NOT NULL,
'''   `CompositeSequenceWID` bigint(20) NOT NULL,
'''   KEY `FK_ComposGrpWIDComposSequenc1` (`CompositeGroupWID`),
'''   KEY `FK_ComposGrpWIDComposSequenc2` (`CompositeSequenceWID`),
'''   CONSTRAINT `FK_ComposGrpWIDComposSequenc1` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ComposGrpWIDComposSequenc2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("composgrpwidcompossequencewid", Database:="warehouse")>
Public Class composgrpwidcompossequencewid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("CompositeGroupWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompositeGroupWID As Long
    <DatabaseField("CompositeSequenceWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompositeSequenceWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `composgrpwidcompossequencewid` WHERE `CompositeGroupWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `composgrpwidcompossequencewid` SET `CompositeGroupWID`='{0}', `CompositeSequenceWID`='{1}' WHERE `CompositeGroupWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, CompositeGroupWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, CompositeGroupWID, CompositeSequenceWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, CompositeGroupWID, CompositeSequenceWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, CompositeGroupWID, CompositeSequenceWID, CompositeGroupWID)
    End Function
#End Region
End Class


End Namespace
