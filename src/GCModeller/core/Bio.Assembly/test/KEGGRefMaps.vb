#Region "Microsoft.VisualBasic::0b138a56b78952a92308962485c0d483, core\Bio.Assembly\test\KEGGRefMaps.vb"

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

    ' Module KEGGRefMaps
    ' 
    '     Sub: Main, ReactionXMLLayout
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Module KEGGRefMaps

    Sub Main()

        Call ReactionXMLLayout()

        Dim repo = MapRepository.BuildRepository("D:\KEGGMaps")

        Call repo.GetXml.SaveTo("./maps.XML")
    End Sub

    Sub ReactionXMLLayout()

        ReactionWebAPI.Download("R00002").GetXml.SaveTo("./test___reaction.xml")


        Pause()
    End Sub
End Module
