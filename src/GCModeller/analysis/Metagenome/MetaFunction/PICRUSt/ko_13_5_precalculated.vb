#Region "Microsoft.VisualBasic::05d5fd27c0a939a0b827cef4a1e4cc1c, GCModeller\analysis\Metagenome\MetaFunction\PICRUSt\ko_13_5_precalculated.vb"

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

    '   Total Lines: 61
    '    Code Lines: 38
    ' Comment Lines: 14
    '   Blank Lines: 9
    '     File Size: 2.06 KB


    '     Class ko_13_5_precalculated
    ' 
    '         Properties: ggId, QualifyName, size, taxonomyRank
    ' 
    '         Function: getFullName
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    ''' <summary>
    ''' the data tree is index via two index:
    ''' 
    ''' 1. greengenes OTU id index
    ''' 2. NCBI taxonomy tree index
    ''' </summary>
    ''' <remarks>
    ''' 相同的taxonomy可能会映射到多个greengenes编号
    ''' tree value is the mapping table of greengenes id to bytes offset
    ''' </remarks>
    Public Class ko_13_5_precalculated : Inherits Tree(Of Dictionary(Of String, Long))

        ''' <summary>
        ''' one of the node in the taxonomy tree
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomyRank As TaxonomyRanks

        Public Overrides ReadOnly Property QualifyName As String
            Get
                If Parent Is Nothing OrElse label = "." OrElse label = "/" Then
                    Return "/"
                Else
                    Return getFullName()
                End If
            End Get
        End Property

        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Data.Count
            End Get
        End Property

        Public ReadOnly Property ggId As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Data.Keys.ToArray
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getFullName() As String
            Return (Parent.QualifyName & ";" & $"{taxonomyRank.Description.ToLower}__{label}").Trim("/"c, ";"c, " "c, ASCII.TAB, "."c)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Sub Add(ggid As String, offset As Long)
            Call Data.Add(ggid, offset)
        End Sub

    End Class
End Namespace
