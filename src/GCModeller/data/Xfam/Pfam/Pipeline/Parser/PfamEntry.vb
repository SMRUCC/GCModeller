#Region "Microsoft.VisualBasic::afb3070b78db303cf089df93209a9860, data\Xfam\Pfam\Parser\HeaderCommon.vb"

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

'     Class PfamCommon
' 
'         Properties: ChainId, PfamCommonName, PfamId, PfamIdAsub, SequenceData
'                     Uniprot, UniqueId
' 
'         Function: ShadowCopy
' 
'     Class PfamCsvRow
' 
'         Properties: Ends, Start
' 
'         Function: CreateObject
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace PfamFastaComponentModels

    ''' <summary>
    ''' Pfam title model
    ''' </summary>
    Public Class PfamEntryHeader : Implements INamedValue

        Friend Const P1 As Integer = 0
        Friend Const P2 As Integer = 1
        Friend Const P3 As Integer = 2

#Region "p1"
        Public Property UniqueId As String Implements IKeyedEntity(Of String).Key
#End Region

#Region "p2"
        Public Property Uniprot As String
        Public Property ChainId As String
#End Region

#Region "p3"
        Public Property PfamId As String
        Public Property PfamIdAsub As String
        Public Property PfamCommonName As String
#End Region

        Public Property location As Location

        Public Function ShadowCopy(Of T As PfamEntryHeader)() As T
            Dim PfamObject As T = Activator.CreateInstance(Of T)()

            With PfamObject
                .ChainId = ChainId
                .PfamId = PfamId
                .PfamIdAsub = PfamIdAsub
                .PfamCommonName = PfamCommonName
                .Uniprot = Uniprot
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
                .PfamCommonName = NULL_ERROR,
                .PfamId = NULL_ERROR,
                .PfamIdAsub = NULL_ERROR,
                .Uniprot = NULL_ERROR,
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

                Call $"NULL title tokens!!!  ----->   ""{str}""".__DEBUG_ECHO

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
            headers.Uniprot = data.First
            headers.ChainId = data.Last

            data = P3.Split(CChar(";"))
            headers.PfamCommonName = data(1)
            data = data.First.Split(CChar("."))
            headers.PfamId = data.First
            headers.PfamIdAsub = data.Last

            Return headers
        End Function
    End Class
End Namespace
