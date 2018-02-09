Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language

Partial Module CLI

    ''' <summary>
    ''' ```
    ''' 0  1      2
    ''' tr|Q3KBE6|Q3KBE6_PSEPF
    ''' ```
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniProt.IDs")>
    <Usage("/UniProt.IDs /in <list.csv/txt> [/out <list.txt>]")>
    Public Function UniProtIDList(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.uniprotIDs.txt"
        Dim list$()

        With [in].ExtensionSuffix
            If .TextEquals("csv") Then
                list = EntityObject.LoadDataSet([in]).Keys
            ElseIf .TextEquals("txt") Then
                list = [in].ReadAllLines
            Else
                Throw New NotImplementedException(.ByRef)
            End If
        End With

        Return list _
            .Select(Function(id) id.Split("|"c)(1)) _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function
End Module