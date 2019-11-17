Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics

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
        Dim engine = New Engine.Engine(def).LoadModel(model.Trim.CreateModel)

        Call snapshots.SaveTo($"{out}/mass.xls", tsv:=True)
        Call flux.SaveTo($"{out}/flux.xls", tsv:=True)

        Return 0
    End Function
End Module
