#Region "Microsoft.VisualBasic::d633431bc609df92143b1ab69f636d8e, LocalBLAST\Pipeline\COG\COGsUtils.vb"

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

    '     Module COGsUtils
    ' 
    '         Function: (+2 Overloads) MyvaCOGCatalog
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG.Whog

Namespace Pipeline.COG

    ''' <summary>
    ''' Cog分类操作
    ''' </summary>
    ''' <remarks></remarks>
    Public Module COGsUtils

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="blastp">raw blastp output file path.</param>
        ''' <param name="descriptParser"></param>
        ''' <returns></returns>
        Public Function MyvaCOGCatalog(blastp$, whogXml$,
                                       Optional identities# = 0.15,
                                       Optional coverage# = 0.5,
                                       Optional descriptParser As TextGrepMethod = Nothing,
                                       Optional ALL As Boolean = False) As MyvaCOG()

            If Not blastp.FileExists Then
                Return {}
            End If

            Dim outputRaw As v228 = Parser.TryParse(blastp)
            Dim bbh As BestHit()

            If ALL Then
                bbh = outputRaw.ExportAllBestHist(coverage, identities)
            Else
                bbh = outputRaw.ExportBestHit(coverage, identities)
            End If

            Return bbh.MyvaCOGCatalog(whogXml, descriptParser)
        End Function

        ''' <summary>
        ''' <see cref="MyvaCOG.CreateObject"/>.
        ''' (因为所输入进来的函数参数<paramref name="bbh"/>是已经通过参数筛选之后的结果，所以这个函数就不需要再使用参数筛选了。)
        ''' </summary>
        ''' <param name="bbh"></param>
        ''' <param name="whogXml"></param>
        ''' <param name="descriptParser">
        ''' 解析出來的是query protein的fasta標題裏面的功能注釋信息
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function MyvaCOGCatalog(bbh As IEnumerable(Of BestHit), whogXml As String, Optional descriptParser As TextGrepMethod = Nothing) As MyvaCOG()
            Dim source As BestHit() = bbh.ToArray
            Dim queriesName$() = LinqAPI.Exec(Of String) <=
 _
                From query As BestHit
                In source
                Select query.QueryName

            Dim MyvaCOG As MyvaCOG() =
                source _
                .Select(AddressOf Pipeline.COG.MyvaCOG.CreateObject) _
                .ToArray

            If Not descriptParser Is Nothing Then
                For i As Integer = 0 To queriesName.Length - 1
                    With MyvaCOG(i)
                        .Description = descriptParser(queriesName(i))
                    End With
                Next
            End If

            MyvaCOG = whogXml _
                .LoadXml(Of WhogRepository)() _
                .MatchCogCategory(MyvaCOG)

            Return MyvaCOG
        End Function
    End Module
End Namespace
