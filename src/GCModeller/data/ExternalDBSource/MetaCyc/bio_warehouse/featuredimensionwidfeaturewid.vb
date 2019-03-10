#Region "Microsoft.VisualBasic::b6949303ced587a0fbabe648ca613566, data\ExternalDBSource\MetaCyc\bio_warehouse\featuredimensionwidfeaturewid.vb"

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

    ' Class featuredimensionwidfeaturewid
    ' 
    '     Properties: FeatureDimensionWID, FeatureWID
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
''' DROP TABLE IF EXISTS `featuredimensionwidfeaturewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `featuredimensionwidfeaturewid` (
'''   `FeatureDimensionWID` bigint(20) NOT NULL,
'''   `FeatureWID` bigint(20) NOT NULL,
'''   KEY `FK_FeatureDimensionWIDFeatur1` (`FeatureDimensionWID`),
'''   KEY `FK_FeatureDimensionWIDFeatur2` (`FeatureWID`),
'''   CONSTRAINT `FK_FeatureDimensionWIDFeatur1` FOREIGN KEY (`FeatureDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureDimensionWIDFeatur2` FOREIGN KEY (`FeatureWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("featuredimensionwidfeaturewid", Database:="warehouse")>
Public Class featuredimensionwidfeaturewid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("FeatureDimensionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property FeatureDimensionWID As Long
    <DatabaseField("FeatureWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property FeatureWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `featuredimensionwidfeaturewid` (`FeatureDimensionWID`, `FeatureWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `featuredimensionwidfeaturewid` (`FeatureDimensionWID`, `FeatureWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `featuredimensionwidfeaturewid` WHERE `FeatureDimensionWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `featuredimensionwidfeaturewid` SET `FeatureDimensionWID`='{0}', `FeatureWID`='{1}' WHERE `FeatureDimensionWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, FeatureDimensionWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, FeatureDimensionWID, FeatureWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, FeatureDimensionWID, FeatureWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, FeatureDimensionWID, FeatureWID, FeatureDimensionWID)
    End Function
#End Region
End Class


End Namespace
