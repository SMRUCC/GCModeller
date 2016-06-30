Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Extensions

Partial Module CLI

    ''' <summary>
    ''' 分析出仅含有一个结构域的蛋白质
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("pure_domain", Usage:="pure_domain -i <input_smart_log> -o <output_file>")>
    Public Function FiltePureDomain(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        'Dim SMARTLog As ModularArchitecture.SMARTLog = CommandLine("-i").LoadXml(Of ModularArchitecture.SMARTLog)()
        'Dim LQuery = From Protein In SMARTLog.Proteins Let Id = Protein.PureDomain Where Not String.IsNullOrEmpty(Id) Select New DomainTag With {.Id = Id, .Protein = Protein.Title}   '
        'Dim Result = LQuery.ToArray

        'Dim Xml = Result.GetXml
        'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), Xml, append:=False)

        'Return 0

        Throw New NotImplementedException
    End Function
End Module