'#Region "Microsoft.VisualBasic::ea9b595cfcd807a837c97b973e69fd74, GCModeller\core\Bio.Assembly\Test\KEGGRefMaps.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:


'    ' Code Statistics:

'    '   Total Lines: 33
'    '    Code Lines: 17
'    ' Comment Lines: 1
'    '   Blank Lines: 15
'    '     File Size: 786 B


'    ' Module KEGGRefMaps
'    ' 
'    '     Sub: Main, ReactionXMLLayout
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.Imaging
'Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
'Imports SMRUCC.genomics.Assembly.KEGG.WebServices

'Module KEGGRefMaps

'    Sub Main()



'        '  Pause()

'        Call Map.ParseHTML("http://www.kegg.jp/kegg-bin/show_pathway?map=map00430&show_description=show").GetXml.SaveTo("./testMap.XML")

'        Call "./testMap.XML".LoadXml(Of Map).GetImage.SaveAs("./ddddddddddddddddd.png")

'        Pause()

'        Call ReactionXMLLayout()

'        Dim repo = MapRepository.BuildRepository("D:\KEGGMaps")

'        Call repo.GetXml.SaveTo("./maps.XML")
'    End Sub

'    Sub ReactionXMLLayout()

'        ReactionWebAPI.Download("R00002").GetXml.SaveTo("./test___reaction.xml")


'        Pause()
'    End Sub
'End Module
