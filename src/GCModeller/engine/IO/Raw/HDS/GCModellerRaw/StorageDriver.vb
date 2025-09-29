#Region "Microsoft.VisualBasic::f194300ea30f873a838ff12b3352c81f, engine\IO\Raw\HDS\GCModellerRaw\StorageDriver.vb"

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


' Code Statistics:

'   Total Lines: 162
'    Code Lines: 113 (69.75%)
' Comment Lines: 19 (11.73%)
'    - Xml Docs: 15.79%
' 
'   Blank Lines: 30 (18.52%)
'     File Size: 6.86 KB


'     Class StorageDriver
' 
'         Properties: mass
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: metabolome, proteome, transcriptome
' 
'         Sub: (+2 Overloads) Dispose, FluxSnapshot, MassSnapshot, SetSymbolNames, WriteCellularGraph
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace Raw

    ''' <summary>
    ''' Data storage adapter driver
    ''' </summary>
    Public Class StorageDriver : Implements IDisposable, IOmicsDataAdapter

        ReadOnly output As Writer

        Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="output">the file path for save the experiment data.</param>
        ''' <param name="engine"></param>
        ''' <param name="graph_debug">
        ''' write the cellular graph into the result data file if this options is config as TRUE. debug used only
        ''' </param>
        Sub New(output$, engine As Engine.Engine, Optional graph_debug As Boolean = True)
            Dim models As CellularModule() = engine.models
            Dim core = engine.getCore
            Dim outfile As Stream = output.Open(FileMode.OpenOrCreate, doClear:=True)

            Me.output = New Writer(engine.getMassPool, engine.fluxIndex, outfile).Init
            Me.mass = New OmicsTuple(Of String())(transcriptome, proteome, metabolome)

            If graph_debug Then
                Call WriteCellularGraph(graph:=core.ToGraph)
            End If
        End Sub

        ''' <summary>
        ''' set molecule symbol names
        ''' </summary>
        ''' <param name="symbols"></param>
        Public Sub SetSymbolNames(symbols As Dictionary(Of String, String))
            Call output.GetStream.WriteText(symbols.GetJson, "/symbols.json", allocate:=False)
        End Sub

        Public Function GetStream() As StreamPack
            Return output.GetStream
        End Function

        Private Sub WriteCellularGraph(graph As NetworkGraph)
            Dim s As StreamPack = output.GetStream
            Dim metaboIndex As New Index(Of String)

            Call s.Delete("/graph/mass.jsonl")
            Call s.Delete("/graph/links.jsonl")

            Using file As Stream = s.OpenBlock("/graph/mass.jsonl")
                Dim sb As New StreamWriter(file, encoding:=Encodings.UTF8WithoutBOM.CodePage)

                Call VBDebugger.EchoLine($"write {graph.vertex.Count} nodes...")

                ' write nodes
                For Each metabo As Node In TqdmWrapper.Wrap(graph.vertex.ToArray, wrap_console:=App.EnableTqdm)
                    Dim metadata As New Dictionary(Of String, String) From {
                        {"id", metabo.ID},
                        {"label", metabo.label},
                        {"group", metabo.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)}
                    }

                    Call sb.WriteLine(metadata.GetJson)

                    If metabo.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) <> "reaction" Then
                        Call metaboIndex.Add(metabo.label)
                    End If
                Next

                Call sb.Flush()
            End Using

            Using file As Stream = s.OpenBlock($"/graph/links.jsonl")
                Dim sb As New StreamWriter(file, encoding:=Encodings.UTF8WithoutBOM.CodePage)

                Call VBDebugger.EchoLine($"write {graph.graphEdges.Count} network edges...")

                ' write graph network
                For Each link As Edge In TqdmWrapper.Wrap(graph.graphEdges.ToArray, wrap_console:=App.EnableTqdm)
                    Dim metabo As String
                    Dim react As String

                    If link.U.label Like metaboIndex Then
                        metabo = link.U.label
                        react = link.V.label
                    Else
                        metabo = link.V.label
                        react = link.U.label
                    End If

                    Dim metadata As New Dictionary(Of String, String) From {
                        {"label", link.data.label},
                        {"weight", link.data.length},
                        {"type", link.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE)},
                        {"graph", link.data!graph}
                    }

                    Call sb.WriteLine(metadata.GetJson)
                Next

                Call sb.Flush()
            End Using
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function transcriptome() As String()
            Return output.mRNAId.Objects.AsList + output.RNAId.Objects + output.tRNA.Objects + output.rRNA.Objects
        End Function

        Private Function proteome() As String()
            Return output.Polypeptide.Objects.AsList + output.Proteins.Objects
        End Function

        Private Function metabolome() As String()
            Return output.Metabolites.Objects
        End Function

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call output.WriteMassData(NameOf(Writer.Metabolites), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.mRNAId), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.Polypeptide), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.Proteins), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.RNAId), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.tRNA), iteration, snapshot:=data)
            Call output.WriteMassData(NameOf(Writer.rRNA), iteration, snapshot:=data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call output.WriteFluxData(NameOf(Writer.Reactions), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.Transcription), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.Translation), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.ProteinDegradation), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.PeptideDegradation), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.RNADegradation), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.tRNACharge), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.ribosomeAssembly), iteration, snapshot:=data)
            Call output.WriteFluxData(NameOf(Writer.ProteinMature), iteration, snapshot:=data)
        End Sub

        Public Sub ForwardRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ForwardRegulation
            Call output.WriteFluxForwardRegulation(NameOf(Writer.Reactions), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.Transcription), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.Translation), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.ProteinDegradation), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.PeptideDegradation), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.RNADegradation), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.tRNACharge), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.ribosomeAssembly), iteration, snapshot:=data)
            Call output.WriteFluxForwardRegulation(NameOf(Writer.ProteinMature), iteration, snapshot:=data)
        End Sub

        Public Sub ReverseRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ReverseRegulation
            Call output.WriteFluxReverseRegulation(NameOf(Writer.Reactions), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.Transcription), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.Translation), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.ProteinDegradation), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.PeptideDegradation), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.RNADegradation), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.tRNACharge), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.ribosomeAssembly), iteration, snapshot:=data)
            Call output.WriteFluxReverseRegulation(NameOf(Writer.ProteinMature), iteration, snapshot:=data)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call output.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
