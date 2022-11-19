#Region "Microsoft.VisualBasic::a2a498c093d4c7cf67a87d4b0951368f, GCModeller\engine\IO\Raw\GCModellerRaw\Writer.vb"

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

    '   Total Lines: 158
    '    Code Lines: 91
    ' Comment Lines: 42
    '   Blank Lines: 25
    '     File Size: 6.11 KB


    '     Class Writer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Init, (+2 Overloads) Write
    ' 
    '         Sub: Dispose, writeIndex
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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace Raw

    ''' <summary>
    ''' The GCModeller raw data writer
    ''' (写数据模块)
    ''' </summary>
    Public Class Writer : Inherits CellularModules

        ReadOnly stream As BinaryDataWriter

        ''' <summary>
        ''' 写在文件最末尾的，用于建立二叉树索引？
        ''' </summary>
        Dim offsets As New List(Of (time#, moduleName$, offset&))

        Sub New(loader As Loader, model As CellularModule, output As Stream)
            stream = New BinaryDataWriter(output, Encodings.UTF8WithoutBOM)

            MyBase.mRNAId = loader.GetCentralDogmaFluxLoader.mRNA
            MyBase.RNAId = loader.GetCentralDogmaFluxLoader.componentRNA
            MyBase.Polypeptide = loader.GetProteinMatureFluxLoader.polypeptides
            MyBase.Proteins = loader.GetProteinMatureFluxLoader.proteinComplex
            MyBase.Metabolites = model.Phenotype.fluxes _
                .Select(Function(r) r.AllCompounds) _
                .IteratesALL _
                .Distinct _
                .ToArray
            MyBase.Reactions = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray
        End Sub

        ''' <summary>
        ''' 将编号列表写入的原始文件之中
        ''' </summary>
        ''' <returns></returns>
        Public Function Init() As Writer
            Dim modules As Dictionary(Of String, PropertyInfo) = Me.GetModuleReader

            Call stream.Seek(0, SeekOrigin.Begin)
            Call stream.Write(CellularModules.Magic, BinaryStringFormat.NoPrefixOrTermination)

            Call Me.modules.Clear()
            Call Me.moduleIndex.Clear()

            ' 二进制文件的结构为 
            '
            ' - string 模块名称字符串，最开始为长度整形数
            ' - integer 有多少个编号
            ' - string ZERO 使用零结尾的编号字符串
            '
            For Each [module] As NamedValue(Of PropertyInfo) In modules.NamedValues
                Dim name$ = [module].Name
                Dim index As Index(Of String) = [module].Value.GetValue(Me)
                Dim list$() = index.Objects

                Call stream.Write(name, BinaryStringFormat.DwordLengthPrefix)
                Call stream.Write(list.Length)

                For Each id As String In list
                    Call stream.Write(id, BinaryStringFormat.ZeroTerminated)
                Next

                Call Me.modules.Add(name, index)
                Call Me.moduleIndex.Add(name)
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="module$"></param>
        ''' <param name="time#"></param>
        ''' <param name="snapshot">The snapshot value after the loop cycle in <paramref name="time"/> point</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Write(module$, time#, snapshot As Dictionary(Of String, Double)) As Writer
            Return Write([module], time, snapshot.Takes(modules([module]).Objects).ToArray)
        End Function

        Public Function Write(module$, time#, data#()) As Writer
            offsets += (time, [module], stream.Position)

            ' 一个循环之后得到的结果数据的写入的结构为
            '
            ' - double time 时间值
            ' - byte 在header之中的module的索引号
            ' - double() data块，每一个值的顺序是和header之中的id排布顺序是一样的，长度和header之中的id列表保持一致
            '
            Call stream.Write(time)
            Call stream.Write(CByte(moduleIndex([module])))
            Call stream.Write(data)

            Return Me
        End Function

        Private Sub writeIndex()
            '
            ' 最后在文件末尾写入offset索引
            '
            ' 索引按照time降序排序，结构为
            ' 
            ' - double time
            ' - integer index，从零开始的索引号
            ' - long() 按照modules顺序排序的offset值的集合
            ' - long 当前数据块的起始的offset偏移
            '
            Dim times = offsets _
                .GroupBy(Function(d) d.time) _
                .OrderByDescending(Function(time) time.Key) _
                .Select(Function(time)
                            Dim moduleOffsets = time.ToDictionary(Function(m) m.moduleName)
                            Dim offsets As Long() = moduleOffsets _
                                .Takes(moduleIndex.Objects) _
                                .Select(Function(m) m.offset) _
                                .ToArray

                            Return (time:=time.Key, offsets:=offsets)
                        End Function)
            Dim i As i32 = Scan0

            For Each time As (point#, offsets As Long()) In times
                Dim start& = stream.Position

                ' 最后一个值是当前索引的偏移量，这样子就可以
                ' 在读取的时候从后往前读取索引了
                Call stream.Write(time.point)
                Call stream.Write(++i)
                Call stream.Write(time.offsets)
                Call stream.Write(start)
            Next
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call writeIndex()

            Call stream.Flush()
            Call stream.Close()
            Call stream.Dispose()

            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
