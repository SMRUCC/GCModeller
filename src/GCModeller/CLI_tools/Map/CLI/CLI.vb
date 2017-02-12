Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ChromosomeMap
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Public Module CLI

    <ExportAPI("--Drawing.ChromosomeMap",
               Info:="Drawing the chromosomes map from the PTT object as the basically genome information source.",
               Usage:="--Drawing.ChromosomeMap /ptt <genome.ptt> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]")>
    <Argument("/COG", True, CLITypes.File,
              AcceptTypes:={GetType(MyvaCOG)},
              Description:="The gene object color definition, you can using this parameter to overrides the cog definition in the PTT file.")>
    Public Function DrawingChrMap(args As CommandLine) As Integer
        Dim PTT = args.GetObject("/ptt", AddressOf TabularFormat.PTT.Load)
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim confInf As String = args.GetValue("/conf", out & "/config.inf")
        Dim config As ChromosomeMap.Configurations

        If Not confInf.FileExists Then
Create:     config = ChromosomeMap.GetDefaultConfiguration(confInf)
        Else
            Try
                config = ChromosomeMap.LoadConfig(confInf)
            Catch ex As Exception
                GoTo Create
            End Try
        End If

        Dim COG As String = args("/COG")
        Dim device As DrawingDevice = ChromosomeMap.CreateDevice(config)
        Dim model As ChromesomeDrawingModel = ChromosomeMap.FromPTT(PTT, config)

        If COG.FileExists(True) Then
            Dim COGProfiles As MyvaCOG() = COG.LoadCsv(Of MyvaCOG).ToArray
            model = ChromosomeMap.ApplyCogColorProfile(model, COGProfiles)
        End If

        Return device _
            .InvokeDrawing(model) _
            .SaveImage(out, "tiff")
    End Function
End Module
