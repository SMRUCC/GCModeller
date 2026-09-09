#Region "Microsoft.VisualBasic::666bb40e7def64331662f7843772ab38, models\BioCyc\test\Program.vb"

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

    '   Total Lines: 44
    '    Code Lines: 31 (70.45%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (29.55%)
    '     File Size: 1.41 KB


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Data.BioCyc

Module Program

    ''' <summary>
    ''' 入口：默认执行 BioCyc → RetroPath 的次级代谢产物生物合成通路搜索演示。
    '''   dotnet run -c Release                 # 全部目标
    '''   dotnet run -c Release -- CHORISMATE   # 只跑指定 UNIQUE-ID 的目标
    '''   dotnet run -c Release -- all          # 以全库汇模式跑全部目标
    '''   dotnet run -c Release -- metabolic    # MetabolicAdapter 演示（内部标准代谢模型）
    '''   dotnet run -c Release -- route        # A → B 定向合成通路搜索演示（EcoCyc）
    '''   dotnet run -c Release -- legacy       # 旧的数据库读取冒烟测试
    ''' </summary>
    Sub Main(args As String())
        If args.Length > 0 AndAlso args(0).Equals("bench", StringComparison.OrdinalIgnoreCase) Then
            BenchDemo.Run(args.Skip(1).ToArray())
        ElseIf args.Length > 0 AndAlso args(0).Equals("route", StringComparison.OrdinalIgnoreCase) Then
            SynthesisRouteDemo.Run(args.Skip(1).ToArray())
        ElseIf args.Length > 0 AndAlso args(0).Equals("legacy", StringComparison.OrdinalIgnoreCase) Then
            LegacySmokeTest()
        ElseIf args.Length > 0 AndAlso args(0).Equals("pathway", StringComparison.OrdinalIgnoreCase) Then
            PathwayFinderDemo.Run(args.Skip(1).FirstOrDefault())
        Else
            MetabolicDemo.Run() '(args.Skip(1).FirstOrDefault())
        End If
    End Sub

    ''' <summary>旧的 BioCyc 文件读取冒烟测试（路径为历史版本，仅保留备查）</summary>
    Private Sub LegacySmokeTest()
        Using file As Stream = "F:\ecoli\28.1\data\proteins.dat".Open
            Dim data = AttrDataCollection(Of proteins).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "F:\ecoli\28.1\data\genes.dat".Open
            Dim data = AttrDataCollection(Of genes).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "F:\ecoli\28.1\data\reactions.dat".Open
            Dim data = AttrDataCollection(Of reactions).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "G:\GCModeller\src\GCModeller\models\BioCyc\test\compounds.txt".Open
            Dim data = AttrDataCollection(Of compounds).LoadFile(file)
            Dim xrefs = compounds.GetDbLinks(data.features.First).ToArray

            Pause()
        End Using
    End Sub

End Module
