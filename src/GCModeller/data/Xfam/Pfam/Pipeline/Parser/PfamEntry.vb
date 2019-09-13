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

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace PfamFastaComponentModels

    ''' <summary>
    ''' Pfam title model
    ''' </summary>
    Public Class PfamEntryHeader

        Friend Const P1 As Integer = 0
        Friend Const P2 As Integer = 1
        Friend Const P3 As Integer = 2

#Region "p1"
        Public Property UniqueId As String
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

        Protected Shared ReadOnly Property internalCreateNULL As PfamFasta
            Get
                Return New PfamFasta With {
                    .ChainId = NULL_ERROR,
                    .Location = New Location(0, 0),
                    .PfamCommonName = NULL_ERROR,
                    .PfamId = NULL_ERROR,
                    .PfamIdAsub = NULL_ERROR,
                    .SequenceData = NULL_ERROR,
                    .Uniprot = NULL_ERROR,
                    .UniqueId = NULL_ERROR
                }
            End Get
        End Property

        Public Shared Function ParseHeadTitle(strValue As String) As PfamFasta
            Dim DataToken As String() = strValue.Split

            If DataToken.IsNullOrEmpty OrElse
                DataToken.TryCount = 0 OrElse
                DataToken.Length < 2 Then

                Call $"NULL title tokens!!!  ----->   ""{strValue}""".__DEBUG_ECHO
                Return internalCreateNULL
            Else
                Return __createObject(DataToken)
            End If
        End Function

        Private Shared Function __createObject(data As String()) As PfamFasta
            Dim PfamFasta As PfamFasta = New PfamFasta

            Dim P1 As String = data(PfamEntryHeader.P1)
            Dim P2 As String = data(PfamEntryHeader.P2)
            Dim P3 As String = data(PfamEntryHeader.P3)

            data = P1.Split(CChar("/"))
            PfamFasta.UniqueId = data.First
            PfamFasta.Location = ComponentModel.Loci.Location.CreateObject(data.Last, "-")

            data = P2.Split(CChar("."))
            PfamFasta.Uniprot = data.First
            PfamFasta.ChainId = data.Last

            data = P3.Split(CChar(";"))
            PfamFasta.PfamCommonName = data(1)
            data = data.First.Split(CChar("."))
            PfamFasta.PfamId = data.First
            PfamFasta.PfamIdAsub = data.Last

            Return PfamFasta
        End Function
    End Class
End Namespace
