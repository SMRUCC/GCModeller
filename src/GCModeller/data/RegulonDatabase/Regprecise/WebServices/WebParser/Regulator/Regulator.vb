#Region "Microsoft.VisualBasic::03de0a176df6b6f58821d855dcf320f1, data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulator\Regulator.vb"

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

'   Total Lines: 105
'    Code Lines: 66 (62.86%)
' Comment Lines: 25 (23.81%)
'    - Xml Docs: 96.00%
' 
'   Blank Lines: 14 (13.33%)
'     File Size: 4.33 KB


'     Class Regulator
' 
'         Properties: biological_process, effector, family, infoURL, locus_tag
'                     LocusId, operons, pathway, Regulates, regulationMode
'                     regulator, regulatorySites, regulog, type
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ExportMotifs, (+2 Overloads) GetMotifSite, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Regprecise

    Public Class Regulator : Implements IReadOnlyId

        <XmlAttribute> Public Property type As Types
        <XmlAttribute> Public Property family As String
        <XmlAttribute> Public Property regulationMode As String

        <XmlElement> Public Property regulator As NamedValue
        <XmlElement> Public Property effector As String
        <XmlElement> Public Property pathway As String
        ''' <summary>
        ''' a group of regulators in this family
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("locus_tag")> Public Property locus_tags As NamedValue()
        <XmlElement> Public Property biological_process As String()
        <XmlElement> Public Property regulog As NamedValue

        ''' <summary>
        ''' 用来下载生成motif数据库的时候所需要使用的
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("url")>
        Public Property infoURL As String

        ''' <summary>
        ''' a collection of the motif sites
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("regulatory_sites", [Namespace]:=MotifFasta.xmlns)>
        Public Property regulatorySites As MotifFasta()

        ''' <summary>
        ''' 被这个调控因子所调控的基因，按照操纵子进行分组，这个适用于推断Regulon的
        ''' </summary>
        ''' <returns></returns>
        <XmlArray> Public Property operons As Operon()
        <XmlElement> Public Property Regulates As RegulatedGene()

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        ''' <summary>
        ''' 该Regprecise调控因子的基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        ''' first id of <see cref="locus_tags"/>
        ''' </returns>
        ''' <remarks></remarks>
        Public ReadOnly Property LocusId As String Implements IReadOnlyId.Identity
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If locus_tags.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return locus_tags(0).name
                End If
            End Get
        End Property

        Sub New()
            xmlns.Add("model", MotifFasta.xmlns)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", type.ToString, regulator.ToString)
        End Function

        Public Function GetMotifSite(GeneId As String, MotifPosition As Integer) As Regtransbase.WebServices.MotifFasta
            Dim LQuery = (From fa As Regtransbase.WebServices.MotifFasta In regulatorySites
                          Where String.Equals(GeneId, fa.locus_tag) AndAlso
                              fa.position = MotifPosition
                          Select fa).FirstOrDefault
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trace">locus_tag:position</param>
        ''' <returns></returns>
        Public Function GetMotifSite(trace As String) As MotifFasta
            Dim Tokens As String() = trace.Split(":"c)
            Return GetMotifSite(Tokens(Scan0), CInt(Val(Tokens(1))))
        End Function

        ''' <summary>
        ''' 这个函数会自动移除一些表示NNNN的特殊符号
        ''' </summary>
        ''' <returns></returns>
        Public Function ExportMotifs() As IEnumerable(Of FastaSeq)
            Return From fa As MotifFasta
                   In regulatorySites
                   Where Not fa Is Nothing AndAlso Not fa.SequenceData.StringEmpty
                   Let t As String = $"{fa.locus_tag}:{fa.position} [family={family}] [regulog={regulog.name}]"
                   Let attrs = New String() {t}
                   Let seq As String = Regtransbase.WebServices.Regulator.SequenceTrimming(fa)
                   Select New FastaSeq With {
                       .SequenceData = seq,
                       .Headers = attrs
                   }
        End Function
    End Class
End Namespace
