#Region "Microsoft.VisualBasic::5dea98493ed7b263091e596afbe4b5be, CLI_tools\gcc\CLI\ProteinDomain.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::88ad8b9fdecb7842118482e73ba318de, CLI_tools\gcc\CLI\ProteinDomain.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module ProteinDomain
'    ' 
'    '     Function: AddingRules, GetList, SMART
'    '     Class Extend
'    ' 
'    '         Function: Invoke
'    ' 
'    '     Class Rule
'    ' 
'    '         Properties: CheckThrought, Left, Right
'    ' 
'    '         Constructor: (+2 Overloads) Sub New
'    '         Function: Copy, Generate, GetDomains, ToString
'    '         Class Refx
'    ' 
'    '             Properties: DomainModify, IsDomain, Stochem, UniqueId
'    ' 
'    '             Constructor: (+2 Overloads) Sub New
'    '             Function: CopyData, ToString
'    ' 
'    ' 
'    ' 
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.Text
'Imports System.Text.RegularExpressions
'Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
'Imports Microsoft.VisualBasic.Data.csv
'Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
'Imports SMRUCC.genomics.GCModeller.Assembly
'Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage

'''' <summary>
'''' 利用Smart程序分析目标蛋白质结构域，方便进行信号传导网络的重建工作
'''' </summary>
'''' <remarks></remarks>
'Module ProteinDomain

'    ''' <summary>
'    ''' 以Pfam数据库为准
'    ''' </summary>
'    ''' <param name="TargetFile">目标蛋白质序列数据库</param>
'    ''' <param name="ExportSaved">保存的结果数据CSV文件的文件路径</param>
'    ''' <remarks></remarks>
'    Public Function SMART(TargetFile As String, GrepText As String, ExportSaved As String) As IO.File
'        If String.IsNullOrEmpty(GrepText) Then
'            GrepText = "tokens | 2"
'        End If
'        Dim Args As String = String.Format("-build_cache -i ""{0}"" -o ""{1}"" -db Pfam -grep_script ""{2}""", TargetFile, ExportSaved, GrepText)
'        Dim Process As Microsoft.VisualBasic.CommandLine.IORedirect = New CommandLine.IORedirect(My.Application.Info.DirectoryPath & "/SMART.exe", Args)
'        Call printf("Start to performence the protein domain architecture analysis...")
'        AddHandler Process.PrintOutput, AddressOf Console.WriteLine
'        Call Process.Start(WaitForExit:=True)
'        Call printf("END_OF_SMART_ANALYSIS")

'        Return IO.File.Load(ExportSaved)
'    End Function

'    Public Function AddingRules(MetaCyc As DatabaseLoadder,
'                                RuleFile As IO.File,
'                                Model As BacterialModel,
'                                GrepScript As String,
'                                modelFilePath$) As Integer
'        Dim Domains As IO.File = SMART(MetaCyc.Database.FASTAFiles.ProteinSourceFile, GrepScript, modelFilePath & ".csv") '获取所有的蛋白质结构域
'        Dim Rules = (From row In RuleFile Select New Rule(row.First)).ToArray
'        Dim DomainDistributed = GetList(Rules, Domains)
'        Dim InvokesResult = (From rule In Rules Select Extend.Invoke(rule, DomainDistributed)).ToArray
'        For Each Collection In InvokesResult
'            Call Model.ProteinAssemblies.AddRange((From item In Collection Select item.Generate).ToArray)
'        Next
'        Model.ProteinAssemblies = Model.ProteinAssemblies
'        For i As Integer = 0 To Model.BacteriaGenome.Genes.Count - 1
'            '     Dim Protein = Model.BacteriaGenome.Genes(i).TranslateProtein
'            '    Dim DomainList As String() = Split((From row In Domains Where String.Equals(row.First, Protein.UniqueId) Select row).First()(3), ", ")
'            '  Model.BacteriaGenome.Genes(i).ProteinProduct = New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.GeneObject.Protein With {.Domains = DomainList, .UniqueId = Protein.UniqueId}
'        Next
'        Call Model.Save(modelFilePath)

'        Return Model.ProteinAssemblies.Count
'    End Function

'    Public Class Extend
'        ''' <summary>
'        ''' 假设Domain没有发生变化被反应掉，其仅是被修饰了整个展开过程为一个递归的过程
'        ''' </summary>
'        ''' <param name="Rule"></param>
'        ''' <param name="DomainDistributionList"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Shared Function Invoke(Rule As Rule, DomainDistributionList As Dictionary(Of String, String())) As Rule()
'            If Rule.CheckThrought Then
'                Return New Rule() {Rule}
'            Else
'                Call printf("Expending rule:: %s", Rule.ToString)
'            End If
'            Dim NewRuleList As List(Of Rule) = New List(Of Rule)

'            For i As Integer = 0 To Rule.Left.Count - 1
'                Dim refx = Rule.Left(i)
'                If refx.IsDomain Then
'                    Dim ProteinList As String() = DomainDistributionList(refx.UniqueId)
'                    For Each Protein In ProteinList
'                        Dim NewRule As Rule = Rule.Copy
'                        NewRule.Left(i).IsDomain = False
'                        If Not String.IsNullOrEmpty(NewRule.Left(i).DomainModify) Then
'                            NewRule.Left(i).UniqueId = Protein & "-" & NewRule.Left(i).DomainModify
'                        Else
'                            NewRule.Left(i).UniqueId = Protein
'                        End If
'                        For Each index In NewRule.Left(i).RelatedIndex
'                            If Not String.IsNullOrEmpty(NewRule.Right(index).DomainModify) Then
'                                NewRule.Right(index).UniqueId = Protein & "-" & NewRule.Right(index).DomainModify
'                            Else
'                                NewRule.Right(index).UniqueId = Protein
'                            End If
'                            NewRule.Right(index).IsDomain = False
'                        Next
'                        Call NewRuleList.AddRange(Invoke(NewRule, DomainDistributionList))
'                    Next
'                End If
'            Next

'            Return NewRuleList.ToArray
'        End Function
'    End Class


'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <returns>{DomainId, Protein-UniqueId()}</returns>
'    ''' <remarks></remarks>
'    Private Function GetList(Rules As Rule(), DomainDistributionList As IO.File) As Dictionary(Of String, String())
'        Dim AllDomains As List(Of String) = New List(Of String)
'        For Each Rule In Rules
'            Call AllDomains.AddRange(Rule.GetDomains)
'        Next
'        AllDomains = AllDomains.Distinct.AsList
'        Dim DomainDistributed As Dictionary(Of String, String()) = New Dictionary(Of String, String())
'        For Each DomainId As String In AllDomains
'            Call DomainDistributed.Add(DomainId, (From row In DomainDistributionList Let List = row(3) Where InStr(List, DomainId) Select row.First).ToArray)
'        Next
'        Return DomainDistributed
'    End Function

'    Public Class Rule

'        Public Class Refx
'            Public Property UniqueId As String
'            Public Property Stochem As Integer
'            Public Property IsDomain As Boolean = False
'            ''' <summary>
'            ''' 结构域修饰
'            ''' </summary>
'            ''' <value></value>
'            ''' <returns></returns>
'            ''' <remarks></remarks>
'            Public Property DomainModify As String

'            ''' <summary>
'            ''' 假若目标为一个结构域的话，则本属性为左端的结构域在右端所对应的位置
'            ''' </summary>
'            ''' <remarks></remarks>
'            Protected Friend RelatedIndex As List(Of Integer) = New List(Of Integer)

'            Sub New(ref As String)
'                Stochem = Val(Regex.Match(ref, "^\d+ ").Value)
'                Stochem = If(Stochem = 0, 1, Stochem)
'                If Stochem = 1 Then
'                    UniqueId = ref
'                Else
'                    UniqueId = Mid(ref, Len(Stochem.ToString) + 2)
'                End If

'                Dim collection = Regex.Matches(UniqueId, "\[[^]]+\]")
'                If collection.Count > 0 Then
'                    IsDomain = True
'                    ref = collection.Item(0).Value
'                    ref = collection.Item(0).Value.Split(CChar(":")).Last
'                    UniqueId = Mid(ref, 1, Len(ref) - 1)
'                    If collection.Count > 1 Then
'                        ref = collection.Item(1).Value
'                        DomainModify = Mid(ref, 2, ref.Length - 2)
'                    End If
'                End If
'            End Sub

'            Protected Friend Sub New()
'            End Sub

'            Public Function CopyData() As Refx
'                Return New Refx With {.DomainModify = DomainModify, .IsDomain = IsDomain, .Stochem = Stochem, .UniqueId = UniqueId, .RelatedIndex = RelatedIndex}
'            End Function

'            Public Overrides Function ToString() As String
'                If IsDomain Then
'                    Dim str = String.Format("{0} [DM:{1}]", Stochem, UniqueId)
'                    If Not String.IsNullOrEmpty(DomainModify) Then
'                        str = str & String.Format(" [{0}]", DomainModify)
'                    End If
'                    Return str
'                Else
'                    Return String.Format("{0} {1}", Stochem, UniqueId)
'                End If
'            End Function
'        End Class

'        Public Property Left As Refx()
'        Public Property Right As Refx()

'        Sub New(str As String)
'            Dim Tokens As String() = Strings.Split(str, "->")
'            Me.Left = (From ref As String In Strings.Split(Tokens.First, " + ") Select New Refx(ref)).ToArray
'            Me.Right = (From ref As String In Strings.Split(Tokens.Last, " + ") Select New Refx(ref)).ToArray

'            For i As Integer = 0 To Left.Count - 1
'                If Not Left(i).IsDomain Then
'                    Continue For
'                End If
'                Dim DomainId As String = Left(i).UniqueId
'                For j As Integer = 0 To Right.Count - 1
'                    If String.Equals(Right(j).UniqueId, DomainId) Then
'                        Call Left(i).RelatedIndex.Add(j)
'                    End If
'                Next
'            Next
'        End Sub

'        Public ReadOnly Property CheckThrought() As Boolean
'            Get
'                Return (From refx In Left Where refx.IsDomain Select 1).ToArray.Count = 0
'            End Get
'        End Property

'        Protected Friend Sub New()
'        End Sub

'        ''' <summary>
'        ''' 获取本条规则中的所有的涉及到的结构域Id集合
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Function GetDomains() As String()
'            Return (From refx In Left Where refx.IsDomain Select refx.UniqueId).ToArray
'        End Function

'        Public Function Generate() As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
'            Dim Reaction As New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
'            Dim sBuilder As StringBuilder = New StringBuilder(1024)
'            For Each Refx In Left
'                Call sBuilder.Append(Refx.UniqueId & "-")
'            Next
'            Call sBuilder.Remove(sBuilder.Length - 1, 1)
'            Reaction.Identifier = sBuilder.ToString
'            Reaction.Reactants = (From refx In Left Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = refx.UniqueId, .StoiChiometry = refx.Stochem}).ToArray
'            Reaction.Products = (From refx In Right Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = refx.UniqueId, .StoiChiometry = refx.Stochem}).ToArray
'            Reaction.UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10}

'            Return Reaction
'        End Function

'        Public Function Copy() As Rule
'            Dim Rule As Rule = New Rule
'            Rule.Left = (From refx In Left Select refx.CopyData()).ToArray
'            Rule.Right = (From refx In Right Select refx.CopyData).ToArray

'            Return Rule
'        End Function

'        Public Overrides Function ToString() As String
'            Dim sBuilder As StringBuilder = New StringBuilder(1024)
'            For Each Refx In Left
'                Call sBuilder.Append(Refx.ToString & " + ")
'            Next
'            Call sBuilder.Remove(sBuilder.Length - 3, 3)
'            Call sBuilder.Append(" -> ")
'            For Each Refx In Right
'                Call sBuilder.Append(Refx.ToString & " + ")
'            Next
'            Call sBuilder.Remove(sBuilder.Length - 3, 3)

'            Return sBuilder.ToString
'        End Function
'    End Class
'End Module
