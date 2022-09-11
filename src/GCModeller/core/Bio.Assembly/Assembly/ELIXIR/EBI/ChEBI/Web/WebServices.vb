#Region "Microsoft.VisualBasic::1b6cf4a0216328cdf7d2281ab123c3ef, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Web\WebServices.vb"

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

    '   Total Lines: 84
    '    Code Lines: 38
    ' Comment Lines: 36
    '   Blank Lines: 10
    '     File Size: 3.97 KB


    '     Module WebServices
    ' 
    '         Function: BatchQuery, CreateRequest, GetCompleteEntity, QueryChEBI
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace Assembly.ELIXIR.EBI.ChEBI.WebServices

    ''' <summary>
    ''' The main aim of ChEBI Web Services is to provide programmatic access 
    ''' to the ChEBI database in order to aid our users in integrating ChEBI 
    ''' into their applications. Web Services is an integration technology. 
    ''' To ensure software from various sources work well together, this 
    ''' technology is built on open standards such as Simple Object Access 
    ''' Protocol (SOAP), a messaging protocol for transporting information, 
    ''' Web Services Description Language (WSDL), a standard method of describing 
    ''' Web Services and their capabilities. For the transport layer itself, 
    ''' Web Services utilise most of the commonly available network protocols, 
    ''' especially Hypertext Transfer Protocol (HTTP).
    '''
    ''' ChEBI provides SOAP access To its database. If you just wish To obtain 
    ''' light weight ontology objects you can use the Ontology Lookup Service 
    ''' As alternative Web Services.
    ''' 
    ''' > https://www.ebi.ac.uk/chebi/webServices.do
    ''' </summary>
    Public Module WebServices

        Public Function CreateRequest(chebiId As String) As String
            Dim cid As String = Strings.Trim(chebiId).Split(":"c).Last
            Dim url$ = $"http://www.ebi.ac.uk/webservices/chebi/2.0/test/getCompleteEntity?chebiId={cid}"

            Return url
        End Function

        ''' <summary>
        ''' Retrieves the complete entity including synonyms, database links and chemical structures, using the ChEBI identifier.
        ''' </summary>
        ''' <param name="chebiId$"></param>
        ''' <returns></returns>
        Public Function GetCompleteEntity(chebiId$) As ChEBIEntity()
            Dim url$ = WebServices.CreateRequest(chebiId)
            Dim xml$ = url.GET
            Dim out As ChEBIEntity() = REST.ParsingRESTData(xml)
            Return out
        End Function

        ''' <summary>
        ''' 查询在线数据或者读取本地的缓存数据(ChEBI的编号<paramref name="chebiID"/>参数必须为纯数字的编号参数)
        ''' </summary>
        ''' <param name="chebiID$">纯数字的编号，不能够带有``chebi:``前缀</param>
        ''' <param name="localCache$">函数会分别使用主ID和二级ID构建缓存数据</param>
        ''' <returns></returns>
        <Extension>
        Public Function QueryChEBI(chebiID$, localCache$) As ChEBIEntity()
            Return New QueryImpl(localCache).Query(Of ChEBIEntity())(chebiID)
        End Function

        ''' <summary>
        ''' 执行批量数据查询
        ''' </summary>
        ''' <param name="chebiIDlist">编号列表是不带有``CHEBI:``前缀的chebi物质编号</param>
        ''' <param name="localCache$"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function BatchQuery(chebiIDlist$(), localCache$, Optional sleepInterval% = 2000) As IEnumerable(Of ChEBIEntity)
            Using progress As New ProgressBar("Downloading ChEBI data...")
                Dim tick As New ProgressProvider(progress, chebiIDlist.Length)
                Dim ETA$
                Dim query As New QueryImpl(localCache, sleep:=sleepInterval)

                For Each id As String In chebiIDlist
                    For Each part As ChEBIEntity In query.Query(Of ChEBIEntity())(id).SafeQuery
                        Yield part
                    Next

                    ETA = $"ETA=" & tick.ETA().FormatTime

                    progress.SetProgress(tick.StepProgress, ETA)
                Next
            End Using
        End Function
    End Module
End Namespace
