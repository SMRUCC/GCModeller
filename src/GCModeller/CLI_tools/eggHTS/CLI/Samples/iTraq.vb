Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.HTS.Proteomics

Partial Module CLI

    <ExportAPI("/iTraq.Sign.Replacement")>
    <Usage("/iTraq.Sign.Replacement /in <iTraq.data.csv> /symbols <signs.csv> [/out <out.DIR>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(iTraqReader)},
              Description:="")>
    <Argument("/symbols", False, CLITypes.File,
              AcceptTypes:={GetType(iTraqSigns)},
              Description:="Using for replace the mass spectrum expeirment symbol to the user experiment tag.")>
    Public Function iTraqSignReplacement(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim symbols$ = args <= "/symbols"
        Dim out$ = args.GetValue("/out", [in].ParentPath)


    End Function
End Module