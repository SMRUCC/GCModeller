#Region "Microsoft.VisualBasic::d19b563cc5005b435f2d5c0c38859ac7, analysis\SequenceToolkit\MotifFinder\Gibbs\Writer.vb"

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

    '   Total Lines: 80
    '    Code Lines: 33 (41.25%)
    ' Comment Lines: 38 (47.50%)
    '    - Xml Docs: 94.74%
    ' 
    '   Blank Lines: 9 (11.25%)
    '     File Size: 4.00 KB


    ' Class Writer
    ' 
    '     Sub: initOutputDirectory, writeMotif, writeMotifLength, writeResult, writeSequenceInfo
    '          writeSites
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class Writer

    ''' <summary>
    ''' Writes all of the files that need to be written </summary>
    ''' <param name="icpc">, information content per column </param>
    ''' <param name="ml"> </param>
    ''' <param name="sl"> </param>
    ''' <param name="sc"> </param>
    ''' <param name="fastaFileName"> </param>
    ''' <param name="sitesFileName"> </param>
    ''' <param name="motifFileName"> </param>
    ''' <param name="motifLengthFileName"> </param>
    ''' <param name="motif"> </param>
    ''' <param name="bindingSites"> </param>
    ''' <param name="plantedSequences"> </param>
    ''' <exception cref="FileNotFoundException"> </exception>
    Public Shared Sub writeSequenceInfo(icpc As Double, ml As Integer, sl As Integer, sc As Integer, fastaFileName As String, sitesFileName As String, motifFileName As String, motifLengthFileName As String,
                                            motif As WeightMatrix, bindingSites As IList(Of String), plantedSequences As IList(Of KeyValuePair(Of String, Integer)))
        'Create the output directory
        initOutputDirectory(fastaFileName)

        'Write files in the new directory
        Utils.getSequenceFromPair(plantedSequences).writeFasta(fastaFileName)
        writeSites(sc, bindingSites, Utils.getSiteFromPair(plantedSequences), sitesFileName)
        writeMotif(ml, motif, motifFileName, icpc)
        writeMotifLength(ml, motifLengthFileName)
    End Sub

    ''' <summary>
    ''' Use any format for writing down the location of the planted site in each sequence </summary>
    ''' <param name="motifs"> </param>
    ''' <param name="sitePositions"> </param>
    ''' <param name="filename"> </param>
    Public Shared Sub writeSites(sc As Integer, motifs As IList(Of String), sitePositions As IList(Of Integer), filename As String)
        Using printWriter As New IO.StreamWriter(filename.Open(FileMode.OpenOrCreate, doClear:=True))
            Call Enumerable.Range(0, sc).ForEach(Sub(i, j) printWriter.WriteLine(String.Format("{0} {1:D}", motifs(i), sitePositions(i))))
        End Using
    End Sub

    ''' <summary>
    ''' Write down the motif that was generated. It should be stored in a format as
    ''' shown in the miniproj.pdf in step 8 </summary>
    ''' <param name="motif"> </param>
    ''' <param name="filename"> </param>
    Public Shared Sub writeMotif(ml As Integer, motif As WeightMatrix, filename As String, icpc As Double)
        Using printWriter As New IO.StreamWriter(filename.Open(FileMode.OpenOrCreate, doClear:=True))
            printWriter.WriteLine(String.Format(">{0}_{1:F}" & vbTab & "{2:D}", filename, icpc, ml))
            printWriter.WriteLine(motif) 'not exactly right for debugging purposes
            printWriter.WriteLine("<")
        End Using
    End Sub

    ''' <summary>
    ''' Write down the motif length </summary>
    ''' <param name="ml"> </param>
    ''' <param name="filename"> </param>
    Private Shared Sub writeMotifLength(ml As Integer, filename As String)
        ml.ToString.SaveTo(filename)
    End Sub

    ''' <summary>
    ''' Writes a benchmark result to the output directory </summary>
    ''' <param name="outputDirectory">, the directory to print the files in </param>
    ''' <param name="benchmark">, name of benchmark </param>
    ''' <param name="result">, result for file </param>
    Public Shared Sub writeResult(outputDirectory As String, benchmark As String, result As String)
        result.SaveTo(outputDirectory & "/" & benchmark.ToLower())
    End Sub


    ''' <summary>
    ''' Creates the directory where the files will be written </summary>
    ''' <param name="filename">, path to a file that needs to be written </param>
    Private Shared Sub initOutputDirectory(filename As String)

    End Sub
End Class
