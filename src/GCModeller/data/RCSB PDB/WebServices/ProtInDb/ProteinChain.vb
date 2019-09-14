#Region "Microsoft.VisualBasic::bb02e1bdf51a3ae2944b562c12ce9f6e, RCSB PDB\WebServices\ProtInDb\ProteinChain.vb"

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

    ' Class ProteinChain
    ' 
    '     Properties: ChainId, InterfaceOnSurface, PdbId, SequenceData, Surface
    ' 
    '     Function: ToFASTA, ToString, TryParse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel

Public Class ProteinChain
    Implements IPolymerSequenceModel

    <XmlAttribute> Public Property PdbId As String
    <XmlAttribute> Public Property ChainId As String
    <XmlElement> Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
    <XmlAttribute> Public Property Surface As Integer()
    ''' <summary>
    ''' {<see cref="ProteinChain.ChainId">ChainId</see>, SurfaceSite}
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement> Public Property InterfaceOnSurface As KeyValuePairObject(Of String, Integer())()

    Public Function ToFASTA() As FASTA.FastaSeq
        Dim FsaObject As FASTA.FastaSeq = New FASTA.FastaSeq With {.SequenceData = SequenceData}
        FsaObject.Headers = New String() {String.Format("[PdbId:={0}] [ChainId:={1}]", PdbId, ChainId)}
        Return FsaObject
    End Function

    Public Shared Function TryParse(FilePath As String) As ProteinChain
        Dim strBuffer As String() = IO.File.ReadAllLines(FilePath)
        Dim ProteinChainFile As ProteinChain = New ProteinChain With {
            .PdbId = Mid(FileIO.FileSystem.GetName(FilePath), 1, 4),
            .ChainId = Mid(FileIO.FileSystem.GetName(FilePath), 5, 1)
        }
        ProteinChainFile.SequenceData = (From strLine As String In strBuffer
                                         Let Tokens As String() = strLine.Split(CChar(":"))
                                         Where String.Equals(Tokens.First, "sequence")
                                         Select Tokens.Last).First
        ProteinChainFile.Surface = (From ch As Char In (From strLine As String In strBuffer
                                                        Let Tokens As String() = strLine.Split(CChar(":"))
                                                        Where String.Equals(Tokens.First, "surfaces")
                                                        Select Tokens.Last).First.ToArray
                                    Select CInt(ch.ToString)).ToArray
        Dim p As Integer = Array.IndexOf(strBuffer, "#partnerChain:InterfacesOnSurface")
        If p > -1 Then
            Dim InterfaceData = strBuffer.Skip(p + 1).ToArray
            ProteinChainFile.InterfaceOnSurface = (From strLine As String In InterfaceData
                                                   Let Tokens As String() = strLine.Split(CChar(":"))
                                                   Let chainId As String = Tokens.First
                                                   Let data As Integer() = (From ch As Char In Tokens.Last.ToArray Select CInt(ch.ToString)).ToArray
                                                   Select New KeyValuePairObject(Of String, Integer()) With {.Key = chainId, .Value = data}).ToArray
        End If

        Return ProteinChainFile
    End Function

    Public Overrides Function ToString() As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each interactionChain In Me.InterfaceOnSurface
            Call sBuilder.Append(interactionChain.Key & ", ")
        Next
        Call sBuilder.Remove(sBuilder.Length - 2, 2)

        Return String.Format("{0}:{1}; Len:={2}aa, itr:={3}; {4}", PdbId, ChainId, Len(SequenceData), sBuilder.ToString, MyBase.ToString)
    End Function
End Class
