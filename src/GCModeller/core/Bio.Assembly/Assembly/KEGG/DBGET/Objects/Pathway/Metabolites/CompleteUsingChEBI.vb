#Region "Microsoft.VisualBasic::34246b03c75bd9de19770b071d521d8d, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\CompleteUsingChEBI.vb"

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

    '   Total Lines: 80
    '    Code Lines: 56
    ' Comment Lines: 11
    '   Blank Lines: 13
    '     File Size: 3.31 KB


    '     Module MetaboliteWebApi
    ' 
    '         Function: CompleteUsingChEBI
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

Namespace Assembly.KEGG.DBGET.bGetObject

    Partial Module MetaboliteWebApi

        Const ChEBI$ = NameOf(ChEBI) & "_unclassified"

        ''' <summary>
        ''' 所下载的数据都会被放在一个名字叫做<see cref="ChEBI"/>的文件夹之中
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <param name="accessionTsv$"></param>
        ''' <param name="forceUpdate"></param>
        ''' <returns></returns>
        Public Function CompleteUsingChEBI(DIR$, accessionTsv$, Optional forceUpdate As Boolean = False) As String()
            Dim accs As Accession() = Accession _
                .Load(accessionTsv, type:="KEGG COMPOUND accession") _
                .Values _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .ToArray
            ' 这里是已经下载的文件列表
            Dim downloads As New Index(Of String)((ls - l - r - "*.xml" <= DIR).Select(AddressOf BaseName))
            Dim path$
            Dim failures As New List(Of String)
            Dim ETA$

            DIR = DIR & "/" & ChEBI

            Using progress As New ProgressBar("Download missing ChEBI compounds data...", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, accs.Length)

                Call $"Have {downloads.Count} compounds have been downloaded...".__DEBUG_ECHO

                For Each acc As Accession In accs

                    If downloads(acc.ACCESSION_NUMBER) > -1 Then
                        ' 已经有数据了，直接跳过
                        Call $"Skip download compounds model: {acc.GetJson}".__DEBUG_ECHO
                        GoTo EXIT_LOOP
                    Else
                        path = DIR & "/" & acc.ACCESSION_NUMBER & ".xml"
                    End If

                    ' 检查是否存在
                    If Not forceUpdate AndAlso path.FileExists(ZERO_Nonexists:=True) Then
                        Call $"Skip download compounds model: {acc.GetJson}".__DEBUG_ECHO
                        GoTo EXIT_LOOP
                    Else
                        Thread.Sleep(1000)
                    End If

                    Dim cpd As Compound = MetaboliteWebApi.DownloadCompound(acc.ACCESSION_NUMBER)

                    If cpd Is Nothing Then
                        ' 没有下载成功
                        failures += acc.ACCESSION_NUMBER
                    Else
                        Call cpd.SaveAsXml(path)
                    End If
EXIT_LOOP:
                    ETA = tick.ETA().FormatTime
                    Call progress.SetProgress(
                        tick.StepProgress,
                        details:=ETA)
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace
