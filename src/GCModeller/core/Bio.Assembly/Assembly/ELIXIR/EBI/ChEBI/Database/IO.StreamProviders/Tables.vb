#Region "Microsoft.VisualBasic::76ed38aa781610d639e29c70669165e8, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Database\IO.StreamProviders\Tables.vb"

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

    '   Total Lines: 182
    '    Code Lines: 101
    ' Comment Lines: 55
    '   Blank Lines: 26
    '     File Size: 6.80 KB


    '     Class Entity
    ' 
    '         Properties: COMPOUND_ID, ID, SOURCE, TYPE
    ' 
    '         Function: ToString
    ' 
    '     Class ChemicalData
    ' 
    '         Properties: CHEMICAL_DATA
    ' 
    '         Function: ChemicalModel, ppm, PpmToMassDelta, ToString
    ' 
    '     Class Names
    ' 
    '         Properties: ADAPTED, LANGUAGE, NAME
    ' 
    '         Function: ToString
    ' 
    '     Class InChI
    ' 
    '         Properties: CHEBI_ID, InChI
    ' 
    '     Class Accession
    ' 
    '         Properties: ACCESSION_NUMBER
    ' 
    '         Function: Load, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

    Public Class Entity

        ''' <summary>
        ''' 数据表之中的自增ID，无意义
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
        ''' <summary>
        ''' ChEBI编号
        ''' </summary>
        ''' <returns></returns>
        Public Property COMPOUND_ID As String
        ''' <summary>
        ''' 编号类型
        ''' </summary>
        ''' <returns></returns>
        Public Property TYPE As String
        ''' <summary>
        ''' 数据库名称
        ''' </summary>
        ''' <returns></returns>
        Public Property SOURCE As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", TYPE, SOURCE, ID)
        End Function
    End Class

    ''' <summary>
    ''' ``chemical_data.tsv``
    ''' </summary>
    Public Class ChemicalData : Inherits Entity

        Public Property CHEMICAL_DATA As String

        ''' <summary>
        ''' 分子量差值
        ''' </summary>
        ''' <param name="measured#"></param>
        ''' <param name="actualValue#"></param>
        ''' <returns></returns>
        Public Shared Function ppm(measured#, actualValue#) As Double
            ' （测量值-实际分子量）/实际分子量
            ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
            Dim ppmd# = stdNum.Abs(measured - actualValue) / actualValue
            ppmd = ppmd * 1000000
            Return ppmd
        End Function

        Public Shared Function PpmToMassDelta(measure As Double, ppm As Double) As Double
            Dim da As Double

            ' (measured - actualValue) / actualValue = ppm
            ' ppm * actualValue = measured - actualValue
            ' ppm * actualValue + actualValue = measured
            ' measure + da = measured / (1 + ppm) 
            ' da = measured / (1 + ppm) - measure
            ppm = ppm / 10 ^ 6
            da = measure / (1 + ppm) - measure

            Return stdNum.Abs(da)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ChemicalModel(table As IEnumerable(Of ChemicalData)) As Dictionary(Of String, Dictionary(Of String, ChemicalData()))
            Return table _
                .GroupBy(Function(c) c.COMPOUND_ID) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c _
                                      .GroupBy(Function(x) x.TYPE) _
                                      .ToDictionary(Function(t) t.Key,
                                                    Function(list)
                                                        Return list.ToArray
                                                    End Function)
                              End Function)
        End Function

        Public Overrides Function ToString() As String
            Return $"[{COMPOUND_ID}] {TYPE}  {CHEMICAL_DATA}"
        End Function
    End Class

    Public Class Names : Inherits Entity

        Public Property NAME As String
        Public Property ADAPTED As String
        Public Property LANGUAGE As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) [{1}]{2}", ID, LANGUAGE, NAME)
        End Function
    End Class

    ''' <summary>
    ''' ``chebiId_inchi.tsv``
    ''' </summary>
    Public Class InChI
        Public Property CHEBI_ID As String
        Public Property InChI As String
    End Class

    ''' <summary>
    ''' Database xref dblinks.(ChEBI的ftp服务器之上的``database_accession.tsv``文件数据的解析器)
    ''' </summary>
    ''' <remarks>
    ''' ftp://ftp.ebi.ac.uk/pub/databases/chebi/Flat_file_tab_delimited/
    ''' </remarks>
    Public Class Accession : Inherits Entity

        ''' <summary>
        ''' 在其他数据库之中的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ACCESSION_NUMBER As String

        Public Overrides Function ToString() As String
            Return ACCESSION_NUMBER
        End Function

        ''' <summary>
        ''' 返回的字典的键名是chebi编号
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="type$">
        ''' 例如假若只需要KEGG代谢物的编号的话，则指定这个参数的值为``KEGG COMPOUND accession``
        ''' 这个参数为空值的话是不会进行任何过滤操作的
        ''' </param>
        ''' <returns></returns>
        Public Shared Function Load(path$, Optional type$ = Nothing) As Dictionary(Of NamedCollection(Of Accession))
            Dim index As Index(Of String) = path.TsvHeaders
            Dim table As New List(Of Accession)
            Dim IDtype As New Value(Of String)

            Dim test = Function(t$)
                           Return type = t
                       End Function
            ' 假若所指定的类型参数值为空的话，则不会进行筛选过滤
            If type.StringEmpty Then
                test = Function(t$) True
            End If

            For Each line As String In path.IterateAllLines.Skip(1)
                Dim t$() = line.Split(ASCII.TAB)

                If Not test(IDtype = t(index(NameOf(Accession.TYPE)))) Then
                    Continue For
                End If

                table += New Accession With {
                    .ACCESSION_NUMBER = t(index(NameOf(ACCESSION_NUMBER))),
                    .COMPOUND_ID = t(index(NameOf(COMPOUND_ID))),
                    .ID = t(index(NameOf(ID))),
                    .SOURCE = t(index(NameOf(SOURCE))),
                    .TYPE = IDtype
                }
            Next

            Dim ChEBI As Dictionary(Of NamedCollection(Of Accession)) =
                table _
                .GroupBy(Function(t) t.COMPOUND_ID) _
                .Select(Function(t)
                            Return New NamedCollection(Of Accession) With {
                                .name = t.Key,
                                .value = t.ToArray
                            }
                        End Function) _
                .ToDictionary

            Return ChEBI
        End Function
    End Class
End Namespace
