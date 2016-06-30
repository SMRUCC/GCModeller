Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    '<Command("export_hits", usage:="export_hits -i <input_dir> -o <output_file> -fsa <fasta_file>")>
    'Public Shared Function ExportHits(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
    '    Dim XmlLogDir As String = CommandLine("-i")
    '    Dim SavedFile As String = CommandLine("-o")
    '    Dim FASTA As SequenceModel.FASTA.File = SequenceModel.FASTA.File.Read(CommandLine("-fsa"))
    '    Dim Hits As List(Of String) = New List(Of String)
    '    Dim LQuery = (From Log In FileIO.FileSystem.GetFiles(XmlLogDir, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
    '                  Let XmlLog = NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.Load(Log)
    '                  Select Hits.Append(XmlLog.GetAllits())) '
    '    LQuery = LQuery.ToArray
    '    Hits = (From s As String In Hits Select s Distinct Order By s Ascending).ToList

    '    Dim FASTAList As List(Of SequenceModel.FASTA.FASTA) = New List(Of SequenceModel.FASTA.FASTA)
    '    LQuery = From Id In Hits Select FASTAList.Append(FASTA.Query(Id, CompareMethod.Binary)) '
    '    LQuery = LQuery.ToArray
    '    FASTA = FASTAList

    '    Call FASTA.Save(SavedFile)
    '    Return 0
    'End Function
End Module