#Region "Microsoft.VisualBasic::9a147ebc20f97b0a21192b29d6fc8b2f, ExternalDBSource\MetaCyc\bio_warehouse\enzreactioninhibitoractivator.vb"

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

    ' Class enzreactioninhibitoractivator
    ' 
    '     Properties: CompoundWID, EnzymaticReactionWID, InhibitOrActivate, Mechanism, PhysioRelevant
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
''' DROP TABLE IF EXISTS `enzreactioninhibitoractivator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzreactioninhibitoractivator` (
'''   `EnzymaticReactionWID` bigint(20) NOT NULL,
'''   `CompoundWID` bigint(20) NOT NULL,
'''   `InhibitOrActivate` char(1) DEFAULT NULL,
'''   `Mechanism` char(1) DEFAULT NULL,
'''   `PhysioRelevant` char(1) DEFAULT NULL,
'''   KEY `FK_EnzReactionIA1` (`EnzymaticReactionWID`),
'''   CONSTRAINT `FK_EnzReactionIA1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzreactioninhibitoractivator", Database:="warehouse")>
Public Class enzreactioninhibitoractivator: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("EnzymaticReactionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property EnzymaticReactionWID As Long
    <DatabaseField("CompoundWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompoundWID As Long
    <DatabaseField("InhibitOrActivate"), DataType(MySqlDbType.VarChar, "1")> Public Property InhibitOrActivate As String
    <DatabaseField("Mechanism"), DataType(MySqlDbType.VarChar, "1")> Public Property Mechanism As String
    <DatabaseField("PhysioRelevant"), DataType(MySqlDbType.VarChar, "1")> Public Property PhysioRelevant As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `enzreactioninhibitoractivator` WHERE `EnzymaticReactionWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `enzreactioninhibitoractivator` SET `EnzymaticReactionWID`='{0}', `CompoundWID`='{1}', `InhibitOrActivate`='{2}', `Mechanism`='{3}', `PhysioRelevant`='{4}' WHERE `EnzymaticReactionWID` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, EnzymaticReactionWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant, EnzymaticReactionWID)
    End Function
#End Region
End Class


End Namespace
