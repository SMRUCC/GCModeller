#Region "Microsoft.VisualBasic::095c735bf8786c3d6387701e35aed2bd, models\Networks\KEGG\ReactionTable.vb"

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

' Class ReactionTable
' 
'     Properties: definition, EC, entry, KO, name
'                 products, substrates
' 
'     Function: creates, Load, ToString
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data

Namespace ReactionNetwork

    ''' <summary>
    ''' A simplify data model of KEGG reaction object.
    ''' 
    ''' 对一个代谢反应过程的描述
    ''' </summary>
    Public Class ReactionTable

        ''' <summary>
        ''' 反应编号
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As String
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
        ''' 
        ''' </summary>
        ''' <param name="br08201">
        ''' <see cref="Reaction"/>
        ''' </param>
        ''' <returns></returns>
        Public Shared Iterator Function Load(br08201 As String) As IEnumerable(Of ReactionTable)
            Dim proc As New SwayBar
            Dim model As ReactionTable = Nothing

            For Each file As String In (ls - l - r - "*.XML" <= br08201)
                Try
                    model = Reaction.LoadXml(handle:=file).DoCall(AddressOf creates)
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

        Public Shared Function Load(repo As ReactionRepository) As IEnumerable(Of ReactionTable)
            Return repo.metabolicNetwork.Select(AddressOf creates)
        End Function

        Private Shared Function creates(xml As Reaction) As ReactionTable
            Dim eq As DefaultTypes.Equation = xml.ReactionModel
            Dim rxnName$ = xml.CommonNames.SafeQuery.FirstOrDefault Or xml.Definition.AsDefault
            Dim KOlist$() = xml.Orthology?.Terms.SafeQuery.Keys

            Return New ReactionTable With {
                .definition = xml.Definition,
                .EC = xml.Enzyme,
                .entry = xml.ID,
                .name = rxnName,
                .products = eq.Products _
                    .Select(Function(cp) cp.ID) _
                    .ToArray,
                .substrates = eq.Reactants _
                    .Select(Function(cp) cp.ID) _
                    .ToArray,
                .KO = KOlist
            }
        End Function
    End Class
End Namespace