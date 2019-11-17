Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Module CLI

    <ExportAPI("/run")>
    <Usage("/run /model <model.gcmarkup> [/out <result_directory>]")>
    Public Function Run(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim out$ = args("/out") Or $"{in$.TrimSuffix}.vcell_simulation/"
        Dim model As VirtualCell = [in].LoadXml(Of VirtualCell)
        Dim def As Definition = model.metabolismStructure _
            .Compounds _
            .Select(Function(c) c.ID) _
            .DoCall(Function(compounds)
                        Return Definition.KEGG(compounds)
                    End Function)
        Dim cell = model.Trim.CreateModel

        Using snapshots As New WriteStream(Of DataSet)($"{out}/mass.xls", metaBlank:=0, tsv:=True),
            flux As New WriteStream(Of DataSet)($"{out}/flux.xls", metaBlank:=0, tsv:=True)

            Dim dataStorage As DataStorageEngine = DataStorageEngine.InitializeDriver(cell)
            Dim engine As Engine = New Engine(def) _
                .LoadModel(cell) _
                .AttachBiologicalStorage(dataStorage)
        End Using

        Return 0
    End Function
End Module
