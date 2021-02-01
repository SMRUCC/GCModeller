#Region "Microsoft.VisualBasic::361cba810d2d79fb2b2cc406e0111c35, core\test\KEGGenzymeTest.vb"

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

    ' Module KEGGenzymeTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Module KEGGenzymeTest

    Sub Main()

        Dim kgml = "D:\GCModeller\src\GCModeller\core\data\ko02060.xml".LoadXml(Of KGML.pathway)

        Dim kolist = kgml.KOlist



        Pause()

        Dim tree As htext = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.GetResource
        Dim entries = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.ParseEntries

        Pause()
    End Sub
End Module
