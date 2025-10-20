#Region "Microsoft.VisualBasic::69d17bf4732f69b4fbf3a8974cc28500, localblast\venn\Files.vb"

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

    '     Module Files
    ' 
    '         Function: __innerCopy
    ' 
    '         Sub: SelectCopy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.ParallelTask

Namespace BlastAPI

    Public Module Files

        ''' <summary>
        ''' There are some times the bbh data source size is too large for the bi-directionary best hit blastp, 
        ''' so that you needs to select the genome source first, using this method by the besthit method to 
        ''' filtering the raw data.
        ''' (可能有时候需要进行两两双向比对的数据太多了，故而需要先进行单向比对，在使用这个函数将原数据拷贝出来之后，
        ''' 再进行单向必对)
        ''' </summary>
        ''' <param name="BlastoutputSource"></param>
        ''' <param name="EXPORT"></param>
        ''' <remarks></remarks>
        ''' <param name="trimValue">默认是匹配上60%个Query基因组的基因数目</param>
        <ExportAPI("Source.Copy",
               Info:="There are some times the bbh data source size is too large for the bi-directionary best hit blastp, so that you needs to select the genome source first, using this method by the besthit method to filtering the raw data.")>
        Public Sub SelectCopy(<Parameter("DIR.Blastoutput", "The directory which contains the blast output data for calculates the best hit data.")>
                          BlastoutputSource$,
                          <Parameter("DIR.Orf_Source", "The directory which cointains the orf original raw data for the bbh test.")>
                          CopySource$,
                          <Parameter("DIR.EXPORT", "The directory which is the destination directory for copying the genome data, this data ")>
                          EXPORT$,
                          Optional identities# = 0.3,
                          <Parameter("Percentage.Trim", "Default is the 60% of the number of the query genome proteins.")>
                          Optional trimValue# = 0.6)

            Dim LQuery = From path As KeyValuePair(Of String, String)
                     In BlastoutputSource.LoadSourceEntryList({"*.txt"}).AsParallel
                         Let Blastout = BlastPlus.LoadBlastOutput(path.Value)
                         Where Not Blastout Is Nothing
                         Select ID = VennDataBuilder.LogNameParser(path.Value).HitName,
                         Blastout,
                         Besthits = Blastout.ExportAllBestHist(identities) ' 加载单向最佳比对

            Call "Blast output data parsing job done, start to screening the besthit genomes.....".debug

            Dim selecteds = (From genome
                         In LQuery.AsParallel
                             Let screenedData = (From x In genome.Besthits Where x.isMatched Select x).ToArray
                             Where Not screenedData.IsNullOrEmpty    ' 筛选出符合条件的基因组
                             Select Besthits = screenedData,
                             genome.ID,
                             genome.Blastout).ToArray

            Call $"Screening {selecteds.Length} genomes at first time!".debug

            Dim n As Integer = selecteds.First.Blastout.Queries.Length * trimValue
            selecteds = (From genome
                     In selecteds
                         Where genome.Besthits.Length >= n
                         Select genome).ToArray

            Call $"Screening {selecteds.Length} genomes at second time which contains at least {n} besthit proteins.".debug

            Dim LoadORFres = (From path As KeyValuePair(Of String, String)
                          In CopySource.LoadSourceEntryList({"*.fasta", "*.fsa", "*.fa"})
                              Select genomeID = path.Key,
                              pathValue = path.Value
                              Group By genomeID Into Group) _
                             .ToDictionary(Function(g) g.genomeID,
                                           Function(g) g.Group.Select(Function(gp) gp.pathValue).ToArray)

            Call (From genome In selecteds Select genome.Besthits).IteratesALL.SaveTo(EXPORT & "/Besthits.csv", False)
            Call "Start to copy genome proteins data...".debug
            Call (From genome In selecteds Select __innerCopy(LoadORFres, EXPORT, genome.ID)).ToArray
            Call "Job done!".debug
        End Sub

        Private Function __innerCopy(loadORFres As Dictionary(Of String, String()), EXPORT$, genomeID$) As Boolean
            If Not loadORFres.ContainsKey(genomeID) Then
                Call $"------------------------------{genomeID} is not exists in the data source!-------------------------".Warning
                Return False
            End If

            Dim list As String() = loadORFres(genomeID)
            Dim CopyTo As String = EXPORT & "/" & FileIO.FileSystem.GetFileInfo(list.First).Name

            Try
                Call FileIO.FileSystem.CopyFile(list.First, CopyTo)
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function
    End Module
End Namespace
