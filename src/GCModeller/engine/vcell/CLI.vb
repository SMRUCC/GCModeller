Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Module CLI

    <Extension>
    Private Function getDeletionList(res As DefaultString) As String()
        If res.FileExists Then
            Return res.ReadAllLines _
                .Select(Function(l) l.Split.First) _
                .ToArray
        Else
            Return res.Split(","c)
        End If
    End Function

    <ExportAPI("/run")>
    <Usage("/run /model <model.gcmarkup> [/deletes <genelist> /out <result_directory>]")>
    <Description("Run GCModeller VirtualCell.")>
    <Argument("/deletes", True, CLITypes.String,
              AcceptTypes:={GetType(String())},
              Description:="The ``locus_tag`` id list that will removes from the genome, 
              use the comma symbol as delimiter. Or a txt file path for the gene id list.")>
    Public Function Run(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim deletes As String() = args("/deletes").getDeletionList
        Dim out$ = args("/out") Or $"{in$.TrimSuffix}.vcell_simulation/"
        Dim model As VirtualCell = [in].LoadXml(Of VirtualCell)
        Dim def As Definition = model.metabolismStructure _
            .compounds _
            .Select(Function(c) c.ID) _
            .DoCall(Function(compounds)
                        Return Definition.KEGG(compounds, 5000)
                    End Function)
        Dim cell As CellularModule = model.CreateModel
        Dim massIndex = OmicsDataAdapter.GetMassTuples(cell)
        Dim fluxIndex = OmicsDataAdapter.GetFluxTuples(cell)

        Call "Open data stream output device..".__DEBUG_ECHO

        Using transcriptomeSnapshots As New WriteStream(Of DataSet)($"{out}/mass/transcriptome.xls", metaKeys:=massIndex.transcriptome, metaBlank:=0, tsv:=True),
              proteomeSnapshots As New WriteStream(Of DataSet)($"{out}/mass/proteome.xls", metaKeys:=massIndex.proteome, metaBlank:=0, tsv:=True),
              metabolomeSnapshots As New WriteStream(Of DataSet)($"{out}/mass/metabolome.xls", metaKeys:=massIndex.metabolome, metaBlank:=0, tsv:=True),
              transcriptomeFlux As New WriteStream(Of DataSet)($"{out}/flux/transcriptome.xls", metaKeys:=fluxIndex.transcriptome, metaBlank:=0, tsv:=True),
              proteomeFlux As New WriteStream(Of DataSet)($"{out}/flux/proteome.xls", metaKeys:=fluxIndex.proteome, metaBlank:=0, tsv:=True),
              metabolomeFlux As New WriteStream(Of DataSet)($"{out}/flux/metabolome.xls", metaKeys:=fluxIndex.metabolome, metaBlank:=0, tsv:=True)

            Dim massSnapshots As New OmicsTuple(Of DataStorageDriver)(
                transcriptome:=transcriptomeSnapshots.createDriver,
                proteome:=proteomeSnapshots.createDriver,
                metabolome:=metabolomeSnapshots.createDriver
            )
            Dim fluxSnapshots As New OmicsTuple(Of DataStorageDriver)(
                transcriptome:=transcriptomeFlux.createDriver,
                proteome:=proteomeFlux.createDriver,
                metabolome:=metabolomeFlux.createDriver
            )
            Dim dataStorage As New OmicsDataAdapter(cell, massSnapshots, fluxSnapshots)
            Dim engine As Engine = New Engine(def) _
                .LoadModel(cell, deletes) _
                .AttachBiologicalStorage(dataStorage)

            Return engine.Run
        End Using
    End Function

    <Extension>
    Private Function createDriver(save As WriteStream(Of DataSet)) As DataStorageDriver
        Return Sub(i, data)
                   Dim snapshot As New DataSet With {
                      .ID = "#" & (i + 1),
                      .Properties = data
                   }

                   Call save.Flush(snapshot)
               End Sub
    End Function
End Module
