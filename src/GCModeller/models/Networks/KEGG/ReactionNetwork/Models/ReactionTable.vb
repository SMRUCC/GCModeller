#Region "Microsoft.VisualBasic::1883cce7c553cf13898fcc874b6b0020, GCModeller\models\Networks\KEGG\ReactionNetwork\Models\ReactionTable.vb"

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

'   Total Lines: 157
'    Code Lines: 101
' Comment Lines: 39
'   Blank Lines: 17
'     File Size: 6.18 KB


'     Class ReactionTable
' 
'         Properties: definition, EC, entry, geneNames, KO
'                     name, products, substrates
' 
'         Function: creates, (+3 Overloads) Load, loadXmls, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data

Namespace ReactionNetwork

    ''' <summary>
    ''' A simplify data model of KEGG reaction object.
    ''' 
    ''' 对一个代谢反应过程的描述
    ''' </summary>
    Public Class ReactionTable : Implements INamedValue

        ''' <summary>
        ''' 反应编号
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As String Implements INamedValue.Key
        Public Property name As String
        Public Property definition As String

        ''' <summary>
        ''' 酶编号，可以通过这个编号和相对应的基因或者KO编号关联起来
        ''' </summary>
        ''' <returns></returns>
        Public Property EC As String()
        ''' <summary>
        ''' 和<see cref="EC"/>几乎是一个意思,只不过通过这个属性值可以更加的容易与相应的基因进行关联
        ''' </summary>
        ''' <returns></returns>
        Public Property KO As String()
        ''' <summary>
        ''' Each element in this array of name string is corresponding 
        ''' to the <see cref="KO"/> property.
        ''' </summary>
        ''' <returns></returns>
        Public Property geneNames As String()

        ''' <summary>
        ''' 底物列表
        ''' </summary>
        ''' <returns></returns>
        Public Property substrates As String()
        ''' <summary>
        ''' 产物列表
        ''' </summary>
        ''' <returns></returns>
        Public Property products As String()

        Public Overrides Function ToString() As String
            Return name
        End Function

        ''' <summary>
        ''' load network table data in auto detection mode 
        ''' </summary>
        ''' <param name="br08201">
        ''' <see cref="Reaction"/>
        ''' </param>
        ''' <returns></returns>
        Public Shared Function Load(br08201 As String) As IEnumerable(Of ReactionTable)
            If br08201.FileExists AndAlso br08201.ExtensionSuffix("csv") Then
                Return br08201.LoadCsv(Of ReactionTable)
            Else
                Return br08201.DoCall(AddressOf loadXmls)
            End If
        End Function

        Private Shared Iterator Function loadXmls(br08201 As String) As IEnumerable(Of ReactionTable)
            Dim proc As New SwayBar
            Dim model As ReactionTable = Nothing
            Dim KOnames As Dictionary(Of String, BriteHText) = DefaultKOTable()

            For Each file As String In (ls - l - r - {"*.XML", "*.xml"} <= br08201)
                Try
                    model = Reaction _
                        .LoadXml(handle:=file) _
                        .DoCall(Function(r)
                                    Return creates(r, KOnames)
                                End Function)

                    ' populate data from xml load result
                    ' if success
                    Yield model
                Catch ex As Exception
                    Call file.PrintException
                    Call App.LogException(ex)
                Finally
                    Call proc.Step(model?.name)
                End Try
            Next
        End Function

        ''' <summary>
        ''' convert from reaction model to current reaction table model
        ''' </summary>
        ''' <param name="repo"></param>
        ''' <returns></returns>
        Public Shared Function Load(repo As IEnumerable(Of Reaction)) As IEnumerable(Of ReactionTable)
            Dim KOnames As Dictionary(Of String, BriteHText) = DefaultKOTable()
            Dim table = repo.Select(Function(r) creates(r, KOnames))

            Return table
        End Function

        Public Shared Function Load(repo As ReactionRepository) As IEnumerable(Of ReactionTable)
            Dim KOnames As Dictionary(Of String, BriteHText) = DefaultKOTable()
            Dim table = repo.metabolicNetwork.Select(Function(r) creates(r, KOnames))

            Return table
        End Function

        Private Shared Function creates(xml As Reaction, KOnames As Dictionary(Of String, BriteHText)) As ReactionTable
            Dim eq As DefaultTypes.Equation = xml.ReactionModel
            Dim rxnName$ = xml.CommonNames.SafeQuery.FirstOrDefault Or xml.Definition.AsDefault
            Dim KOlist$() = xml.Orthology?.Terms.SafeQuery.Keys
            Dim enzymes = xml.Enzyme
            Dim geneNames As String() = KOlist _
                .Select(Function(id)
                            If KOnames.ContainsKey(id) Then
                                Return KOnames(id).description _
                                    .Split(";"c) _
                                    .First
                            Else
                                Return id
                            End If
                        End Function) _
                .ToArray

            If Not enzymes Is Nothing Then
                enzymes = enzymes _
                    .Where(Function(str) Not str.StringEmpty) _
                    .ToArray
            End If

            If geneNames.IsNullOrEmpty Then
                If enzymes.IsNullOrEmpty Then
                    geneNames = {xml.ID}
                Else
                    geneNames = {$"{xml.ID} [{enzymes.JoinBy(", ")}]"}
                End If
            End If

            Return New ReactionTable With {
                .definition = xml.Definition,
                .EC = enzymes,
                .entry = xml.ID,
                .name = rxnName,
                .products = eq.Products _
                    .Select(Function(cp) cp.ID) _
                    .ToArray,
                .substrates = eq.Reactants _
                    .Select(Function(cp) cp.ID) _
                    .ToArray,
                .KO = KOlist,
                .geneNames = geneNames
            }
        End Function
    End Class
End Namespace
