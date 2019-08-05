#Region "Microsoft.VisualBasic::8d6ceb3270229e8a11b9bad601a4487c, Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\ReactionWebAPI.vb"

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

    '     Module ReactionWebAPI
    ' 
    '         Function: Download, FetchTo, ValueList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module ReactionWebAPI

        ''' <summary>
        ''' 使用ID来下载代谢过程的模型数据
        ''' </summary>
        ''' <param name="ID">编号格式为：``R\d+``，例如R00259</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Download(ID As String, Optional cache$ = "./reactions/", Optional sleepTime% = 3000) As Reaction
            Static handlers As New Dictionary(Of String, ReactionQuery)

            Return handlers.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New ReactionQuery(cache, sleepTime)
                           End Function) _
               .Query(Of Reaction)(ID)
        End Function

        <Extension>
        Friend Function ValueList(keys As IEnumerable(Of KeyValuePair)) As NamedValue()
            Return keys _
                .Select(Function(k)
                            Return New NamedValue With {
                                .name = k.Key,
                                .text = k.Value
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回成功下载的对象的数目</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT$) As String()
            Dim failures As New List(Of String)

            For Each ID As String In list
                Dim r As Reaction = Download(ID)

                If r Is Nothing Then
                    failures += ID
                Else
                    Dim path$ = String.Format("{0}/{1}.xml", EXPORT, ID)
                    Call r.GetXml.SaveTo(path)
                End If
            Next

            Return failures.ToArray
        End Function
    End Module
End Namespace
