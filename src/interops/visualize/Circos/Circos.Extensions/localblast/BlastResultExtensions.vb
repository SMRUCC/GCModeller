#Region "Microsoft.VisualBasic::de5fcccc32a3c68ecadd6ad60ec16ab7, visualize\Circos\Circos.Extensions\localblast\BlastResultExtensions.vb"

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

    ' Module BlastResultExtensions
    ' 
    '     Function: __createGenomeCircle, AddGeneInfoTrack, GenerateBlastnAlignment, GetGenomeCircle
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype.GeneObjects
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Public Module BlastResultExtensions

    ''' <summary>
    ''' The blast result alignment will be mapping on the circos plot circle individual as the 
    ''' highlights element in the circos plot.
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <param name="table">
    ''' The ncbi blast alignment result table object which can be achive from the NCBI website.
    ''' </param>
    ''' <param name="r1">The max radius of the alignment circles.</param>
    ''' <param name="rInner"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Add.Blast_alignment", Info:="The blast result alignment will be mapping on the circos plot circle individual as the highlights element in the circos plot.")>
    Public Function GenerateBlastnAlignment(doc As Configurations.Circos,
                                            <Parameter("Table", "The ncbi blast alignment result table object which can be achive from the NCBI website.")>
                                            table As AlignmentTable,
                                            <Parameter("r.Max", "The max radius of the alignment circles.")>
                                            r1 As Double,
                                            <Parameter("r.Inner")>
                                            rInner As Double,
                                            Optional Color As IdentityColors = Nothing) As Configurations.Circos

        Dim alignment = (From hit As HitRecord
                         In table.Hits
                         Select hit
                         Group By hit.SubjectIDs Into Group).ToArray
        Dim d As Double = Math.Abs(r1 - rInner) / alignment.Length
        Dim Colors As String() =
            CircosColor.AllCircosColors.Shuffles
        Dim i As Integer = 0

        For Each genome In alignment
            Dim Document As New BlastMaps(genome.Group.ToArray, Colors(i), Color)
            Dim PlotElement As New HighLight(Document)

            Call doc.AddTrack(PlotElement)

            PlotElement.r1 = $"{r1}r"
            r1 -= d
            PlotElement.r0 = $"{r1}r"
            i += 2
        Next

        Return doc
    End Function

    ''' <summary>
    ''' The directory which contains the completed PTT data: ``*.ptt, *.rnt, *.fna``
    ''' and so on which you can download from the NCBI FTP website.
    ''' </summary>
    ''' <param name="PTT">
    ''' The directory which contains the completed PTT data: *.ptt, *.rnt, *.fna and so on which you can download from the NCBI FTP website.
    ''' </param>
    ''' <param name="myvaCog">
    ''' The csv file path of the myva cog value which was export from the alignment between
    ''' the bacteria genome And the myva cog database Using the NCBI blast package In the GCModeller.
    ''' </param>
    ''' <param name="defaultColor">
    ''' The default color of the gene which is not assigned to any COG will be have.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle")>
    Public Function GetGenomeCircle(<Parameter("Dir.PTT",
                                               "The directory which contains the completed PTT data: *.ptt, *.rnt, *.fna and so on which you can download from the NCBI FTP website.")>
                                    PTT As String,
                                    <Parameter("COG.Myva", "The csv file path of the myva cog value which was export from the alignment between
                                    the bacteria genome and the myva cog database using the NCBI blast package in the GCModeller.")>
                                    Optional myvaCog As String = "",
                                    <Parameter("Color.Default", "The default color of the gene which is not assigned to any COG will be have.")>
                                    Optional defaultColor As String = "blue") As PTTMarks

        Dim pttDB As New PTTDbLoader(PTT)

        If String.IsNullOrEmpty(myvaCog) OrElse Not FileIO.FileSystem.FileExists(myvaCog) Then
            Return New PTTMarks(pttDB, Nothing, defaultColor)
        Else
            Dim Myva = myvaCog.LoadCsv(Of MyvaCOG)(False).ToArray
            Return __createGenomeCircle(pttDB, Myva, defaultColor)
        End If
    End Function

    ''' <summary>
    ''' Creates the circos gene circle from the PTT database which is defined 
    ''' in the ``*.ptt/*.rnt`` file, and you can download this directory from 
    ''' the NCBI FTP website.
    ''' </summary>
    ''' <param name="PTT"></param>
    ''' <param name="COG"></param>
    ''' <param name="defaultColor"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle.From.Objects",
               Info:="Creates the circos gene circle from the PTT database which is defined in the *.ptt/*.rnt file, and you can download this directory from the NCBI FTP website.")>
    Public Function __createGenomeCircle(PTT As PTTDbLoader, COG As IEnumerable(Of MyvaCOG), Optional defaultColor As String = "blue") As PTTMarks
        Dim Data As New PTTMarks(PTT, COG.ToArray, defaultColor)
        Return Data
    End Function

    <ExportAPI("Plots.add.genes_track")>
    <Extension>
    Public Function AddGeneInfoTrack(circos As Configurations.Circos,
                                    gbk As GenBank.GBFF.File,
                                   COGs As IEnumerable(Of MyvaCOG),
                 Optional splitOverlaps As Boolean = False,
                 Optional dumpAll As Boolean = False) As Configurations.Circos

        Dim dump As GeneTable() = FeatureDumps(gbk, dumpAll:=dumpAll)
        Dim hash = (From x As MyvaCOG
                    In COGs
                    Select x
                    Group x By x.QueryName Into Group) _
                        .ToDictionary(Function(x) x.QueryName,
                                      Function(x)
                                          Return x.Group.First
                                      End Function)
        For Each gene As GeneTable In dump
            If hash.ContainsKey(gene.locus_id) Then
                gene.COG = hash(gene.locus_id).COG
            End If
        Next

        Return GenerateGeneCircle(circos, dump, splitOverlaps:=splitOverlaps)
    End Function
End Module
