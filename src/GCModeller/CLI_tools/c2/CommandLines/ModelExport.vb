Partial Module CommandLines

    <CommandLine.Reflection.ExportAPI("-modelexport",
        info:="export a sbml file format model from a specific metacyc dabase.",
        usage:="-modelexport -i <metacyc_dir> -o <output_model_file>",
        Example:="")>
    Public Function ModelExport(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim MetaCyc As String = CommandLine("-i")
        Dim SbmlFile As String = CommandLine("-o")

        Using Export As c2.NetworkVisualization.SBML =
            New NetworkVisualization.SBML(MetaCyc:=LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCyc))
            Call Export.Export.GetXml.SaveTo(Path:=SbmlFile)
        End Using

        Return 0
    End Function
End Module