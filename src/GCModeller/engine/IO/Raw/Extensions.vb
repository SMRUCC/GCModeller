#Region "Microsoft.VisualBasic::6b62ceff59d2b4e48b0d0b951844f06f, engine\IO\Raw\Extensions.vb"

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

    '   Total Lines: 93
    '    Code Lines: 76 (81.72%)
    ' Comment Lines: 7 (7.53%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 10 (10.75%)
    '     File Size: 3.38 KB


    ' Module Extensions
    ' 
    '     Function: GetMatrix, (+2 Overloads) GetTimeFrames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports HTS_Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Iterator Function GetMatrix(raw As Raw.Reader, module$) As IEnumerable(Of DataSet)
        For Each time As Double In raw.AllTimePoints
            Yield New DataSet With {
                .ID = time,
                .Properties = raw.Read(time, [module])
            }
        Next
    End Function

    <Extension>
    Public Function GetTimeFrames(raw As Raw.Reader, modu As String) As HTS_Matrix
        Dim list As String() = raw.ModuleIdSet(modu).Objects
        Dim t As Double() = raw.AllTimePoints.ToArray
        Dim ticks As New List(Of DataFrameRow)
        Dim ds As Dictionary(Of String, Double)

        For Each ti As Double In t
            ds = raw.Read(time:=ti, modu)
            ticks.Add(New DataFrameRow With {
                .geneID = ti,
                .experiments = list.Select(Function(i) ds(i)).ToArray
            })
        Next

        Return New HTS_Matrix With {
            .expression = ticks.ToArray,
            .sampleID = list,
            .tag = NameOf(GetTimeFrames)
        }
    End Function

    ''' <summary>
    ''' 读取netCDF格式的模拟计算结果数据文件
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function GetTimeFrames(raw As vcXML.Reader, modu As String, type As String) As HTS_Matrix
        ' get offset index for read data from raw data xml file
        Dim index As XmlOffset() = raw.getStreamIndex(modu)(type) _
            .OrderBy(Function(p) p.id) _
            .ToArray
        ' each row is feature item
        ' and the column value is the time stream data
        Dim entities As DataSet() = raw _
            .getStreamEntities(index(Scan0).module, index(Scan0).content_type) _
            .Select(Function(id)
                        Return New DataSet With {
                            .ID = id,
                            .Properties = New Dictionary(Of String, Double)
                        }
                    End Function) _
            .ToArray
        Dim vector As Double()

        For Each offset As XmlOffset In index
            vector = raw.getFrameVector(offset.offset)

            For i As Integer = 0 To vector.Length - 1
                entities(i).Add(offset.id, vector(i))
            Next
        Next

        Dim timeTicks As String() = index _
            .Select(Function(o) o.id.ToString) _
            .ToArray
        Dim matrix As New HTS_Matrix With {
            .sampleID = timeTicks,
            .tag = raw.ToString,
            .expression = entities _
                .Select(Function(v)
                            Return New DataFrameRow With {
                                .geneID = v.ID,
                                .experiments = v(timeTicks)
                            }
                        End Function) _
                .ToArray
        }

        Return matrix
    End Function
End Module
