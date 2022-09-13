#Region "Microsoft.VisualBasic::48b138a7b404aa92aa56995ed2005d70, GCModeller\data\MicrobesOnline\MySQL\genomics\pdbreps.vb"

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

    '   Total Lines: 65
    '    Code Lines: 36
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 3.25 KB


    ' Class pdbreps
    ' 
    '     Properties: pdbChain, pdbChainRep, pdbId, pdbIdRep
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `pdbreps`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pdbreps` (
'''   `pdbIdRep` varchar(6) NOT NULL DEFAULT '',
'''   `pdbChainRep` char(1) NOT NULL DEFAULT '',
'''   `pdbId` varchar(6) NOT NULL DEFAULT '',
'''   `pdbChain` char(1) NOT NULL DEFAULT '',
'''   PRIMARY KEY (`pdbIdRep`,`pdbChainRep`,`pdbId`,`pdbChain`),
'''   KEY `pdbIdRep` (`pdbIdRep`,`pdbChainRep`),
'''   KEY `pdbId` (`pdbId`,`pdbChain`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdbreps")>
Public Class pdbreps: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdbIdRep"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property pdbIdRep As String
    <DatabaseField("pdbChainRep"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property pdbChainRep As String
    <DatabaseField("pdbId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property pdbId As String
    <DatabaseField("pdbChain"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property pdbChain As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdbreps` (`pdbIdRep`, `pdbChainRep`, `pdbId`, `pdbChain`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdbreps` (`pdbIdRep`, `pdbChainRep`, `pdbId`, `pdbChain`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdbreps` WHERE `pdbIdRep`='{0}' and `pdbChainRep`='{1}' and `pdbId`='{2}' and `pdbChain`='{3}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdbreps` SET `pdbIdRep`='{0}', `pdbChainRep`='{1}', `pdbId`='{2}', `pdbChain`='{3}' WHERE `pdbIdRep`='{4}' and `pdbChainRep`='{5}' and `pdbId`='{6}' and `pdbChain`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdbIdRep, pdbChainRep, pdbId, pdbChain)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdbIdRep, pdbChainRep, pdbId, pdbChain)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdbIdRep, pdbChainRep, pdbId, pdbChain)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdbIdRep, pdbChainRep, pdbId, pdbChain, pdbIdRep, pdbChainRep, pdbId, pdbChain)
    End Function
#End Region
End Class


End Namespace
