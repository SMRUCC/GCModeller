#Region "Microsoft.VisualBasic::274035a40af04ab09e8d87c76775b0b0, data\ExternalDBSource\MetaCyc\bio_warehouse\taxon.vb"

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

    ' Class taxon
    ' 
    '     Properties: DataSetWID, DivisionWID, GencodeWID, InheritedDivision, InheritedGencode
    '                 InheritedMCGencode, MCGencodeWID, Name, ParentWID, Rank
    '                 WID
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
''' DROP TABLE IF EXISTS `taxon`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxon` (
'''   `WID` bigint(20) NOT NULL,
'''   `ParentWID` bigint(20) DEFAULT NULL,
'''   `Name` varchar(100) DEFAULT NULL,
'''   `Rank` varchar(100) DEFAULT NULL,
'''   `DivisionWID` bigint(20) DEFAULT NULL,
'''   `InheritedDivision` char(1) DEFAULT NULL,
'''   `GencodeWID` bigint(20) DEFAULT NULL,
'''   `InheritedGencode` char(1) DEFAULT NULL,
'''   `MCGencodeWID` bigint(20) DEFAULT NULL,
'''   `InheritedMCGencode` char(1) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Taxon_Division` (`DivisionWID`),
'''   KEY `FK_Taxon_GeneticCode` (`GencodeWID`),
'''   KEY `FK_Taxon` (`DataSetWID`),
'''   CONSTRAINT `FK_Taxon` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Taxon_Division` FOREIGN KEY (`DivisionWID`) REFERENCES `division` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Taxon_GeneticCode` FOREIGN KEY (`GencodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxon", Database:="warehouse")>
Public Class taxon: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("ParentWID"), DataType(MySqlDbType.Int64, "20")> Public Property ParentWID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "100")> Public Property Name As String
    <DatabaseField("Rank"), DataType(MySqlDbType.VarChar, "100")> Public Property Rank As String
    <DatabaseField("DivisionWID"), DataType(MySqlDbType.Int64, "20")> Public Property DivisionWID As Long
    <DatabaseField("InheritedDivision"), DataType(MySqlDbType.VarChar, "1")> Public Property InheritedDivision As String
    <DatabaseField("GencodeWID"), DataType(MySqlDbType.Int64, "20")> Public Property GencodeWID As Long
    <DatabaseField("InheritedGencode"), DataType(MySqlDbType.VarChar, "1")> Public Property InheritedGencode As String
    <DatabaseField("MCGencodeWID"), DataType(MySqlDbType.Int64, "20")> Public Property MCGencodeWID As Long
    <DatabaseField("InheritedMCGencode"), DataType(MySqlDbType.VarChar, "1")> Public Property InheritedMCGencode As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxon` (`WID`, `ParentWID`, `Name`, `Rank`, `DivisionWID`, `InheritedDivision`, `GencodeWID`, `InheritedGencode`, `MCGencodeWID`, `InheritedMCGencode`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxon` (`WID`, `ParentWID`, `Name`, `Rank`, `DivisionWID`, `InheritedDivision`, `GencodeWID`, `InheritedGencode`, `MCGencodeWID`, `InheritedMCGencode`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxon` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxon` SET `WID`='{0}', `ParentWID`='{1}', `Name`='{2}', `Rank`='{3}', `DivisionWID`='{4}', `InheritedDivision`='{5}', `GencodeWID`='{6}', `InheritedGencode`='{7}', `MCGencodeWID`='{8}', `InheritedMCGencode`='{9}', `DataSetWID`='{10}' WHERE `WID` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, ParentWID, Name, Rank, DivisionWID, InheritedDivision, GencodeWID, InheritedGencode, MCGencodeWID, InheritedMCGencode, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, ParentWID, Name, Rank, DivisionWID, InheritedDivision, GencodeWID, InheritedGencode, MCGencodeWID, InheritedMCGencode, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, ParentWID, Name, Rank, DivisionWID, InheritedDivision, GencodeWID, InheritedGencode, MCGencodeWID, InheritedMCGencode, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
