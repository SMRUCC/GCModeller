#Region "Microsoft.VisualBasic::82ab903eeeb9575e953bdb55cf42a6ae, GCModeller\engine\IO\Raw\GCModellerRaw\StorageDriver.vb"

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

'   Total Lines: 83
'    Code Lines: 49
' Comment Lines: 17
'   Blank Lines: 17
'     File Size: 3.57 KB


'     Class StorageDriver
' 
'         Properties: mass
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: metabolome, proteome, transcriptome
' 
'         Sub: (+2 Overloads) Dispose, FluxSnapshot, MassSnapshot
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
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

        Sub New(output$, engine As Engine.Engine)
            Dim model As CellularModule = engine.model
            Dim core = engine.getCore

            Me.output = New Writer(model, output.Open(FileMode.OpenOrCreate, doClear:=True)).Init
            Me.mass = New OmicsTuple(Of String())(transcriptome, proteome, metabolome)

            Call WriteCellularGraph(graph:=core.ToGraph)
        End Sub

        Private Sub WriteCellularGraph(graph As NetworkGraph)
            Dim s = output.GetStream
            Dim metaboIndex As New Index(Of String)

            ' write nodes
            For Each metabo As Node In graph.vertex
                Using file As Stream = s.OpenBlock($"/graph/mass/{metabo.label}.dat")
                    Dim sb As New BinaryDataWriter(file, byteOrder:=ByteOrder.BigEndian)

                    Call sb.Write(metabo.ID)
                    Call sb.Write(metabo.label, BinaryStringFormat.DwordLengthPrefix)
                    Call sb.Write(metabo.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), BinaryStringFormat.DwordLengthPrefix)

                    Call sb.Flush()
                End Using

                Call metaboIndex.Add(metabo.label)
            Next

            ' write graph network
            For Each link As Edge In graph.graphEdges
                Dim metabo As String
                Dim react As String

                If link.U.label Like metaboIndex Then
                    metabo = link.U.label
                    react = link.V.label
                Else
                    metabo = link.V.label
                    react = link.U.label
                End If

                Using file As Stream = s.OpenBlock($"/graph/links/{metabo}/{react}.dat")
                    Dim sb As New BinaryDataWriter(file, byteOrder:=ByteOrder.BigEndian)

                    Call sb.Write(link.data.label, BinaryStringFormat.DwordLengthPrefix)
                    Call sb.Write(link.data.length)
                    Call sb.Write(link.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE), BinaryStringFormat.DwordLengthPrefix)

                    Call sb.Flush()
                End Using
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function transcriptome() As String()
            Return output.mRNAId.Objects.AsList + output.RNAId.Objects
        End Function

        Private Function proteome() As String()
            Return output.Polypeptide.Objects
        End Function

        Private Function metabolome() As String()
            Return output.Metabolites.Objects
        End Function

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call output.Write(NameOf(Writer.Metabolites), iteration, snapshot:=data)
            Call output.Write(NameOf(Writer.mRNAId), iteration, snapshot:=data)
            Call output.Write(NameOf(Writer.Polypeptide), iteration, snapshot:=data)
            Call output.Write(NameOf(Writer.Proteins), iteration, snapshot:=data)
            Call output.Write(NameOf(Writer.RNAId), iteration, snapshot:=data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call output.Write(NameOf(Writer.Reactions), iteration, snapshot:=data)
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
