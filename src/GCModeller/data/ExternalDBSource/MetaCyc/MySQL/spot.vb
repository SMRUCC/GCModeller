#Region "Microsoft.VisualBasic::bf8c3ea030e85dfdb1de49c5df912240, data\ExternalDBSource\MetaCyc\MySQL\spot.vb"

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

    ' Class spot
    ' 
    '     Properties: DatasetWID, MolecularWeightEst, PIEst, SpotId, WID
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `spot`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `spot` (
'''   `WID` bigint(20) NOT NULL,
'''   `SpotId` varchar(25) DEFAULT NULL,
'''   `MolecularWeightEst` float DEFAULT NULL,
'''   `PIEst` varchar(50) DEFAULT NULL,
'''   `DatasetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Spot` (`DatasetWID`),
'''   CONSTRAINT `FK_Spot` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("spot", Database:="warehouse", SchemaSQL:="
CREATE TABLE `spot` (
  `WID` bigint(20) NOT NULL,
  `SpotId` varchar(25) DEFAULT NULL,
  `MolecularWeightEst` float DEFAULT NULL,
  `PIEst` varchar(50) DEFAULT NULL,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Spot` (`DatasetWID`),
  CONSTRAINT `FK_Spot` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class spot: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("SpotId"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="SpotId")> Public Property SpotId As String
    <DatabaseField("MolecularWeightEst"), DataType(MySqlDbType.Double), Column(Name:="MolecularWeightEst")> Public Property MolecularWeightEst As Double
    <DatabaseField("PIEst"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="PIEst")> Public Property PIEst As String
    <DatabaseField("DatasetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DatasetWID")> Public Property DatasetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `spot` (`WID`, `SpotId`, `MolecularWeightEst`, `PIEst`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `spot` (`WID`, `SpotId`, `MolecularWeightEst`, `PIEst`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `spot` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `spot` SET `WID`='{0}', `SpotId`='{1}', `MolecularWeightEst`='{2}', `PIEst`='{3}', `DatasetWID`='{4}' WHERE `WID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `spot` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `spot` (`WID`, `SpotId`, `MolecularWeightEst`, `PIEst`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, SpotId, MolecularWeightEst, PIEst, DatasetWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{SpotId}', '{MolecularWeightEst}', '{PIEst}', '{DatasetWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `spot` (`WID`, `SpotId`, `MolecularWeightEst`, `PIEst`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, SpotId, MolecularWeightEst, PIEst, DatasetWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `spot` SET `WID`='{0}', `SpotId`='{1}', `MolecularWeightEst`='{2}', `PIEst`='{3}', `DatasetWID`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, SpotId, MolecularWeightEst, PIEst, DatasetWID, WID)
    End Function
#End Region
Public Function Clone() As spot
                  Return DirectCast(MyClass.MemberwiseClone, spot)
              End Function
End Class


End Namespace

