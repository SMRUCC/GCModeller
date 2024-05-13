#Region "Microsoft.VisualBasic::d4b17f8ab0f96d5662a0f6a4e5e2d5a4, analysis\Motifs\PrimerDesigner\test\Program.vb"

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

    '   Total Lines: 40
    '    Code Lines: 30
    ' Comment Lines: 1
    '   Blank Lines: 9
    '     File Size: 1.31 KB


    ' Module Program
    ' 
    '     Sub: Main, searchTest, SlicerTest
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.PrimerDesigner.Restriction_enzyme
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Module Program

    Const test_demo As String = "\GCModeller\src\GCModeller\visualize\data\addgene-plasmid-100854-sequence-189713.gbk"

    Sub Main(args As String())
        ' Call searchTest()
        Call SlicerTest()
    End Sub

    Sub searchTest()
        Dim list = WikiLoader.PullAll.ToArray
        Dim plasmid As FastaSeq = GBFF.File.Load(test_demo).Origin.ToFasta
        Dim nt_scan As New Scanner(plasmid)
        Dim result As New List(Of SimpleSegment)

        For Each enzyme As Enzyme In list
            Dim motif = enzyme.TranslateRegular()
            Dim sites = motif.Scan(nt_scan)

            Call result.AddRange(sites)
        Next

        Pause()
    End Sub

    Sub SlicerTest()
        Dim plasmid As FastaSeq = GBFF.File.Load(test_demo).Origin.ToFasta
        Dim list = WikiLoader.PullAll.Shuffles.Take(5).ToArray
        Dim simulation As New Slicer(plasmid, list)
        Dim result = simulation.GetSegments.ToArray

        Pause()
    End Sub
End Module
