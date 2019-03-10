#Region "Microsoft.VisualBasic::21429b6ed5c9597458d0e49820f35dd6, data\ExternalDBSource\MetaCyc\bio_warehouse\enzreactionaltcompound.vb"

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

    ' Class enzreactionaltcompound
    ' 
    '     Properties: AlternativeWID, Cofactor, EnzymaticReactionWID, PrimaryWID
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
''' DROP TABLE IF EXISTS `enzreactionaltcompound`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzreactionaltcompound` (
'''   `EnzymaticReactionWID` bigint(20) NOT NULL,
'''   `PrimaryWID` bigint(20) NOT NULL,
'''   `AlternativeWID` bigint(20) NOT NULL,
'''   `Cofactor` char(1) DEFAULT NULL,
'''   KEY `FK_ERAC1` (`EnzymaticReactionWID`),
'''   KEY `FK_ERAC2` (`PrimaryWID`),
'''   KEY `FK_ERAC3` (`AlternativeWID`),
'''   CONSTRAINT `FK_ERAC1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ERAC2` FOREIGN KEY (`PrimaryWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ERAC3` FOREIGN KEY (`AlternativeWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzreactionaltcompound", Database:="warehouse")>
Public Class enzreactionaltcompound: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("EnzymaticReactionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property EnzymaticReactionWID As Long
    <DatabaseField("PrimaryWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property PrimaryWID As Long
    <DatabaseField("AlternativeWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property AlternativeWID As Long
    <DatabaseField("Cofactor"), DataType(MySqlDbType.VarChar, "1")> Public Property Cofactor As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `enzreactionaltcompound` (`EnzymaticReactionWID`, `PrimaryWID`, `AlternativeWID`, `Cofactor`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `enzreactionaltcompound` (`EnzymaticReactionWID`, `PrimaryWID`, `AlternativeWID`, `Cofactor`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `enzreactionaltcompound` WHERE `EnzymaticReactionWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `enzreactionaltcompound` SET `EnzymaticReactionWID`='{0}', `PrimaryWID`='{1}', `AlternativeWID`='{2}', `Cofactor`='{3}' WHERE `EnzymaticReactionWID` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, EnzymaticReactionWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, PrimaryWID, AlternativeWID, Cofactor)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, PrimaryWID, AlternativeWID, Cofactor)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, EnzymaticReactionWID, PrimaryWID, AlternativeWID, Cofactor, EnzymaticReactionWID)
    End Function
#End Region
End Class


End Namespace
