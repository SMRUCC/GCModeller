#Region "Microsoft.VisualBasic::555af1fc732675187301694f903887ed, data\Xfam\Pfam\Pipeline\Database\PfamEntry.vb"

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

    '   Total Lines: 111
    '    Code Lines: 84 (75.68%)
    ' Comment Lines: 3 (2.70%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 24 (21.62%)
    '     File Size: 3.78 KB


    '     Class PfamEntryHeader
    ' 
    '         Properties: ChainId, CommonName, location, PfamId, PfamIdAsub
    '                     UniProt, UniqueId
    ' 
    '         Function: createObjectInternal, internalCreateNull, (+2 Overloads) ParseHeaderTitle, ShadowCopy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Pipeline.Database

    ''' <summary>
    ''' Pfam title model
    ''' </summary>
    Public Class PfamEntryHeader : Implements INamedValue

        Friend Const P1 As Integer = 0
        Friend Const P2 As Integer = 1
        Friend Const P3 As Integer = 2

        Public Const REGEX_PFAM_ENTRY As String = "PF(am)?\d+\.\d+;.+?;"

#Region "p1"
        Public Property UniqueId As String Implements IKeyedEntity(Of String).Key
#End Region

#Region "p2"
        Public Property UniProt As String
        Public Property ChainId As String
#End Region

#Region "p3"
        Public Property PfamId As String
        Public Property PfamIdAsub As String
        Public Property CommonName As String
#End Region

        Public Property location As Location

        Public Function ShadowCopy(Of T As PfamEntryHeader)() As T
            Dim PfamObject As T = Activator.CreateInstance(Of T)()

            With PfamObject
                .ChainId = ChainId
                .PfamId = PfamId
                .PfamIdAsub = PfamIdAsub
                .CommonName = CommonName
                .UniProt = UniProt
                .UniqueId = UniqueId
            End With

            Return PfamObject
        End Function

        Const NULL_ERROR As String = "NULL_ERROR"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Shared Function internalCreateNull(Of T As {New, PfamEntryHeader})() As T
            Return New T() With {
                .ChainId = NULL_ERROR,
                .location = New Location(0, 0),
                .CommonName = NULL_ERROR,
                .PfamId = NULL_ERROR,
                .PfamIdAsub = NULL_ERROR,
                .UniProt = NULL_ERROR,
                .UniqueId = NULL_ERROR
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseHeaderTitle(str As String) As PfamEntryHeader
            Return ParseHeaderTitle(Of PfamEntryHeader)(str)
        End Function

        Public Shared Function ParseHeaderTitle(Of T As {New, PfamEntryHeader})(str As String) As T
            Dim tokens As String() = str.Split

            If tokens.IsNullOrEmpty OrElse
                tokens.TryCount = 0 OrElse
                tokens.Length < 2 Then

                Call $"NULL title tokens!!!  ----->   ""{str}""".debug

                Return internalCreateNull(Of T)()
            Else
                Return tokens.DoCall(AddressOf createObjectInternal(Of T))
            End If
        End Function

        Private Shared Function createObjectInternal(Of T As {New, PfamEntryHeader})(data As String()) As T
            Dim headers As New T

            Dim P1 As String = data(PfamEntryHeader.P1)
            Dim P2 As String = data(PfamEntryHeader.P2)
            Dim P3 As String = data(PfamEntryHeader.P3)

            data = P1.Split(CChar("/"))
            headers.UniqueId = data.First
            headers.location = Location.CreateObject(data.Last, "-")

            data = P2.Split(CChar("."))
            headers.UniProt = data.First
            headers.ChainId = data.Last

            data = P3.Split(CChar(";"))
            headers.CommonName = data(1)
            data = data.First.Split(CChar("."))
            headers.PfamId = data.First
            headers.PfamIdAsub = data.Last

            Return headers
        End Function
    End Class
End Namespace
