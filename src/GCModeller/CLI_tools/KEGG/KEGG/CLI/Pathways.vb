Imports LANS.SystemsBiology.Assembly.KEGG.Archives.Xml
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("/Compile.Model", Info:="KEGG pathway model compiler",
               Usage:="/Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]")>
    Public Function Compile(args As CommandLine.CommandLine) As Integer
        Dim pwyDIR As String = args("/pathway")
        Dim modDIR As String = args("/mods")
        Dim sp As String = args("/sp")
        Dim reactions As String = GCModeller.FileSystem.KEGG.GetReactions
        Dim out As String = args.GetValue("/out", pwyDIR & "." & sp & "_KEGG.xml")
        Dim model As XmlModel = CompilerAPI.Compile(
            KEGGPathways:=pwyDIR,
            KEGGModules:=modDIR,
            KEGGReactions:=reactions,
            speciesCode:=sp)
        Return model.SaveAsXml(out).CLICode
    End Function
End Module