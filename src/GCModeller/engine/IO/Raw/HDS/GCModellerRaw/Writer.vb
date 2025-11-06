#Region "Microsoft.VisualBasic::c5e958e2a543c96e10a283594db5ed09, engine\IO\Raw\HDS\GCModellerRaw\Writer.vb"

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

'   Total Lines: 126
'    Code Lines: 88 (69.84%)
' Comment Lines: 17 (13.49%)
'    - Xml Docs: 94.12%
' 
'   Blank Lines: 21 (16.67%)
'     File Size: 5.32 KB


'     Class Writer
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: GetStream, Init, Write
' 
'         Sub: Dispose
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace Raw

    ''' <summary>
    ''' The GCModeller raw data writer
    ''' </summary>
    ''' <remarks>
    ''' (写数据模块)
    ''' </remarks>
    Public Class Writer : Inherits CellularModules

        ReadOnly stream As StreamPack
        ReadOnly nameMaps As New Dictionary(Of String, String)
        ReadOnly ticks As New List(Of Double)
        ReadOnly compartments As String()
        ReadOnly instance_id As New Dictionary(Of String, Dictionary(Of String, String()))
        ReadOnly flux_idset As New Dictionary(Of String, String())
        ReadOnly mapping As Dictionary(Of String, (source_id As String, compart_id As String))

        Sub New(mass As MassTable, fluxIndex As Dictionary(Of String, String()), output As Stream)
            stream = New StreamPack(output, meta_size:=32 * 1024 * 1024)
            stream.Clear(32 * 1024 * 1024)

            ' create molecule index
            MyBase.mRNAId = getTemplateIndex(mass.GetRole(MassRoles.mRNA))
            MyBase.RNAId = getTemplateIndex(mass.GetRole(MassRoles.RNA))
            MyBase.tRNA = getTemplateIndex(mass.GetRole(MassRoles.tRNA))
            MyBase.rRNA = getTemplateIndex(mass.GetRole(MassRoles.rRNA))
            MyBase.Polypeptide = getTemplateIndex(mass.GetRole(MassRoles.polypeptide))
            MyBase.Proteins = getTemplateIndex(mass.GetRole(MassRoles.protein))
            MyBase.Metabolites = getTemplateIndex(mass.GetRole(MassRoles.compound))
            ' create flux index
            MyBase.Reactions = fluxIndex!MetabolismNetworkLoader
            MyBase.Transcription = fluxIndex!transcription
            MyBase.Translation = fluxIndex!translation
            MyBase.ProteinDegradation = fluxIndex!proteinDegradation
            MyBase.PeptideDegradation = fluxIndex!polypeptideDegradation
            MyBase.RNADegradation = fluxIndex!RNADegradation
            MyBase.tRNACharge = fluxIndex!tRNAProcess
            MyBase.ribosomeAssembly = fluxIndex!ribosomeAssembly
            MyBase.ProteinMature = fluxIndex!ProteinMatureFluxLoader

            mapping = mass.getMapping
            compartments = mass.compartment_ids.ToArray

            Call stream.WriteText(compartments.JoinBy(vbCrLf), "/compartments.txt")
            Call stream.WriteText(
                {
                    New Dictionary(Of String, Integer) From {
                        {NameOf(mRNAId), mRNAId.Count},
                        {NameOf(RNAId), RNAId.Count},
                        {NameOf(Polypeptide), Polypeptide.Count},
                        {NameOf(Proteins), Proteins.Count},
                        {NameOf(Metabolites), Metabolites.Count},
                        {NameOf(Reactions), Reactions.Count},
                        {NameOf(tRNA), tRNA.Count},
                        {NameOf(rRNA), rRNA.Count},
                        {NameOf(Transcription), Transcription.Count},
                        {NameOf(Translation), Translation.Count},
                        {NameOf(ProteinDegradation), ProteinDegradation.Count},
                        {NameOf(PeptideDegradation), PeptideDegradation.Count},
                        {NameOf(RNADegradation), RNADegradation.Count},
                        {NameOf(tRNACharge), tRNACharge.Count},
                        {NameOf(ribosomeAssembly), ribosomeAssembly.Count},
                        {NameOf(ProteinMature), ProteinMature.Count}
                    }.GetJson
                }, "/.etc/count.json")
        End Sub

        Private Shared Function getTemplateIndex(factors As IEnumerable(Of Factor)) As Index(Of String)
            Return factors.Select(Function(f) f.ID).Distinct.Indexing
        End Function

        Public Shared Function CompartmentIdSet(models As IEnumerable(Of CellularModule)) As String()
            Return models.Select(Function(m) m.CellularEnvironmentName) _
                .JoinIterates(models.Select(Function(m) m.Phenotype.fluxes.Select(Function(r) r.enzyme_compartment)).IteratesALL) _
                .JoinIterates(models.Select(Function(m) m.Phenotype.fluxes _
                    .Select(Function(r)
                                Return r.equation.GetMetabolites
                            End Function) _
                    .IteratesALL _
                    .Select(Function(c)
                                Return c.Compartment
                            End Function)).IteratesALL) _
                .Distinct _
                .Where(Function(s) Not s.StringEmpty(, True)) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetStream() As StreamPack
            Return stream
        End Function

        ''' <summary>
        ''' 将编号列表写入的原始文件之中
        ''' </summary>
        ''' <returns></returns>
        Public Function Init() As Writer
            Dim modules As Dictionary(Of String, PropertyInfo) = Me.GetModuleReader

            Call Me.modules.Clear()
            Call Me.moduleIndex.Clear()
            Call Me.nameMaps.Clear()
            Call Me.ticks.Clear()
            Call instance_id.Clear()
            Call flux_idset.Clear()

            For Each [module] As NamedValue(Of PropertyInfo) In modules.NamedValues
                Dim name$ = [module].Name
                Dim index As Index(Of String) = [module].Value.GetValue(Me)
                Dim isFlux As Boolean = name.EndsWith("-Flux")

                If index Is Nothing Then
                    Continue For
                End If

                Dim list$() = index.Objects

                If isFlux Then
                    Call flux_idset.Add(name, list)
                Else
                    Dim templates = list.Select(Function(id) mapping(id)).ToArray

                    list = templates _
                        .Select(Function(a) a.source_id) _
                        .Distinct _
                        .ToArray

                    For Each compartment In templates.GroupBy(Function(a) a.compart_id)
                        Dim instance_id As String() = compartment _
                            .Select(Function(id)
                                        Return id.source_id & "@" & id.compart_id
                                    End Function) _
                            .ToArray
                        Dim compart_id As String = compartment.Key

                        If Not Me.instance_id.ContainsKey(compart_id) Then
                            Call Me.instance_id.Add(compart_id, New Dictionary(Of String, String()))
                        End If

                        Call Me.instance_id(compart_id).Add(name, instance_id)
                    Next
                End If

                Call nameMaps.Add([module].Value.Name, name)
                Call stream.WriteText(list.JoinBy(vbCrLf), $"/index/{name}.txt")
                Call Me.modules.Add(name, index)
                Call Me.moduleIndex.Add(name)
            Next

            Call stream.WriteText(instance_id.GetJson, "/dynamics/cellular_symbols.json")
            Call stream.WriteText(flux_idset.GetJson, "/dynamics/cellular_flux.json")

            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="module">the molecule data types</param>
        ''' <param name="time">the time tick point</param>
        ''' <param name="snapshot">The snapshot value after the loop cycle in <paramref name="time"/> point</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WriteMassData(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Dim resolve_name As String = nameMaps([module])
            Dim index As Index(Of String) = modules(resolve_name)

            For Each compart_id As String In compartments
                Dim instance_id As String() = Me.instance_id(compart_id).TryGetValue(resolve_name)

                If instance_id.IsNullOrEmpty Then
                    Continue For
                End If

                Dim v As Double() = snapshot.Takes(instance_id).ToArray
                Dim path As String = $"/dynamics/{compart_id}/{resolve_name}/frames/{time}.dat"

                Call stream.Delete(path)
                Call ticks.Add(time)

                Using file As Stream = stream.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)
                    Call New BinaryDataWriter(file, ByteOrder.BigEndian).Write(v)
                End Using
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="module">the molecule data types</param>
        ''' <param name="time">the time tick point</param>
        ''' <param name="snapshot">The snapshot value after the loop cycle in <paramref name="time"/> point</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WriteFluxData(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Dim resolve_name As String = nameMaps([module])
            Dim index As Index(Of String) = modules(resolve_name)
            Dim instance_id As String() = index.Objects

            If Not instance_id.IsNullOrEmpty Then
                Dim v As Double() = snapshot.Takes(instance_id).ToArray
                Dim path As String = $"/dynamics/flux/{resolve_name}/frames/{time}.dat"

                Call stream.Delete(path)
                Call ticks.Add(time)

                Using file As Stream = stream.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)
                    Call New BinaryDataWriter(file, ByteOrder.BigEndian).Write(v)
                End Using
            End If

            Return Me
        End Function

        Public Function WriteFluxForwardRegulation(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Dim resolve_name As String = nameMaps([module])
            Dim index As Index(Of String) = modules(resolve_name)
            Dim instance_id As String() = index.Objects

            If Not instance_id.IsNullOrEmpty Then
                Dim v As Double() = snapshot.Takes(instance_id).ToArray
                Dim path As String = $"/dynamics/flux/{resolve_name}/forward/{time}.dat"

                Call stream.Delete(path)
                Call ticks.Add(time)

                Using file As Stream = stream.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)
                    Call New BinaryDataWriter(file, ByteOrder.BigEndian).Write(v)
                End Using
            End If

            Return Me
        End Function

        Public Function WriteFluxReverseRegulation(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Dim resolve_name As String = nameMaps([module])
            Dim index As Index(Of String) = modules(resolve_name)
            Dim instance_id As String() = index.Objects

            If Not instance_id.IsNullOrEmpty Then
                Dim v As Double() = snapshot.Takes(instance_id).ToArray
                Dim path As String = $"/dynamics/flux/{resolve_name}/reverse/{time}.dat"

                Call stream.Delete(path)
                Call ticks.Add(time)

                Using file As Stream = stream.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)
                    Call New BinaryDataWriter(file, ByteOrder.BigEndian).Write(v)
                End Using
            End If

            Return Me
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Dim ticks = Me.ticks.Distinct.OrderBy(Function(ti) ti).ToArray

            Using file As Stream = stream.OpenFile("/.etc/ticks.dat", FileMode.OpenOrCreate, FileAccess.Write)
                Call New BinaryDataWriter(file, byteOrder:=ByteOrder.BigEndian).Write(ticks)
            End Using

            Call stream.WriteText({ticks.Length.ToString}, "/.etc/ticks.txt")

            Call stream.Close()
            Call stream.Dispose()

            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
