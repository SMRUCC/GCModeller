#Region "Microsoft.VisualBasic::8e67ffa020ddca6c2d818089ecfa8aad, meme_suite\MEME\Programs\TomTOM\MotifMatch.vb"

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

    '     Class MotifMatch
    ' 
    '         Properties: [Module], Family, Headers, LocusId, MatchSelf
    '                     MEMEEvalue, MEMEPvalue, Right, Site, Start
    '                     Title, uid, Width
    ' 
    '         Function: GetSequenceData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.TomTOM
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Programs

    Public Class MotifMatch : Inherits TOMText
        Implements IFastaProvider

        ''' <summary>
        ''' This motif match him self???
        ''' </summary>
        ''' <returns></returns>
        <Column("match.self?")> Public ReadOnly Property MatchSelf As Boolean
            Get
                Return String.Equals($"{[Module]}.{Query}", $"{Family}.{Target}", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        ''' <summary>
        ''' 用于映射用的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property uid As String
            Get
                Dim query As String = $"{Family}.{Target}"
                Dim hit As String = $"{[Module]}.{Me.Query}"
                Dim ar As String() = {query, hit}
                Dim sort As String() = ar.OrderBy(Function(s) s).ToArray
                Return String.Join("@", sort)
            End Get
        End Property

        Public Property [Module] As String
        Public Property Family As String
        <Column("MEME.Evalue")> Public Property MEMEEvalue As Double
        <XmlAttribute> Public Property Width As Integer
        ''' <summary>
        ''' Site name，该目标序列的Fasta文件的文件头，一般是基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property LocusId As String
        ''' <summary>
        ''' 位点的序列
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Site As String
        <Column("MEME.pvalue")> Public Overridable Property MEMEPvalue As Double
        ''' <summary>
        ''' 在整条序列之中的起始位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Start As Long
        <XmlAttribute> Public Property Right As Long

#Region "Implements SequenceModel.FASTA.I_FastaProvider"
        <Ignored> Public ReadOnly Property Title As String Implements IFastaProvider.title
            Get
                Return $"{uid}::{LocusId}"
            End Get
        End Property
        <Ignored> Public ReadOnly Property Headers As String()
            Get
                Return {Title}
            End Get
        End Property
#End Region

        Public Overrides Function ToString() As String
            Return uid
        End Function

        Private Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Return Site
        End Function
    End Class
End Namespace
