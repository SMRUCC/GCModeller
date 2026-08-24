#Region "Microsoft.VisualBasic::fb5fa3d89d76b68f2fce614227af62df, core\test\KEGGenzymeTest.vb"

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

    '   Total Lines: 31
    '    Code Lines: 21 (67.74%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (32.26%)
    '     File Size: 915 B


    ' Module KEGGenzymeTest
    ' 
    '     Sub: embeddingTest, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Module KEGGenzymeTest

    Sub Main()
        Call embeddingTest()
        Dim kgml = "D:\GCModeller\src\GCModeller\core\data\ko02060.xml".LoadXml(Of KGML.pathway)

        Dim kolist = kgml.KOlist



        Pause()

        Dim tree As htext = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.GetResource
        Dim entries = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.ParseEntries

        Pause()
    End Sub

    Sub embeddingTest()
        Dim pathways As New MetabolicEmbedding
        Dim list = New String() {"1.1.1.1", "2.1.3.1", "1.5.2.1", "1.1.1.2", "1.1.1.3"}
        Dim vec = pathways.MakeVector(list)

        Pause()
    End Sub
End Module
