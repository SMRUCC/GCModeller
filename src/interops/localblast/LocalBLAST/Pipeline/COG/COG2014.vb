#Region "Microsoft.VisualBasic::a398921e2085e82012226207d1aaf2b0, localblast\LocalBLAST\Pipeline\COG\COG2014.vb"

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

    '     Module COG2014
    ' 
    '         Function: (+2 Overloads) COG2014_result, COGCatalog
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.COG.COGs
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline.COG

    Public Module COG2014

        <Extension>
        Public Function COG2014_result(sbh As IEnumerable(Of BestHit), cog2014 As COGTable()) As MyvaCOG()
            Return sbh.COG2014_result(COGTable.GI2COGs(cog2014))
        End Function

        <Extension>
        Public Function COG2014_result(sbh As IEnumerable(Of BestHit), gi2cogs As Dictionary(Of String, NamedValue(Of String()))) As MyvaCOG()
            Dim query = sbh.GroupBy(Function(prot) prot.QueryName)
            Dim out As New List(Of MyvaCOG)

            For Each protein As IGrouping(Of String, BestHit) In query
                ' 取最好的
                Dim best As BestHit = protein.TopHit
                Dim header As NTheader = NTheader.ParseNTheader(best.HitName.Split("|"c)).FirstOrDefault
                Dim gi$ = header.gi

                out += New MyvaCOG With {
                    .QueryName = protein.Key,
                    .QueryLength = best.query_length,
                    .LengthQuery = best.length_query,
                    .Identities = best.identities,
                    .Description = best.HitName,
                    .Evalue = best.evalue,
                    .Length = best.hit_length
                }

                If Not gi Is Nothing Then
                    Dim COG$() = gi2cogs(gi).Value

                    With out.Last
                        .COG = COG.JoinBy("; ")
                        .DataAsset = New Dictionary(Of String, String) From {
                            {"genome", gi2cogs(gi).Name}
                        }
                    End With
                End If
            Next

            Return out
        End Function

        ''' <summary>
        ''' 在<see cref="COG2014_result"/>生成的结果之中并没有<see cref="MyvaCOG.Category"/>的值，则可以在这里进行填充
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="names"></param>
        ''' <returns></returns>
        <Extension>
        Public Function COGCatalog(genes As IEnumerable(Of MyvaCOG), names As COGName()) As MyvaCOG()
            Dim table As Dictionary(Of COGName) = names.ToDictionary
            Dim out As New List(Of MyvaCOG)

            For Each protein As MyvaCOG In genes

                ' 直接使用字符串的split方法可能会因为空字符串的出现而出错，所以在这里使用vb6的split方法 
                Dim cogs$() = Strings _
                    .Split(protein.COG, ";") _
                    .Select(AddressOf Strings.Trim) _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray

                If cogs.IsNullOrEmpty OrElse (cogs.Length = 1 AndAlso cogs(Scan0) = "") Then
                    ' 空数据直接略过
                Else
                    ' 居然有些COG编号还是没有分类的
                    Dim catalogs = cogs _
                        .Where(AddressOf table.ContainsKey) _
                        .Select(Function(c) table(c)) _
                        .ToArray
                    Dim catalog$ = catalogs _
                        .Select(Function(c) c.Func.ToCharArray) _
                        .IteratesALL _
                        .Distinct _
                        .JoinBy("")

                    protein.Category = catalog
                End If

                out += protein
            Next

            Return out
        End Function
    End Module
End Namespace
