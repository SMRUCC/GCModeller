#Region "Microsoft.VisualBasic::4836441808d3f953ee6d64e45cde60cc, meme_suite\MEME\Analysis\MotifScanning\LDM\AnnotationModel.vb"

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

    '     Class AnnotationModel
    ' 
    '         Properties: Evalue, Expression, Motif, NumberOfSites, PspMatrix
    '                     PWM, Sites, SourceLen, Uid, Width
    ' 
    '         Function: Complement, CreateObject, LoadDocument, LoadLDM, LoadMEMEOUT
    '                   MEME_UID, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat

Namespace Analysis.MotifScans

    ''' <summary>
    ''' MAST_LDM for motif annotation.
    ''' </summary>
    Public Class AnnotationModel : Implements INamedValue

        <XmlAttribute> Public Property Evalue As Double
        Public Property Sites As Site()
            Get
                Return __sites
            End Get
            Set(value As Site())
                __sites = value
                If value.IsNullOrEmpty Then
                    __siteHash = New Dictionary(Of Site)
                Else
                    __siteHash = value.ToDictionary
                End If
            End Set
        End Property
        <XmlAttribute> Public Property Width As Integer
        Public Property PWM As ResidueSite()
        <XmlAttribute> Public Property Uid As String Implements INamedValue.Key
        <XmlAttribute> Public Property Expression As String
        <XmlAttribute("bp")> Public Property SourceLen As Integer

        Public ReadOnly Property NumberOfSites As Integer
            Get
                If Sites Is Nothing Then
                    Return 0
                End If
                Return Sites.Length
            End Get
        End Property

        Dim __sites As Site()
        Dim __siteHash As Dictionary(Of Site)

        Default Public ReadOnly Property Site(name As String) As Site
            Get
                Return __siteHash(name)
            End Get
        End Property

        ''' <summary>
        ''' 互补反向
        ''' </summary>
        ''' <returns></returns>
        Public Function Complement() As AnnotationModel
            Dim motif = PWM.Select(Function(x) x.Complement)
            Call Array.Reverse(motif)
            Call motif.WriteAddress

            Dim LDM As New AnnotationModel With {
                .Evalue = Evalue,
                .PWM = motif,
                .Sites = Sites,
                .Uid = Uid,
                .Width = Width
            }
            Return LDM
        End Function

        Public Overrides Function ToString() As String
            Return Uid
        End Function

        Public ReadOnly Property PspMatrix As MotifPM()
            Get
                If _PspMatrix Is Nothing Then
                    _PspMatrix = PWM.Select(Function(rsd) rsd.ToNtBase)
                End If
                Return _PspMatrix
            End Get
        End Property
        ''' <summary>
        ''' 在<see cref="PspMatrix"/>属性之中进行延时加载
        ''' </summary>
        Dim _PspMatrix As MotifPM()

        Public ReadOnly Property Motif As String
            Get
                Return New String(PWM.Select(Function(x) TOMQuery.TomTOm.ToChar(x)))
            End Get
        End Property

        ''' <summary>
        ''' 从指定路径的meme.txt文档之中，解析meme.txt文档得到Motif的模型
        ''' </summary>
        ''' <param name="memeText">meme_out.txt的文件路径</param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(memeText As String, Optional uidPrefix As String = "") As AnnotationModel()
            Dim uid As String = BaseName(memeText)
            If Not String.IsNullOrEmpty(uidPrefix) Then
                uid = uidPrefix & "::" & uid
            End If
            Dim len As Integer = MEME.Text.GetLength(memeText)
            Dim Motifs As AnnotationModel() =
                MEME.Text.SafelyLoad(memeText).Select(AddressOf AnnotationModel.CreateObject)

            For Each motif As AnnotationModel In Motifs
                motif.Uid = $"{uid}.{motif.Uid}"
                motif.SourceLen = len
            Next

            Return Motifs
        End Function

        Public Shared Function CreateObject(Motif As MEME.LDM.Motif) As AnnotationModel
            Return New AnnotationModel With {
                .Sites = Motif.Sites.Select(Function(site) Analysis.MotifScans.Site.CreateObject(site)),
                .Evalue = Motif.Evalue,
                .Width = Motif.Width,
                .PWM = Motif.PspMatrix.Select(Function(site) New ResidueSite With {
                    .Bits = site.Bits,
                    .PWM = New Double() {site.A, site.T, site.G, site.C}}),
                .Expression = Motif.Signature,
                .Uid = Motif.Id
            }
        End Function

        Public Shared Function MEME_UID(path As String) As String
            Dim name As String = path.Replace("\", "/").Split("/"c).Last
            If String.Equals(name, "meme.txt", StringComparison.OrdinalIgnoreCase) Then
                Return path.ParentDirName
            Else
                Return path.BaseName
            End If
        End Function

        ''' <summary>
        ''' 加载自带的源里面的模型数据
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function LoadLDM(Optional source As String = "") As Dictionary(Of AnnotationModel)
            If String.IsNullOrEmpty(source) Then
                source = GCModeller.FileSystem.GetMotifLDM
            End If
            Dim Files As String() = FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").ToArray
            Dim loads = Files.Select(Function(x) x.LoadXml(Of AnnotationModel))
            Return loads.ToDictionary
        End Function

        ''' <summary>
        ''' Load models from meme output DIR
        ''' </summary>
        ''' <param name="source">MEME.txt DIR</param>
        ''' <param name="full">
        ''' If full, then <see cref="FileIO.SearchOption.SearchAllSubDirectories"/>
        ''' Else <see cref="FileIO.SearchOption.SearchTopLevelOnly"/>
        ''' </param>
        ''' <returns></returns>
        Public Shared Function LoadMEMEOUT(source As String, Optional full As Boolean = False, Optional trimMotifId As Boolean = False) As Dictionary(Of AnnotationModel)
            Dim search As FileIO.SearchOption =
                If(full, FileIO.SearchOption.SearchAllSubDirectories, FileIO.SearchOption.SearchTopLevelOnly)
            Dim files As String() = FileIO.FileSystem.GetFiles(source, search, "*.txt").ToArray
            Dim models As IEnumerable(Of AnnotationModel)

            If full Then
                Dim LDMs = (From file As String
                            In files
                            Let parent As String = file.ParentDirName
                            Let LDM As AnnotationModel() = AnnotationModel.LoadDocument(file, uidPrefix:=parent)
                            Select LDM)
                models = LDMs.Unlist
            Else
                models = files.Select(Function(x) AnnotationModel.LoadDocument(x)).Unlist
            End If

            If trimMotifId Then
                Dim hash As New Dictionary(Of AnnotationModel)(models.ToDictionary(Function(x) Strings.Split(x.Uid, "::").Last))
                Return hash
            Else
                Dim hash As Dictionary(Of AnnotationModel) = models.ToDictionary
                Return hash
            End If
        End Function
    End Class
End Namespace
