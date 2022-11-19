#Region "Microsoft.VisualBasic::ac53fb9cdea909c9cf10a77a329f6fed, GCModeller\analysis\Metagenome\MetaFunction\test\Program.vb"

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

    '   Total Lines: 39
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 1.33 KB


    ' Module Program
    ' 
    '     Sub: Main, testRead, testWrite
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt
Imports SMRUCC.genomics.Metagenomics

Module Program

    Dim dbfile As String = "D:\biodeep\bionovogene_health\metacolon\ko_13_5_precalculated.PICRUSt"

    Sub Main(args As String())
        Call testWrite()
        Call testRead()
    End Sub

    Sub testRead()
        Using file As New MetaBinaryReader(dbfile.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
            Dim demotest As String = "142199"
            Dim data = file.getByOTUId(demotest)
            Dim tax As Taxonomy = file.GetTaxonomy(demotest)

            Dim data2 = file.findByTaxonomy(tax)

            Call Console.WriteLine(data2.Keys.All(Function(id) data2(id) = data(id)))
        End Using
    End Sub

    Sub testWrite()
        Dim gg = otu_taxonomy.Load("F:\16s\greengenes\taxonomy\gg_13_8_99.gg.tax")

        Using file As Stream = dbfile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
            writer = MetaBinaryWriter.CreateWriter(gg, file)

            Dim raw = "D:\biodeep\bionovogene_health\metacolon\ko_13_5_precalculated.tab".Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Call writer.ImportsComputes(raw)

        End Using
    End Sub
End Module
