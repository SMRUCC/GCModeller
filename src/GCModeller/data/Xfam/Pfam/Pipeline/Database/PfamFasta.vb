#Region "Microsoft.VisualBasic::c709e0d6d12301262eb171ab029c9e2d, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//Pipeline/Database/PfamFasta.vb"

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
    '    Code Lines: 33
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 1.84 KB


    '     Class PfamFasta
    ' 
    '         Properties: Headers, SequenceData, Title
    ' 
    '         Function: CreateCsvArchive, CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Pipeline.Database

    ''' <summary>
    ''' 用于解析和表示pfam的fasta序列库中的蛋白结构域序列的数据模型
    ''' </summary>
    Public Class PfamFasta : Inherits PfamEntryHeader
        Implements IPolymerSequenceModel
        Implements IAbstractFastaToken

        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property Title As String Implements IAbstractFastaToken.title
            Get
                Return String.Format("{0}/{1}-{2} {3}.{4} {5}.{6};{7};", UniqueId, location.Left, location.Right, UniProt, ChainId, PfamId, PfamIdAsub, CommonName)
            End Get
        End Property

        Public Property Headers As String() Implements IAbstractFastaToken.headers

        Public Shared Function CreateObject(FastaObject As FastaSeq) As PfamFasta
            Dim FastaData = ParseHeaderTitle(Of PfamFasta)(FastaObject.Title)
            FastaData.SequenceData = FastaObject.SequenceData
            Return FastaData
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", PfamId, SequenceData)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateCsvArchive(data As IEnumerable(Of PfamFasta)) As IEnumerable(Of PfamCsvRow)
            Return From FastaObject As PfamFasta
                   In data.AsParallel
                   Select PfamCsvRow.CreateObject(FastaObject)
        End Function
    End Class
End Namespace
