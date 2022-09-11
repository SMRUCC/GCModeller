#Region "Microsoft.VisualBasic::77ffd444edc3d35fc0a2a4b533132e12, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Compound.vb"

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

    '   Total Lines: 254
    '    Code Lines: 141
    ' Comment Lines: 80
    '   Blank Lines: 33
    '     File Size: 10.94 KB


    '     Module CompoundBrite
    ' 
    '         Function: BioactivePeptides, Carcinogens, CompoundsWithBiologicalRoles, DownloadCompounds, EndocrineDisruptingCompounds
    '                   GetAllCompoundResources, GetAllPubchemMapCompound, Glycosides, Lipids, LoadFile
    '                   NaturalToxins, Pesticides, PhytochemicalCompounds, TargetbasedClassificationOfCompounds
    ' 
    '         Sub: DownloadFromResource, DownloadOthers
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery.Compounds

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' Compounds with Biological Roles.(在这里面包含有KEGG compounds的下载API)
    ''' </summary>
    ''' <remarks>
    ''' Compounds
    ''' 
    '''  br08001  Compounds with biological roles
    '''  br08002  Lipids
    '''  br08003  Phytochemical compounds
    '''  br08021  Glycosides
    '''  br08005  Bioactive peptides
    '''  br08006  Endocrine disrupting compounds
    '''  br08007  Pesticides
    '''  br08008  Carcinogens
    '''  br08009  Natural toxins
    '''  br08010  Target-based classification of compounds
    ''' </remarks>
    Public Module CompoundBrite

#Region "Internal resource ID"

        ''' <summary>
        ''' ``br08001``  Compounds with biological roles
        ''' </summary>
        Public Const cpd_br08001 = "br08001"
        ''' <summary>
        ''' ``br08002``  Lipids
        ''' </summary>
        Public Const cpd_br08002 = "br08002"
        ''' <summary>
        ''' ``br08003``  Phytochemical compounds
        ''' </summary>
        Public Const cpd_br08003 = "br08003"
        ''' <summary>
        ''' ``br08005``  Bioactive peptides
        ''' </summary>
        Public Const cpd_br08005 = "br08005"
        ''' <summary>
        ''' ``br08006``  Endocrine disrupting compounds
        ''' </summary>
        Public Const cpd_br08006 = "br08006"
        ''' <summary>
        ''' ``br08007``  Pesticides
        ''' </summary>
        Public Const cpd_br08007 = "br08007"
        ''' <summary>
        ''' ``br08008``  Carcinogens
        ''' </summary>
        Public Const cpd_br08008 = "br08008"
        ''' <summary>
        ''' ``br08009``  Natural toxins
        ''' </summary>
        Public Const cpd_br08009 = "br08009"
        ''' <summary>
        ''' ``br08010``  Target-based classification of compounds
        ''' </summary>
        Public Const cpd_br08010 = "br08010"

        ''' <summary>
        ''' ``br08021``  Glycosides
        ''' </summary>
        Public Const cpd_br08021 = "br08021"

#End Region

        Const CompoundIDPattern$ = "[DCG]\d+"

        Public Function GetAllPubchemMapCompound() As String()
            Dim satellite As New ResourcesSatellite(GetType(LICENSE))
            Dim data = satellite.GetString("SID_Map_KEGG")
            Dim id As String() = data.LineTokens _
                .Select(Function(line)
                            Return line _
                                .Split(ASCII.TAB) _
                                .ElementAtOrDefault(2)
                        End Function) _
                .Where(Function(cid) cid.StartsWith("C")) _
                .ToArray

            Return id
        End Function

        ''' <summary>
        ''' KEGG BRITE contains a classification of lipids
        ''' 
        ''' > http://www.kegg.jp/kegg-bin/get_htext?br08002.keg
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Lipids() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08002, CompoundIDPattern)
        End Function

        Public Function Glycosides() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08021, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CompoundsWithBiologicalRoles() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08001, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PhytochemicalCompounds() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08003, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function BioactivePeptides() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08005, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function EndocrineDisruptingCompounds() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08006, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Pesticides() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08007, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Carcinogens() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08008, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function NaturalToxins() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08009, CompoundIDPattern)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TargetbasedClassificationOfCompounds() As BriteTerm()
            Return BriteTerm.GetInformation(cpd_br08010, CompoundIDPattern)
        End Function

        Public Iterator Function GetAllCompoundResources() As IEnumerable(Of NamedValue(Of BriteTerm()))
            Yield New NamedValue(Of BriteTerm())("Compounds with biological roles", CompoundsWithBiologicalRoles)
            Yield New NamedValue(Of BriteTerm())("Lipids", Lipids)
            Yield New NamedValue(Of BriteTerm())("Phytochemical compounds", PhytochemicalCompounds)
            Yield New NamedValue(Of BriteTerm())("Bioactive peptides", BioactivePeptides)
            Yield New NamedValue(Of BriteTerm())("Endocrine disrupting compounds", EndocrineDisruptingCompounds)
            Yield New NamedValue(Of BriteTerm())("Pesticides", Pesticides)
            Yield New NamedValue(Of BriteTerm())("Carcinogens", Carcinogens)
            Yield New NamedValue(Of BriteTerm())("Natural toxins", NaturalToxins)
            Yield New NamedValue(Of BriteTerm())("Target-based classification of compounds", TargetbasedClassificationOfCompounds)
            Yield New NamedValue(Of BriteTerm())("Glycosides", Glycosides)
        End Function

        ''' <summary>
        ''' 请注意，这个函数只能够下载包含有分类信息的化合物，假若代谢物还没有分类信息的话，则无法利用这个函数进行下载
        ''' (gif图片是以base64编码放在XML文件里面的)
        ''' 
        ''' + ``br08001``  Compounds with biological roles
        ''' + ``br08002``  Lipids
        ''' + ``br08003``  Phytochemical compounds
        ''' + ``br08021``  Glycosides
        ''' + ``br08005``  Bioactive peptides
        ''' + ``br08006``  Endocrine disrupting compounds
        ''' + ``br08007``  Pesticides
        ''' + ``br08008``  Carcinogens
        ''' + ``br08009``  Natural toxins
        ''' + ``br08010``  Target-based classification of compounds
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <param name="structInfo">是否同时也下载分子结构信息？</param>
        ''' <remarks></remarks>
        Public Sub DownloadFromResource(EXPORT$, Optional directoryOrganized As Boolean = True, Optional structInfo As Boolean = False)
            Dim satellite As New ResourcesSatellite(GetType(LICENSE))
            Dim resource = GetAllCompoundResources.ToArray

            For Each entry As NamedValue(Of BriteTerm()) In resource
                With entry
                    Call .Value.ExecuteDownloads(.Name, EXPORT, directoryOrganized, structInfo)
                End With
            Next

            Call DownloadOthers(EXPORT, GetAllPubchemMapCompound(), structInfo)
        End Sub

        Public Sub DownloadOthers(EXPORT$, compoundIds$(), Optional structInfo As Boolean = False)
            Dim success As Index(Of String) = (ls - l - r - "*.xml" <= EXPORT) _
                .Select(AddressOf BaseName) _
                .Indexing
            Dim saveDIR = EXPORT & "/OtherUnknowns/"
            Dim query As New DbGetWebQuery($"{EXPORT}/.cache")
            Dim details$

            Using progress As New ProgressBar($"Downloads others, {success.Count} success was indexed!", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, compoundIds.Length)

                For Each id As String In compoundIds
                    If Not id Like success Then
                        Call query.Download(id, $"{saveDIR}/{id.Last}/{id}.xml", structInfo, Nothing)
                    End If

                    details = $"ETA={tick.ETA().FormatTime}"
                    details = id & "   " & details

                    progress.SetProgress(tick.StepProgress, details)
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 函数返回失败的编号列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="BriefFile"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <returns></returns>
        Public Function DownloadCompounds(EXPORT$, briefFile$, Optional directoryOrganized As Boolean = True) As String()
            Dim BriefEntries As BriteTerm() = LoadFile(briefFile)
            Dim failures As New List(Of String)

            For Each entry As BriteTerm In BriefEntries
                Dim EntryId As String = entry.entry.Key
                Dim saveDIR As String = entry.BuildPath(EXPORT, directoryOrganized)
                Dim xml As String = String.Format("{0}/{1}.xml", saveDIR, EntryId)
                Dim cpd As bGetObject.Compound = MetaboliteWebApi.DownloadCompound(EntryId)

                If cpd Is Nothing Then
                    Call $"[{entry.ToString}] is not exists in the kegg!".Warning
                    failures += EntryId
                Else
                    Call cpd.GetXml.SaveTo(xml)
                End If
            Next

            Return failures
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadFile(path As String) As BriteTerm()
            Return BriteTerm.GetInformation(path, CompoundIDPattern)
        End Function
    End Module
End Namespace
