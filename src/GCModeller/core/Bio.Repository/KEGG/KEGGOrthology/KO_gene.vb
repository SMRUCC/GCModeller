#Region "Microsoft.VisualBasic::ec5e7632706f4bcae78ad579e0002f27, core\Bio.Repository\KEGG\KEGGOrthology\KO_gene.vb"

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

    ' Class KO_gene
    ' 
    '     Properties: gene, id, ko, name, sp_code
    '                 url
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `orthology_genes` (
'''   `ko` varchar(100) NOT NULL,
'''   `gene` varchar(100) NOT NULL,
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `url` text,
'''   `sp_code` varchar(45) DEFAULT NULL COMMENT 'The bacterial genome name brief code in KEGG database',
'''   `name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`gene`,`ko`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=9312 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
Public Class KO_gene

    Public Property ko As String
    Public Property gene As String
    Public Property id As Long
    Public Property url As String
    ''' <summary>
    ''' The bacterial genome name brief code in KEGG database
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property sp_code As String
    Public Property name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
