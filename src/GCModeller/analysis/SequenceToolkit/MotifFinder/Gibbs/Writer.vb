Imports System.IO
Imports ConsoleApp1.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace edu.illinois
    ' using Pair = javafx.util.Pair;


    Public Class Writer
        ''' <summary>
        ''' Writes all of the files that need to be written </summary>
        ''' <paramname="icpc">, information content per column </param>
        ''' <paramname="ml"> </param>
        ''' <paramname="sl"> </param>
        ''' <paramname="sc"> </param>
        ''' <paramname="fastaFileName"> </param>
        ''' <paramname="sitesFileName"> </param>
        ''' <paramname="motifFileName"> </param>
        ''' <paramname="motifLengthFileName"> </param>
        ''' <paramname="motif"> </param>
        ''' <paramname="bindingSites"> </param>
        ''' <paramname="plantedSequences"> </param>
        ''' <exceptioncref="FileNotFoundException"> </exception>
        ''' <exceptioncref="UnsupportedEncodingException"> </exception>
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
        ''' <paramname="motifs"> </param>
        ''' <paramname="sitePositions"> </param>
        ''' <paramname="filename"> </param>
        Public Shared Sub writeSites(sc As Integer, motifs As IList(Of String), sitePositions As IList(Of Integer), filename As String)
            Using printWriter As New StreamWriter(filename.Open(FileMode.OpenOrCreate, doClear:=True))
                Call Enumerable.Range(0, sc).ForEach(Sub(i, j) printWriter.WriteLine(String.Format("{0} {1:D}", motifs(i), sitePositions(i))))
            End Using
        End Sub

        ''' <summary>
        ''' Write down the motif that was generated. It should be stored in a format as
        ''' shown in the miniproj.pdf in step 8 </summary>
        ''' <paramname="motif"> </param>
        ''' <paramname="filename"> </param>
        Public Shared Sub writeMotif(ml As Integer, motif As WeightMatrix, filename As String, icpc As Double)
            Using printWriter As New StreamWriter(filename.Open(FileMode.OpenOrCreate, doClear:=True))
                printWriter.WriteLine(String.Format(">{0}_{1:F}" & vbTab & "{2:D}", filename, icpc, ml))
                printWriter.WriteLine(motif) 'not exactly right for debugging purposes
                printWriter.WriteLine("<")
            End Using
        End Sub

        ''' <summary>
        ''' Write down the motif length </summary>
        ''' <paramname="ml"> </param>
        ''' <paramname="filename"> </param>
        Private Shared Sub writeMotifLength(ml As Integer, filename As String)
            ml.ToString.SaveTo(filename)
        End Sub

        ''' <summary>
        ''' Writes a benchmark result to the output directory </summary>
        ''' <paramname="outputDirectory">, the directory to print the files in </param>
        ''' <paramname="benchmark">, name of benchmark </param>
        ''' <paramname="result">, result for file </param>
        Public Shared Sub writeResult(outputDirectory As String, benchmark As String, result As String)
            result.SaveTo(outputDirectory & "/" & benchmark.ToLower())
        End Sub


        ''' <summary>
        ''' Creates the directory where the files will be written </summary>
        ''' <paramname="filename">, path to a file that needs to be written </param>
        Private Shared Sub initOutputDirectory(filename As String)

        End Sub
    End Class

End Namespace
