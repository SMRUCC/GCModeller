#Region "Microsoft.VisualBasic::6c19cb6d7e03ca6e998ebc5f82f5f022, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//Pipeline/LocalBlast/PfamHit.vb"

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

    '   Total Lines: 56
    '    Code Lines: 33
    ' Comment Lines: 15
    '   Blank Lines: 8
    '     File Size: 1.95 KB


    '     Class PfamHit
    ' 
    '         Properties: coverage, domainName, ends, Pfam, pfamID
    '                     start
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.Database
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline.LocalBlast

    ''' <summary>
    ''' A hit model of pfam domain annotation on query protein sequence.
    ''' </summary>
    Public Class PfamHit : Inherits BestHit

        ''' <summary>
        ''' The start position of this domain object on target sequence
        ''' </summary>
        ''' <returns></returns>
        Public Property start As Integer
        ''' <summary>
        ''' The end position of this domain object on target sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property ends As Integer

        ''' <summary>
        ''' 经过<see cref="BlastOutputParser"/>的解析输出，不论是哪个方向的比对结果，输出的结果<see cref="HitName"/>就是pfam的定义数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Pfam As PfamEntryHeader
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return PfamEntryHeader.ParseHeaderTitle(HitName)
            End Get
        End Property

        Public ReadOnly Property pfamID As String
            Get
                Return Pfam.PfamId
            End Get
        End Property

        Public ReadOnly Property domainName As String
            Get
                Return Pfam.CommonName
            End Get
        End Property

        Public Overrides ReadOnly Property coverage As Double
            Get
                Return hit_length / length_hit
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Dim {QueryName} As [{HitName}] = {identities} (positive:{positive}, evalue={evalue})"
        End Function
    End Class
End Namespace
