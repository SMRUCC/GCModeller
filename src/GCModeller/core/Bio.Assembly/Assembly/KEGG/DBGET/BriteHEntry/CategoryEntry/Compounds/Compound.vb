#Region "Microsoft.VisualBasic::a4e5a615604aaaedb1b2964e454fb323, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Compounds\Compound.vb"

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

    '     Class CompoundBrite
    ' 
    '         Properties: [class], category, entry, order, subcategory
    ' 
    '         Function: BuildPath, DownloadCompounds, GetAllPubchemMapCompound, Lipids, LoadFile
    '                   ToString
    ' 
    '         Sub: DownloadFromResource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
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
    '''  br08005  Bioactive peptides
    '''  br08006  Endocrine disrupting compounds
    '''  br08007  Pesticides
    '''  br08008  Carcinogens
    '''  br08009  Natural toxins
    '''  br08010  Target-based classification of compounds
    ''' </remarks>
    Public Class CompoundBrite

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        Public Property subcategory As String
        ''' <summary>
        ''' D
        ''' </summary>
        ''' <returns></returns>
        Public Property order As String
        ''' <summary>
        ''' ``{compoundID => name}``
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As KeyValuePair

        ''' <summary>
        ''' KEGG BRITE contains a classification of lipids
        ''' 
        ''' > http://www.kegg.jp/kegg-bin/get_htext?br08002.keg
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Lipids() As CompoundBrite()
            Dim satellite As New ResourcesSatellite(GetType(LICENSE))
            Return CompoundTextModel.Build(BriteHTextParser.Load(satellite.GetString(cpd_br08002))).ToArray
        End Function

        Public Overrides Function ToString() As String
            Return entry.ToString
        End Function

#Region "Internal resource ID"

        ''' <summary>
        ''' ``br08001``  Compounds with biological roles
        ''' </summary>
        Const cpd_br08001 = "br08001"
        ''' <summary>
        ''' ``br08002``  Lipids
        ''' </summary>
        Const cpd_br08002 = "br08002"
        ''' <summary>
        ''' ``br08003``  Phytochemical compounds
        ''' </summary>
        Const cpd_br08003 = "br08003"
        ''' <summary>
        ''' ``br08005``  Bioactive peptides
        ''' </summary>
        Const cpd_br08005 = "br08005"
        ''' <summary>
        ''' ``br08006``  Endocrine disrupting compounds
        ''' </summary>
        Const cpd_br08006 = "br08006"
        ''' <summary>
        ''' ``br08007``  Pesticides
        ''' </summary>
        Const cpd_br08007 = "br08007"
        ''' <summary>
        ''' ``br08008``  Carcinogens
        ''' </summary>
        Const cpd_br08008 = "br08008"
        ''' <summary>
        ''' ``br08009``  Natural toxins
        ''' </summary>
        Const cpd_br08009 = "br08009"
        ''' <summary>
        ''' ``br08010``  Target-based classification of compounds
        ''' </summary>
        Const cpd_br08010 = "br08010"

#End Region

        Public Shared Function GetAllPubchemMapCompound() As String()
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

        Public Shared Function GetLipids() As CompoundBrite()
            Return GetInformation(cpd_br08002)
        End Function

        Public Shared Function GetCompoundsWithBiologicalRoles() As CompoundBrite()
            Return GetInformation(cpd_br08001)
        End Function

        Private Shared Function GetInformation(resourceName As String) As CompoundBrite()
            Static satellite As New ResourcesSatellite(GetType(LICENSE))

            Dim htext = BriteHTextParser.Load(satellite.GetString(resourceName))
            Dim compounds = Build(htext).ToArray

            Return compounds
        End Function

        ''' <summary>
        ''' 请注意，这个函数只能够下载包含有分类信息的化合物，假若代谢物还没有分类信息的话，则无法利用这个函数进行下载
        ''' (gif图片是以base64编码放在XML文件里面的)
        ''' 
        ''' + ``br08001``  Compounds with biological roles
        ''' + ``br08002``  Lipids
        ''' + ``br08003``  Phytochemical compounds
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
        Public Shared Sub DownloadFromResource(EXPORT$, Optional directoryOrganized As Boolean = True, Optional structInfo As Boolean = False)
            Dim satellite As New ResourcesSatellite(GetType(LICENSE))
            Dim resource = {
                New NamedValue(Of CompoundBrite())("Compounds with biological roles", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08001))).ToArray),
                New NamedValue(Of CompoundBrite())("Lipids", CompoundBrite.Lipids),
                New NamedValue(Of CompoundBrite())("Phytochemical compounds", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08003))).ToArray),
                New NamedValue(Of CompoundBrite())("Bioactive peptides", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08005))).ToArray),
                New NamedValue(Of CompoundBrite())("Endocrine disrupting compounds", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08006))).ToArray),
                New NamedValue(Of CompoundBrite())("Pesticides", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08007))).ToArray),
                New NamedValue(Of CompoundBrite())("Carcinogens", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08008))).ToArray),
                New NamedValue(Of CompoundBrite())("Natural toxins", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08009))).ToArray),
                New NamedValue(Of CompoundBrite())("Target-based classification of compounds", Build(BriteHTextParser.Load(satellite.GetString(cpd_br08010))).ToArray)
            }

            For Each entry As NamedValue(Of CompoundBrite()) In resource
                With entry
                    Call .Value.ExecuteDownloads(.Name, EXPORT, directoryOrganized, structInfo)
                End With
            Next

            Dim success As Index(Of String) = (ls - l - r - "*.xml" <= EXPORT) _
                .Select(AddressOf BaseName) _
                .Indexing
            Dim allPubchemMaps = GetAllPubchemMapCompound()
            Dim saveDIR = EXPORT & "/OtherUnknowns/"
            Dim query As New DbGetWebQuery($"{saveDIR}/.cache")
            Dim details$

            Using progress As New ProgressBar($"Downloads others, {success.Count} success was indexed!", 1, CLS:=True)
                Dim tick As New ProgressProvider(allPubchemMaps.Length)

                For Each id As String In allPubchemMaps
                    If Not id Like success Then
                        Call query.Download(id, $"{saveDIR}/{id}.xml", structInfo, Nothing)
                    End If

                    details = $"ETA={tick.ETA(progress.ElapsedMilliseconds)}"
                    details = id & "   " & details

                    progress.SetProgress(tick.StepProgress, details)
                Next
            End Using
        End Sub

        Public Function BuildPath(EXPORT$, directoryOrganized As Boolean, Optional class$ = "") As String
            With Me
                If directoryOrganized Then
                    Dim t As New List(Of String) From {
                        EXPORT,
                        BriteHText.NormalizePath(.class),
                        BriteHText.NormalizePath(.category),
                        BriteHText.NormalizePath(.subcategory)
                    }

                    If Not [class].StringEmpty Then
                        Call t.Insert(index:=1, item:=[class])
                    End If

                    Return String.Join("/", t)
                Else
                    Return EXPORT
                End If
            End With
        End Function

        ''' <summary>
        ''' 函数返回失败的编号列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="BriefFile"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <returns></returns>
        Public Shared Function DownloadCompounds(EXPORT$, briefFile$, Optional directoryOrganized As Boolean = True) As String()
            Dim BriefEntries As CompoundBrite() = LoadFile(briefFile)
            Dim failures As New List(Of String)

            For Each entry As CompoundBrite In BriefEntries
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

        Public Shared Function LoadFile(path As String) As CompoundBrite()
            Return Build(BriteHTextParser.Load(path.SolveStream)).ToArray
        End Function
    End Class
End Namespace
