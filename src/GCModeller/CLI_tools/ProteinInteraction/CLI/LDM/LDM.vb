#Region "Microsoft.VisualBasic::6c48ec25a8372051b88cff41acb46836, CLI_tools\ProteinInteraction\CLI\LDM\LDM.vb"

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

    ' Class Signature
    ' 
    '     Properties: Headers, Level, PfamString, SequenceData, Title
    ' 
    '     Function: (+2 Overloads) CreateObject, ToFasta, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns.Clustal

Public Class Signature : Implements IAbstractFastaToken

    Public Property PfamString As Pfam.PfamString.PfamString
    ''' <summary>
    ''' 多序列比对的出现概率
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property Level As Double

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return PfamString.ProteinId
        End Get
    End Property

    Public Property Headers As String() Implements IAbstractFastaToken.Headers
        Get
            Return {PfamString.ProteinId}
        End Get
        Set(value As String())
            PfamString.ProteinId = value.JoinBy(" ")
        End Set
    End Property

    <XmlText>
    Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    Public Overrides Function ToString() As String
        Return PfamString.ToString
    End Function

    Public Function ToFasta() As FASTA.FastaSeq
        Return New FastaSeq(Me)
    End Function

    Public Shared Function CreateObject(SRChain As SR(), Name As String) As Signature
        Return New Signature With {
            .PfamString = GetPfamString(SRChain, Name),
            .SequenceData = New String(SRChain.Select(Function(x) x.Residue))
        }
    End Function

    Public Shared Function CreateObject(chain As SRChain) As Signature
        Return CreateObject(chain.lstSR, chain.Name)
    End Function
End Class
