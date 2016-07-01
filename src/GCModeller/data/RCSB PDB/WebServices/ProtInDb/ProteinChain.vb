Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class ProteinChain : Inherits ITextFile
    Implements I_PolymerSequenceModel

    <XmlAttribute> Public Property PdbId As String
    <XmlAttribute> Public Property ChainId As String
    <XmlElement> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
    <XmlAttribute> Public Property Surface As Integer()
    ''' <summary>
    ''' {<see cref="ProteinChain.ChainId">ChainId</see>, SurfaceSite}
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement> Public Property InterfaceOnSurface As KeyValuePairObject(Of String, Integer())()

    Public Function ToFASTA() As FASTA.FastaToken
        Dim FsaObject As FASTA.FastaToken = New FASTA.FastaToken With {.SequenceData = SequenceData}
        FsaObject.Attributes = New String() {String.Format("[PdbId:={0}] [ChainId:={1}]", PdbId, ChainId)}
        Return FsaObject
    End Function

    Public Shared Function TryParse(FilePath As String) As ProteinChain
        Dim strBuffer As String() = IO.File.ReadAllLines(FilePath)
        Dim ProteinChainFile As ProteinChain = New ProteinChain With {
            .FilePath = FilePath,
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

    Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
        Throw New NotImplementedException()
    End Function
End Class
