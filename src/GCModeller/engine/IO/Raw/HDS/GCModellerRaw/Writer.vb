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

        Sub New(model As CellularModule, output As Stream)
            stream = New StreamPack(output, meta_size:=32 * 1024 * 1024)
            stream.Clear(32 * 1024 * 1024)

            MyBase.mRNAId = model.Genotype.centralDogmas.Where(Function(g) g.RNA.Value = RNATypes.mRNA).Select(Function(c) c.RNAName).Indexing
            MyBase.RNAId = model.Genotype.centralDogmas.Where(Function(g) g.RNA.Value <> RNATypes.mRNA).Select(Function(c) c.RNAName).Indexing
            MyBase.Polypeptide = model.Genotype.centralDogmas.Where(Function(g) g.RNA.Value = RNATypes.mRNA).Select(Function(c) c.polypeptide).Indexing
            MyBase.Proteins = model.Phenotype.proteins.Select(Function(p) p.ProteinID).Indexing
            MyBase.Metabolites = model.Phenotype.fluxes _
                .Select(Function(r) r.AllCompounds) _
                .IteratesALL _
                .Distinct _
                .ToArray
            MyBase.Reactions = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray

            compartments = {model.CellularEnvironmentName} _
                .JoinIterates(model.Phenotype.fluxes.Select(Function(r) r.enzyme_compartment)) _
                .JoinIterates(model.Phenotype.fluxes _
                    .Select(Function(r)
                                Return r.equation.GetMetabolites
                            End Function) _
                    .IteratesALL _
                    .Select(Function(c)
                                Return c.Compartment
                            End Function)) _
                .Distinct _
                .Where(Function(s) Not s.StringEmpty(, True)) _
                .ToArray

            Call stream.WriteText(compartments.JoinBy(vbCrLf), "/compartments.txt")
            Call stream.WriteText(
                {
                    New Dictionary(Of String, Integer) From {
                        {NameOf(mRNAId), mRNAId.Count},
                        {NameOf(RNAId), RNAId.Count},
                        {NameOf(Polypeptide), Polypeptide.Count},
                        {NameOf(Proteins), Proteins.Count},
                        {NameOf(Metabolites), Metabolites.Count},
                        {NameOf(Reactions), Reactions.Count}
                    }.GetJson
                }, "/.etc/count.json")
        End Sub

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

            For Each [module] As NamedValue(Of PropertyInfo) In modules.NamedValues
                Dim name$ = [module].Name
                Dim index As Index(Of String) = [module].Value.GetValue(Me)
                Dim list$() = index.Objects

                Call nameMaps.Add([module].Value.Name, name)
                Call stream.WriteText(list.JoinBy(vbCrLf), $"/dynamics/{[module].Value.Name}/index.txt")
                Call Me.modules.Add(name, index)
                Call Me.moduleIndex.Add(name)
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
        Public Function Write(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Dim index As Index(Of String) = modules(nameMaps([module]))
            Dim v As Double() = snapshot.Takes(index.Objects).ToArray
            Dim path As String = $"/dynamics/{[module]}/frames/{time}.dat"

            Call stream.Delete(path)
            Call ticks.Add(time)

            Using file As Stream = stream.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)
                Call New BinaryDataWriter(file, ByteOrder.BigEndian).Write(v)
            End Using

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
