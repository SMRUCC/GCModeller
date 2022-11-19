#Region "Microsoft.VisualBasic::3aded5a5cd36794242aa4628d5bde8a3, GCModeller\core\Bio.Assembly\Assembly\KEGG\Medical\DrugParser.vb"

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

    '   Total Lines: 256
    '    Code Lines: 194
    ' Comment Lines: 25
    '   Blank Lines: 37
    '     File Size: 9.40 KB


    '     Module DrugParser
    ' 
    '         Function: __atoms, __bounds, __classGroup, CreateDrugGroupModel, CreateDrugModel
    '                   LoadDrugGroup, (+2 Overloads) ParseStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    ''' <summary>
    ''' 解析KEGG ftp服务器上面的药物数据模型
    ''' </summary>
    Public Module DrugParser

        ''' <summary>
        ''' 解析药物数据库的文件
        ''' </summary>
        ''' <param name="pathOrDoc$">文件路径或者文件的文本内容</param>
        ''' <returns></returns>
        Public Iterator Function ParseStream(pathOrDoc As String) As IEnumerable(Of Drug)
            Dim lines$() = pathOrDoc.SolveStream.LineTokens

            For Each pack As String() In lines.Split("///")
                Yield pack.ParseStream.CreateDrugModel
            Next
        End Function

        Public Function LoadDrugGroup(path$) As DrugGroup()
            Dim lines$() = path.ReadAllLines
            Dim out As DrugGroup() = lines _
                .Split("///") _
                .Select(Function(pack) pack.ParseStream.CreateDrugGroupModel) _
                .ToArray
            Return out
        End Function

        <Extension>
        Public Function CreateDrugGroupModel(getValue As Func(Of String, String())) As DrugGroup
            Return New DrugGroup With {
                .Entry = getValue("ENTRY").FirstOrDefault.Split.First,
                .Names = getValue("NAME") _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Remarks = getValue("REMARK"),
                .Comments = getValue("COMMENT").JoinBy(" "),
                .Targets = getValue("TARGET"),
                .Metabolism = getValue("METABOLISM") _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToArray,
                .Interaction = getValue("INTERACTION") _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToArray,
                .Members = getValue("MEMBER"),
                .Class = getValue("CLASS").__classGroup
            }
        End Function

        Const DrugAndGroup$ = "DG?\d+"

        <Extension>
        Private Function __classGroup(data$()) As NamedCollection(Of String)()
            If data.IsNullOrEmpty Then
                Return {}
            ElseIf data.Length = 1 Then
                Return {
                    New NamedCollection(Of String) With {
                        .Name = data(Scan0)
                    }
                }
            End If

            Dim out As New List(Of NamedCollection(Of String))
            Dim temp As New List(Of String)
            Dim name$ = data(Scan0)

            For Each line As String In data
                If line.Locates(DrugAndGroup) = 1 Then
                    temp += line
                Else
                    out += New NamedCollection(Of String) With {
                        .Name = name,
                        .Value = temp
                    }

                    temp *= 0
                    name = line
                End If
            Next

            out += New NamedCollection(Of String) With {
                .Name = name,
                .Value = temp
            }

            Return out
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="getValue">数据源的函数指针</param>
        ''' <returns></returns>
        <Extension> Public Function CreateDrugModel(getValue As Func(Of String, String())) As Drug
            Return New Drug With {
                .Entry = getValue("ENTRY").FirstOrDefault.Split.First,
                .Names = getValue("NAME") _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Formula = getValue("FORMULA").FirstOrDefault,
                .Exact_Mass = Val(getValue("EXACT_MASS").FirstOrDefault),
                .Mol_Weight = Val(getValue("MOL_WEIGHT").FirstOrDefault),
                .Remarks = getValue("REMARK"),
                .DBLinks = getValue("DBLINKS") _
                    .Select(AddressOf DBLink.FromTagValue) _
                    .ToArray,
                .Efficacy = getValue("EFFICACY").JoinBy(ASCII.TAB),
                .Classes = ClassInheritance _
                    .PopulateClasses(getValue("CLASS")) _
                    .ToArray,
                .Atoms = __atoms(getValue("ATOM")),
                .Bounds = __bounds(getValue("BOUND")),
                .Comments = getValue("COMMENT"),
                .Targets = getValue("TARGET"),
                .Metabolism = getValue("METABOLISM") _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToArray,
                .Interaction = getValue("INTERACTION") _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToArray,
                .Source = getValue("SOURCE") _
                    .Select(Function(s) s.StringSplit(",\s+")) _
                    .IteratesALL _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lines$"></param>
        ''' <param name="ref">假设参考文献都是在每一个小节最末尾的部分</param>
        ''' <returns></returns>
        <Extension>
        Friend Function ParseStream(lines$(), Optional ByRef ref As Reference() = Nothing) As Func(Of String, String())
            Dim list As New Dictionary(Of NamedValue(Of List(Of String)))
            ' 在这里使用空字符串，如果使用Nothing空值的话，添加字典的时候出发生错误
            Dim tag$ = ""
            Dim values As New List(Of String)
            Dim add = Sub()
                          ' 忽略掉original，bracket这些分子结构参数，因为可以很方便的从ChEBI数据库之中获取得到
                          If Not list.ContainsKey(tag) Then
                              list += New NamedValue(Of List(Of String)) With {
                                  .Name = tag,
                                  .Value = values
                              }
                          End If
                      End Sub

            Dim i As i32 = Scan0
            Dim line As New Value(Of String)

            Do While i < lines.Length
                Dim s$ = Mid(line = lines(++i), 1, 12).StripBlank

                If s = "REFERENCE" Then
                    ' 已经到小节的末尾了
                    Dim refList As New List(Of Reference)

                    ' 将前面的数据给添加完
                    Call add()

                    lines = lines.Skip(i.Value - 1).ToArray

                    For Each r As String() In lines.Split(Function(str) InStr(str, "REFERENCE") = 1, includes:=True)
                        refList += New Reference(meta:=r)
                    Next

                    ref = refList

                    Exit Do
                End If

                If s.StringEmpty Then
                    values += (+line).Trim
                Else
                    ' 切换到新的标签
                    Call add()

                    tag = s
                    values = New List(Of String)
                    values += Mid(line, 12).StripBlank
                End If
            Loop

            ' 还会有剩余的数据的，在这里将他们添加上去
            Call add()

            Return Function(key As String) As String()
                       If list.ContainsKey(key) Then
                           Return list(key).Value
                       Else
                           Return {}
                       End If
                   End Function
        End Function

        Private Function __atoms(lines$()) As Atom()
            If lines.IsNullOrEmpty OrElse
                lines(Scan0).ParseInteger = 0 Then
                Return {}
            End If

            Dim out As New List(Of Atom)

            For Each line$ In lines.Skip(1)
                Dim t$() = line.StringSplit("\s+", True)

                out += New Atom With {
                    .index = Val(t(0)),
                    .Formula = t(1),
                    .Atom = t(2),
                    .M = Val(t(3)),
                    .Charge = Val(t(4)),
                    .Edit = t.ElementAtOrDefault(5)
                }
            Next

            Return out
        End Function

        Private Function __bounds(lines$()) As Bound()
            If lines.IsNullOrEmpty OrElse
                lines(Scan0).ParseInteger = 0 Then
                Return {}
            End If

            Dim out As New List(Of Bound)

            For Each line$ In lines.Skip(1)
                Dim t$() = line.StringSplit("\s+", True)

                out += New Bound With {
                    .index = Val(t(0)),
                    .a = Val(t(1)),
                    .b = Val(t(2)),
                    .N = Val(t(3)),
                    .Edit = t.ElementAtOrDefault(4)
                }
            Next

            Return out
        End Function
    End Module
End Namespace
