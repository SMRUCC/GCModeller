#Region "Microsoft.VisualBasic::82c22e476ca6e24abd3677fe794935a1, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\ObjectBase\Citation.vb"

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

    '   Total Lines: 94
    '    Code Lines: 33
    ' Comment Lines: 51
    '   Blank Lines: 10
    '     File Size: 4.16 KB


    '     Structure Citation
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Any of the above components may be omitted, but it is meaningless to supply a timestamp, 
    ''' curator or probability if the evidence-code is omitted. Trailing colons should be 
    ''' omitted, but if a value contains an evidence-code with no accompanying citation, the 
    ''' leading colon must be present. The square brackets are optional.
    ''' </summary>
    ''' <remarks>
    ''' Examples:
    '''   [123456] -- a PubMed or MEDLINE reference
    '''   [SMITH95] -- a non-PubMed reference
    '''   [123456:EV-IDA] -- an evidence code with associated PubMed reference
    '''   [:EV-HINF] -- an evidence code with no associated reference
    '''   [123456:EV-IGI:9876543:paley] -- a time- and user-stamped evidence code with associated reference
    ''' </remarks>
    Public Structure Citation

        ''' <summary>
        ''' reference-ID is a PubMed unique identifier or the identifier of a Publications object 
        ''' (without the leading "PUB-").
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim ReferenceId As String
        ''' <summary>
        ''' evidence-code is the object identifier of some class belonging to the Evidence class, e.g. EV-EXP.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim EvidenceCode As String
        ''' <summary>
        ''' timestamp is a lisp universal time (not human readable) corresponding to the time the 
        ''' evidence code was assigned.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim TimeStamp As String
        ''' <summary>
        ''' curator is the username of the curator who assigned the evidence code.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim Curator As String
        ''' <summary>
        ''' probability is a number between 0 and 1 describing the probability 
        ''' that the evidence is correct, where available.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim Probability As String
        ''' <summary>
        ''' with is a free text string that modifies the evidence-code when the citation annotates a GO term. 
        ''' This is the "with" field described in GO documentation.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Dim [With] As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CITATIONS - :EV-COMP-AINF:3567386625:kaipa</remarks>
        Public Overrides Function ToString() As String
            Return String.Format("CITATIONS - {0}:{1}:{2}:{3}:{4}:{5}", ReferenceId, EvidenceCode, TimeStamp, Curator, Probability, [With])
        End Function

        Public Shared Narrowing Operator CType(e As Citation) As String
            Return String.Format("CITATIONS - {0}:{1}:{2}:{3}:{4}:{5}", e.ReferenceId, e.EvidenceCode, e.TimeStamp, e.Curator, e.Probability, e.[With])
        End Operator

        Public Shared Widening Operator CType(Citation As String) As Citation
            Dim Tokens As List(Of String) = Citation.Split(CChar(":")).AsList
            Dim NewObj As New Citation

            Tokens.AddRange(EmptyFill)

            With NewObj
                .ReferenceId = Tokens(0)
                .EvidenceCode = Tokens(1)
                .TimeStamp = Tokens(2)
                .Curator = Tokens(3)
                .Probability = Tokens(4)
                .With = Tokens(5)
            End With

            Return NewObj
        End Operator

        ''' <summary>
        ''' 为了防止Citation初始化的时候，由于在列表中的元素的数目不够的时候，赋值出现错误
        ''' </summary>
        ''' <remarks>使用On Error Resume Next会明显降低性能，使用这个空填充向量，可以改善错误容忍性能</remarks>
        Friend Shared ReadOnly EmptyFill As String() = {String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty}
    End Structure
End Namespace
