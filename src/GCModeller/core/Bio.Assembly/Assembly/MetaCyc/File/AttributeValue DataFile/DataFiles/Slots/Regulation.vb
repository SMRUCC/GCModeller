#Region "Microsoft.VisualBasic::952dc7850344adbd5671e4be247c4f17, Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Regulation.vb"

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

    '     Class Regulation
    ' 
    '         Properties: AssociatedBindingSite, IsMetabolismRegulation, Ki, Mechanism, Mode
    '                     PhysiologicallyRelevant, RegulatedEntity, Regulator, Table
    '         Interface IRegulator
    ' 
    '             Properties: CommonName, ComponentOf, Regulates, Types, UniqueId
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' This class describes most forms of protein, RNA or activity regulation. Regulation can
    ''' be either by a direct influence on the protein’s activity (e.g. allosteric inhibition of an
    ''' enzyme) or by influencing the quantity of active protein available (e.g. by inducing or
    ''' blocking its transcription or translation). The one form of regulation that is not covered
    ''' by this class is when the quantity of a protein is regulated as a result of chemical
    ''' or binding reactions that either produce or consume the active form of a protein – these
    ''' are represented as Reactions instead. There can be some ambiguity as to what should be
    ''' represented as a reaction and what should be represented as a regulation event. In general,
    ''' an event that can be represented as a reaction should be when a) there is sufficiently
    ''' detailed information known to model it as a reaction, b) both reactants and products exist
    ''' as stable, independent entities, and c) our schema supports referring to both reactant
    ''' and product of the reaction independently and there is some justification for wanting
    ''' to go down to that level of detail. For example, a transcription factor bound to a small
    ''' molecule will generally have a different activity than the unbound transcription factor.
    ''' This could be represented either as the reaction TF + x -¿ TF-x or as a regulation event in
    ''' which x activates or inhibits the activity of TF. However, because both TF and TF-x are
    ''' stable molecules which can potentially regulate different transcription units (not all will,
    ''' but some do), or TF could bind another small molecule y and regulate yet another set of
    ''' transcription units, we prefer to model this kind of interaction as a reaction when the data
    ''' is available. On the other hand, an enzyme binding to some inhibitor could also be represented
    ''' as a reaction, but since there is rarely any reason to refer to the enzyme-inhibitor
    ''' complex outside of the context of the reaction the enzyme catalyzes, we choose instead
    ''' to model these events as regulation events in which the inhibitor regulates the activity of
    ''' the enzyme.
    ''' Instances of this class represent a one-to-one mapping between regulator and regulatedentity
    ''' (i.e. an entity may regulate many processes, or a process may be regulated by many
    ''' entities, but each one requires its own instance of Regulation to represent it)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulation : Inherits [Object]

        ''' <summary>
        ''' This slot is applicable to regulation of transcription or translation in which an entity 
        ''' (protein, small-molecule or RNA) binds to DNA or the mRNA transcript. Its values are 
        ''' instances of either DNA-Binding-Sites or mRNA-Binding-Sites, depending on the type of
        ''' regulation.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("", "", ExternalKey.Directions.Out)> <MetaCycField> Public Property AssociatedBindingSite As String

        ''' <summary>
        ''' This slot optionally contains a keyword which describes the mechanism of the regulation.
        ''' Appropriate possible values will vary depending on the particular subclass of regulation.
        ''' Some subclasses will not use this slot at all.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField> Public Property Mechanism As String

        ''' <summary>
        ''' This slot specifies whether the regulator activates or inhibits the regulated-entity. Possible
        ''' values are:
        ''' “+” — The regulator activates or increases quantity or activity of the regulated-entity
        ''' (an exception is transcription attenuation, in which even though the regulated-entity is a
        ''' terminator object, “+” means activation of transcription of the downstream genes rather
        ''' than of the terminator).
        ''' “-” — The regulator inhibits or decreases quantity or activity of the regulated-entity (with
        ''' the same caveat about transcription attenuation as above)..
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField> Public Property Mode As String

        ''' <summary>
        ''' This slot links the regulation frame to the object that is being regulated. In the case of
        ''' enzyme modulation, this object will be an Enzymatic-Reaction frame. In the case of transcription
        ''' initiation regulation, it will be a Promoter frame. In the case of transcription
        ''' attenuation, it will be a Terminator frame. In other cases, it could be a gene or a protein
        ''' frame. The regulated entity will link back to the regulation frame using the inverse of this
        ''' slot, Regulated-By
        ''' (本属性连接本类型的对象至被其所调控的目标对象。当调控对象为酶活力调控的时候，将会链接至酶促反应对象，
        ''' 当调控类型为转录起始调控的时候，目标对象将会是启动子对象，当为转录终止调控的时候，目标对象将会是一个
        ''' 终止子对象，对于其他情况而言，所调控的目标对象可能为一个基因或者蛋白质。对于被调控的目标对象而言，都
        ''' 会有一个Regulated-By属性连接回本对象之上)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("", "", ExternalKey.Directions.Out)> <MetaCycField> Public Property RegulatedEntity As String

        ''' <summary>
        ''' This slot links the regulation frame to the object that is doing the regulating, typically
        ''' a protein, RNA or small molecule. The regulator frame will link back to the regulation
        ''' frame using the inverse of this slot, Regulates.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("", "", ExternalKey.Directions.In)> <MetaCycField> Public Property Regulator As String

        ''' <summary>
        ''' This slot is used for instances of regulation of enzyme activity. Ki is the dissociation
        ''' constant for the binding of an inhibitor to an enzyme or an enzyme-substrate complex.
        ''' When the inhibitor is competitive, Ki is the dissociation constant for the binding of an
        ''' inhibitor to the enzyme, and is often written as Kic. When the inhibitor is uncompetitive,
        ''' Ki is the dissociation constant for the binding of an inhibitor to the enzyme-substrate
        ''' complex, and is often written as Kiu or Ki. The units for Ki are mole.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField> Public Property Ki As String

        Public Shared ReadOnly ReactionRegulation As String() = {"Regulation-of-Enzyme-Activity", "Regulation-of-Reactions"}

        <MetaCycField(name:="PHYSIOLOGICALLY-RELEVANT?", type:=MetaCycField.Types.String)> Public Property PhysiologicallyRelevant As String

        ''' <summary>
        ''' 当前的调控类型对象是否为对酶促反应的调控类型对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsMetabolismRegulation As Boolean
            Get
                Return Array.IndexOf(Regulation.ReactionRegulation, Types) > -1
            End Get
        End Property

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.regulation
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As MetaCyc.File.DataFiles.Slots.Regulation
        '    Dim NewObj As MetaCyc.File.DataFiles.Slots.Regulation = New MetaCyc.File.DataFiles.Slots.Regulation

        '    Call MetaCyc.File.DataFiles.Slots.Object.TypeCast(Of MetaCyc.File.DataFiles.Slots.Regulation) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Regulations.AttributeList, e), NewObj)

        '    If NewObj.Exists("MECHANISM") Then NewObj.Mechanism = NewObj.Object("MECHANISM") Else NewObj.Mechanism = String.Empty
        '    If NewObj.Exists("MODE") Then NewObj.Mode = NewObj.Object("MODE") Else NewObj.Mode = String.Empty
        '    If NewObj.Exists("REGULATED-ENTITY") Then NewObj.RegulatedEntity = NewObj.Object("REGULATED-ENTITY") Else NewObj.RegulatedEntity = String.Empty
        '    If NewObj.Exists("REGULATOR") Then NewObj.Regulator = NewObj.Object("REGULATOR") Else NewObj.Regulator = String.Empty

        '    Return NewObj
        'End Operator

        ''' <summary>
        ''' An object type that it has the ability to regulates the gene expression process.
        ''' (能够调控基因表达过程的一种对象类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IRegulator : Inherits IComplexes

            ''' <summary>
            ''' The unique identifier in the metacyc database of this regulator object.
            ''' (本调控因子对象在MetaCyc数据库之中的唯一标识符)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property UniqueId As String
            ''' <summary>
            ''' The displaying name of this regulator object in the citation articles.
            ''' (本调控因子对象在引用文献中的名称)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property CommonName As String
            ''' <summary>
            ''' 本类型的对象最为其他的对象的组件而存在的时候，则本属性值指明了本对象所能够构成的对象的Unique-Id列表
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property ComponentOf As List(Of String)
            ''' <summary>
            ''' The type of this regulator object.(本调控因子对象的类型)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property Types As List(Of String)
            ''' <summary>
            ''' The regulates target that this regulator entity affect.(本调控因子对象所影响到的调控对象的UniqueId列表)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property Regulates As List(Of String)
        End Interface
    End Class
End Namespace
