#Region "Microsoft.VisualBasic::c5e6d8b963319f185d3ffb57521803c3, data\Xfam\Pfam\Parser\PfamFasta.vb"

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

'     Class PfamFasta
' 
'         Properties: __internalCreateNULL, Headers, Location, Title
' 
'         Function: __createObject, CreateCsvArchive, CreateObject, ParseEntry, ParseHeadTitle
'                   ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace PfamFastaComponentModels

    ''' <summary>
    ''' 用于解析和表示pfam的fasta序列库中的蛋白结构域序列的数据模型
    ''' </summary>
    Public Class PfamFasta : Inherits PfamEntryHeader
        Implements IPolymerSequenceModel
        Implements IAbstractFastaToken

        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property Title As String Implements IAbstractFastaToken.title
            Get
                Return String.Format("{0}/{1}-{2} {3}.{4} {5}.{6};{7};", UniqueId, Location.Left, Location.Right, Uniprot, ChainId, PfamId, PfamIdAsub, PfamCommonName)
            End Get
        End Property

        Public Property Headers As String() Implements IAbstractFastaToken.headers

        Public Shared Function CreateObject(FastaObject As FastaSeq) As PfamFasta
            Dim FastaData = ParseHeaderTitle(Of PfamFasta)(FastaObject.Title)
            FastaData.SequenceData = FastaObject.SequenceData
            Return FastaData
        End Function

        Const REGEX_PFAM_ENTRY As String = "PF(am)?\d+\.\d+;.+?;"

        ''' <summary>
        ''' 本方法仅仅解析出Pfam编号以及Pfam结构域的名称
        ''' </summary>
        ''' <param name="strValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ParseEntry(strValue As String) As PfamFasta
            Dim s As String = Regex.Match(strValue, REGEX_PFAM_ENTRY, RegexOptions.IgnoreCase).Value
            If String.IsNullOrEmpty(s) Then
                Call $"NULL_ERROR: {strValue}   @{MethodBase.GetCurrentMethod.Name}".__DEBUG_ECHO
                Return internalCreateNull(Of PfamFasta)()
            End If

            Dim Tokens As String() = Strings.Split(s, ";")
            Return New PfamFasta With {
                .PfamId = Tokens(0).Split("."c).First,
                .PfamCommonName = Tokens(1)
            }
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", PfamId, SequenceData)
        End Function

        Public Shared Function CreateCsvArchive(data As IEnumerable(Of PfamFasta)) As PfamCsvRow()
            Return (From FastaObject As PfamFasta
                    In data.AsParallel
                    Select PfamCsvRow.CreateObject(FastaObject)).ToArray
        End Function
    End Class
End Namespace
