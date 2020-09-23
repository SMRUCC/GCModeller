#Region "Microsoft.VisualBasic::1b9f98f7bff28641b01fb110192faa41, sub-system\FBA\FBA_DP\rFBA\BuildModel.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module BuildModel
    ' 
    '         Sub: CreateObject, FixError
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller

Namespace rFBA

    Public Module BuildModel

        Public Sub CreateObject(MetaCyc As DatabaseLoadder, ChipData As IO.File, Regulation As IO.File, Regprecise As TranscriptionFactors, EXPORT As String)
            Dim Mapping As New Mapping(MetaCyc)
            Dim MetabolismFile As String = String.Format("{0}/MetabolismSystem-FBA.csv", EXPORT).Replace("\", "/")
            Dim TranscriptionFile As String = String.Format("{0}/TranscriptionRegulation-rFBA.csv", EXPORT).Replace("\", "/")

            Call ModelWriter.CreateObject(FileIO.FileSystem.GetFiles(MetaCyc.Database.DataDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.sbml").First, Mapping.CreateEnzrxnGeneMap).SaveTo(MetabolismFile, False)
            Call ModelWriter.CreateObject(ChipData,
                                          1,
                                          Reflector.Convert(Of ModelWriter.Regulation)(Regulation.DataFrame, False).ToArray,
                                          Mapping.EffectorMapping(Regprecise)).SaveTo(TranscriptionFile, False)

            Dim Model As New rFBA.DataModel.CellSystem
            Model.properties = New CompilerServices.[Property] With {
            .authors = New List(Of String) From {My.User.Name},
            .compiled = Now.ToString,
            .guid = Guid.NewGuid.ToString,
            .name = "rFBA",
            .specieId = MetaCyc.Database.Species.First.Synonyms.First
            }
            Model.MetabolismModel = New Href With {.Value = MetabolismFile}
            Model.TranscriptionModel = New Href With {.Value = TranscriptionFile}
            Model.ObjectiveFunctionModel = New Href With {.Value = EXPORT & "/ObjectiveFunction.csv"}

            Dim ObjectiveFunction As DataModel.ObjectiveFunction() = New DataModel.ObjectiveFunction() {}

            Call ObjectiveFunction.SaveTo(Model.ObjectiveFunctionModel.Value, False)
            Call FixError(Model)

            Call Model.GetXml.SaveTo(String.Format("{0}/{1}.gcmarkup_rfba.xml", EXPORT, MetaCyc.Database.Species.First.Synonyms.Last))
        End Sub

        ''' <summary>
        ''' 修复可能缺少的基因
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <remarks></remarks>
        Private Sub FixError(Model As rFBA.DataModel.CellSystem)
            Using LogFile As New LogFile(FileIO.FileSystem.GetParentPath(Model.ObjectiveFunctionModel.Value) & "/FlixError.log")
                Dim lstLocus = Model.TranscriptionModel.Value.LoadCsv(Of ModelReader.GeneExpression)(False)
                Dim MetabolismFluxs = Model.MetabolismModel.Value.LoadCsv(Of ModelReader.MetabolismFlux)(False)
                Dim AvgRPKM = (From item In lstLocus Select item.RPKM).Average

                For Each Flux In MetabolismFluxs
                    For Each Gene In Flux.AssociatedEnzymeGenes
                        Dim LQuery = (From GeneObject In lstLocus.AsParallel Where String.Equals(GeneObject.AccessionId, Gene) Select 1).ToArray
                        If LQuery.IsNullOrEmpty Then '目标基因不存在，则修复此错误
                            Call lstLocus.Add(New ModelReader.GeneExpression With {.AccessionId = Gene, .BasalExpression = 1, .RPKM = AvgRPKM})
                            Call LogFile.WriteLine(String.Format("Required enzyme {0} is not found in the genome, error was auto-fixed!", Gene), "BuildModel -> FixError()", MSG_TYPES.WRN)
                        End If
                    Next
                Next

                Call lstLocus.SaveTo(Model.TranscriptionModel.Value, False)
            End Using
        End Sub
    End Module
End Namespace
