#Region "Microsoft.VisualBasic::d2ef6d41800bc67b2318aa438f3b3328, models\SBML\Biopax\test\Program.vb"

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

    '   Total Lines: 30
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 972 B


    ' Module Program
    ' 
    '     Sub: graph_test, Main, readerTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.Level3
Imports SMRUCC.genomics.Model.SBML.SBGN

Module Program

    Sub Main(args As String())
        Call graph_test()
    End Sub

    Sub graph_test()
        Dim layout = sbgnFile.ReadXml("E:\GCModeller\src\GCModeller\models\SBML\data\R-HSA-211945.sbgn")

        Call Console.WriteLine(layout.GetJson)
        Call layout.GetJson.SaveTo("E:\GCModeller\src\GCModeller\models\SBML\data\R-HSA-211945.json")

        Pause()
    End Sub

    Sub readerTest()
        Dim docs = "D:\GCModeller\src\GCModeller\models\SBML\data\smpdb_PW000266.owl"
        Dim pathway = File.LoadDoc(docs)
        Dim loader = ResourceReader.LoadResource(pathway)
        Dim reactions As MetabolicReaction() = loader.GetAllReactions.ToArray
        Dim compounds = loader.GetAllCompounds.ToArray

        Pause()
    End Sub
End Module
