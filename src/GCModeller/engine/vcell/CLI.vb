Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Module CLI

    <ExportAPI("/run")>
    <Usage("/run /model <model.gcmarkup> [/out <result_directory>]")>
    Public Function Run(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim out$ = args("/out") Or $"{in$.TrimSuffix}.vcell_simulation/"
        Dim model As VirtualCell = [in].LoadXml(Of VirtualCell)
        Dim def As Definition = model.MetabolismStructure _
            .Compounds _
            .Select(Function(c) c.ID) _
            .DoCall(Function(compounds)
                        Return Definition.KEGG(compounds)
                    End Function)
        Dim loader As New Loader(def)
        Dim cell As Core.Vessel = model _
            .Trim _
            .CreateModel _
            .DoCall(AddressOf loader.CreateEnvironment)
        Dim mass As Dictionary(Of String, Factor) = cell.MassEnvironment.ToDictionary(Function(factor) factor.ID)

        Call cell.Initialize()

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)

        For i As Integer = 0 To 5000
            flux += New DataSet With {
                .ID = i,
                .Properties = cell _
                    .ContainerIterator() _
                    .ToDictionary _
                    .FlatTable
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = mass.ToDictionary(Function(m) m.Key, Function(m) m.Value.Value)
            }

            Call i.__DEBUG_ECHO
        Next

        Call snapshots.SaveTo($"{out}/mass.xls", tsv:=True)
        Call flux.SaveTo($"{out}/flux.xls", tsv:=True)

        Return 0
    End Function
End Module
