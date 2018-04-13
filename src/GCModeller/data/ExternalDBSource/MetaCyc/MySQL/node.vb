#Region "Microsoft.VisualBasic::1ec030e8c1f4969f97adb00d9d887f32, data\ExternalDBSource\MetaCyc\MySQL\node.vb"

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

    ' Class node
    ' 
    '     Properties: BioAssayDataCluster_Nodes, DataSetWID, Node_Nodes, WID
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
''' DROP TABLE IF EXISTS `node`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `node` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `BioAssayDataCluster_Nodes` bigint(20) DEFAULT NULL,
'''   `Node_Nodes` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Node1` (`DataSetWID`),
'''   KEY `FK_Node3` (`BioAssayDataCluster_Nodes`),
'''   KEY `FK_Node4` (`Node_Nodes`),
'''   CONSTRAINT `FK_Node1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Node3` FOREIGN KEY (`BioAssayDataCluster_Nodes`) REFERENCES `bioassaydatacluster` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Node4` FOREIGN KEY (`Node_Nodes`) REFERENCES `node` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("node", Database:="warehouse", SchemaSQL:="
CREATE TABLE `node` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioAssayDataCluster_Nodes` bigint(20) DEFAULT NULL,
  `Node_Nodes` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Node1` (`DataSetWID`),
  KEY `FK_Node3` (`BioAssayDataCluster_Nodes`),
  KEY `FK_Node4` (`Node_Nodes`),
  CONSTRAINT `FK_Node1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Node3` FOREIGN KEY (`BioAssayDataCluster_Nodes`) REFERENCES `bioassaydatacluster` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Node4` FOREIGN KEY (`Node_Nodes`) REFERENCES `node` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class node: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("BioAssayDataCluster_Nodes"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayDataCluster_Nodes")> Public Property BioAssayDataCluster_Nodes As Long
    <DatabaseField("Node_Nodes"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Node_Nodes")> Public Property Node_Nodes As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `node` (`WID`, `DataSetWID`, `BioAssayDataCluster_Nodes`, `Node_Nodes`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `node` (`WID`, `DataSetWID`, `BioAssayDataCluster_Nodes`, `Node_Nodes`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `node` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `node` SET `WID`='{0}', `DataSetWID`='{1}', `BioAssayDataCluster_Nodes`='{2}', `Node_Nodes`='{3}' WHERE `WID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `node` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `node` (`WID`, `DataSetWID`, `BioAssayDataCluster_Nodes`, `Node_Nodes`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, BioAssayDataCluster_Nodes, Node_Nodes)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{BioAssayDataCluster_Nodes}', '{Node_Nodes}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `node` (`WID`, `DataSetWID`, `BioAssayDataCluster_Nodes`, `Node_Nodes`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, BioAssayDataCluster_Nodes, Node_Nodes)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `node` SET `WID`='{0}', `DataSetWID`='{1}', `BioAssayDataCluster_Nodes`='{2}', `Node_Nodes`='{3}' WHERE `WID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, BioAssayDataCluster_Nodes, Node_Nodes, WID)
    End Function
#End Region
Public Function Clone() As node
                  Return DirectCast(MyClass.MemberwiseClone, node)
              End Function
End Class


End Namespace
