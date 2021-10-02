#Region "Microsoft.VisualBasic::38b160625324bdfc04319cd37e3a71b1, localblast\LocalBLAST\LocalBLAST\LocalBLAST\InteropService\InitializeMethods.vb"

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

    '     Module InitializeMethods
    ' 
    '         Function: (+2 Overloads) CreateInstance, GetProgram
    '         Enum Program
    ' 
    '             BlastPlus, LocalBlast
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class InitializeParameter
    ' 
    '         Properties: BlastBin, Program
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace LocalBLAST.InteropService

    <Package("NCBI.Blast.Initialize")>
    Public Module InitializeMethods

        ReadOnly __createService As Dictionary(Of Program, System.Func(Of String, InteropService)) =
            New Dictionary(Of Program, Func(Of String, InteropService)) From {
 _
            {Program.LocalBlast, Function(BlastBin As String) New Programs.LocalBLAST(BlastBin)},
            {Program.BlastPlus, Function(BlastBin As String) New Programs.BLASTPlus(BlastBin)}
        }

        <ExportAPI("CreateSession")>
        Public Function CreateInstance(p As InitializeParameter) As InteropService
            If __createService.ContainsKey(p.Program) Then
                Return __createService(p.Program)(p.BlastBin)
            Else
                Return Nothing
            End If
        End Function

        <ExportAPI("CreateSession")>
        Public Function CreateInstance(Bin As String, Type As Program) As InteropService
            Return __createService(Type)(Bin)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">Only two value: 'blastplus' or 'localblast'</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Get.Type")>
        Public Function GetProgram(Name As String) As Program
            Return CType(InStr(LocalBLAST.InteropService.PROGRAMS, Name) / 10, Program)
        End Function

        Public Enum Program
            BlastPlus
            LocalBlast
        End Enum

        Public Const PROGRAMS As String = "blastplus+localblast"
    End Module

    ''' <summary>
    ''' 对本地BLAST外部命令的初始化参数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InitializeParameter

        ''' <summary>
        ''' BLAST程序的类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Program As LocalBLAST.InteropService.Program
        ''' <summary>
        ''' BLAST程序的可执行文件的存放的文件夹
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property BlastBin As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="BLASTBIN">BLAST程序的可执行文件的存放文件夹</param>
        ''' <param name="Type">BLAST程序的类型</param>
        ''' <remarks></remarks>
        Sub New(BLASTBIN As String, Optional Type As Program = LocalBLAST.InteropService.Program.LocalBlast)
            Me.BlastBin = BLASTBIN
            Me.Program = Type
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Program, BlastBin)
        End Function
    End Class
End Namespace
