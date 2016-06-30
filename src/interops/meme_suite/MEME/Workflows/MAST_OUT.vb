Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Namespace Workflows

    Public Module MAST_OUT

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MASTLogFileDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Parse(MASTLogFileDir As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject()
            Dim Path As String = (MASTLogFileDir & "/mast.xml")
            Dim ObjectId As String = FileIO.FileSystem.GetDirectoryInfo(MASTLogFileDir).Name.Replace(".fsa", "")

            If Not FileIO.FileSystem.FileExists(Path) OrElse FileIO.FileSystem.GetFileInfo(Path).Length <= 100 Then
                Return {New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject From {ObjectId}}
            End If

            Dim XmlOutput As XmlOutput.MAST.MAST = Path.LoadXml(Of XmlOutput.MAST.MAST)()

            If XmlOutput.Sequences.SequenceList.IsNullOrEmpty Then
                Return {New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject From {ObjectId}}
            End If
            Dim list As List(Of RowObject) = New List(Of RowObject)
            For Each seq In XmlOutput.Sequences.SequenceList
                Dim regulatorId As String = seq.name
                If seq.Segments.IsNullOrEmpty Then
                    Continue For
                End If
                For Each hit In seq.Segments
                    For Each site In hit.Hits
                        Call list.Add(New String() {ObjectId, site.motif, site.pvalue, regulatorId, seq.Score.evalue})
                    Next
                Next
            Next
            Return list.ToArray
        End Function

        Public Function Prcedure(Dir As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim LQuery = (From subDir As String In FileIO.FileSystem.GetDirectories(Dir)
                          Let data = Parse(subDir)
                          Where Not data.IsNullOrEmpty
                          Select data).ToArray
            Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
                New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Call File.AppendLine(New String() {"ObjectId", "MotifId", "p-value", "tbfsId", "E-value"})
            For Each DataRows In LQuery
                Call File.AppendRange(DataRows)
            Next

            Return File
        End Function

        Public Function Action(dir As String)
#If DEBUG Then
            For Each subdir As String In FileIO.FileSystem.GetDirectories(dir)
#Else
  Parallel.ForEach(FileIO.FileSystem.GetDirectories(dir), Sub(subdir As String)
#End If
                Dim di = FileIO.FileSystem.GetDirectoryInfo(subdir)
                Dim path = String.Format("{0}/{1}.csv", di.Parent.FullName, di.Name)
                Dim File = Prcedure(subdir)
                Dim LQuery = (From row In File Let s = row.AsLine Select s Distinct).ToArray

                Call IO.File.WriteAllLines(path, LQuery)
#If DEBUG Then
            Next
#Else
              End Sub)
#End If
            Return Nothing
        End Function
    End Module
End Namespace