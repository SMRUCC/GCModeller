#Region "Microsoft.VisualBasic::228a233b0d35dead36f900949e86c4d8, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Reaction.vb"

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
    '    Code Lines: 60
    ' Comment Lines: 182
    '   Blank Lines: 14
    '     File Size: 15.49 KB


    '     Class Reaction
    ' 
    '         Properties: DeltaG0, ECNumber, EnzymaticReaction, EnzymesNotused, Left
    '                     ReactionDirection, RegulatedBy, Reversible, Right, RxnLocations
    '                     Species, Spontaneous, Substrates, Table
    ' 
    '         Function: CreateSchemaItem, GetCoefficient, IsTransportReaction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Reactions are organized into two parallel ontologies. Most reaction frames will have one
    ''' or more parents in both ontologies. The first classifies reactions by the nature of their
    ''' substrates, for example, small-molecule reactions are reactions in which all substrates are
    ''' small molecules, whereas protein reactions are reactions in which at least one substrate
    ''' is a protein. The second ontology classifies reactions by conversion type. For example,
    ''' chemical reactions are those in which a chemical transformation takes place, transport
    ''' reactions are those in which a substrate is transported from one compartment to another
    ''' (some reactions may be both transport reactions and chemical reactions if the substrate is
    ''' chemically altered during transport), and binding reactions are those in which substrates
    ''' weakly bind to each other to form a complex.
    ''' Two novel features of our conceptualization with respect to previous metabolic databases
    ''' are to separate reactions from the enzymes that catalyze them, and to use the EC numbers
    ''' defined by the International Union of Biochemistry and Molecular Biology (IUBMB) to
    ''' uniquely identify reactions, not enzymes. (In database terms, the EC number is a key
    ''' for the Reaction class.) The reason for this separation is that the catalyzes relationship
    ''' between reactions and enzymes is many-to-many: a given enzyme might catalyze more
    ''' than one reaction, and the same reaction might be catalyzed by more than one enzyme.
    ''' Frames in the class Enzymatic-Reaction describe the association between an enzyme and
    ''' a reaction. The entire EC taxonomy can be found under the Chemical-Reactions class.
    ''' You should always write transport reactions in the predominate direction in which the
    ''' reaction occurs. Transport reactions are encoded by labeling substrates with their abstract
    ''' (in vs. out) compartment. For example, if a given substrate is transported from the
    ''' periplasm to the cytosol, it would be labeled with “out” as its compartment as a reactant,
    ''' and with “in” as its compartment as a product. Please see the detailed discussion for the
    ''' Rxn-Locations slot. The default compartment is the cytosol, so the cytosol label may be
    ''' omitted for regular reactions. These labels are implemented as annotations in Ocelot.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Reaction : Inherits [Object]

        ''' <summary>
        ''' This slot holds the EC (Enzyme Commission) number associated with the current reaction,
        ''' if such a number has been assigned by the IUBMB. This slot is single valued.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Name:="EC-NUMBER")> Public Property ECNumber As String

        ' ''' <summary>
        ' ''' The value of this slot is NO if the current reaction either was not defined at all by the
        ' ''' Enzyme Commission, or if the current equation stored for that reaction is not the equation
        ' ''' assigned by the EC (e.g., we have corrected the EC equation). Otherwise, the value is YES,
        ' ''' which is the default inherited value.
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<MetaCycField("OFFICIAL-EC?")> Public Property OfficialEC As Boolean

        ''' <summary>
        ''' These slots hold the compounds from the left and right sides, respectively, of the reaction
        ''' equation. Each value is either the key of a compound frame, or a string that names a
        ''' compound (when the compound is not yet described within the DB as a frame). The terms
        ''' reactant and product are not used because these terms may falsely imply the physiological
        ''' direction of the reaction.
        ''' The coefficient of each substrate, when that coefficient is not equal to 1, is stored as an
        ''' annotation on the substrate value. The annotation label is COEFFICIENT.
        ''' The substrates of transport reactions are also described using the Left and Right slots.
        ''' However, the values of these slots are annotated to indicate their compartments. For
        ''' example, a transporter that moves succinate from the periplasm to the cytosol, accompanied
        ''' by hydrolysis of ATP in the cytosol, would be described with succinate and ATP as
        ''' the values of the Left slot, and with succinate, ADP, and Pi as the values of the Right
        ''' slot. The succinate in the Left slot would be annotated with CCO-OUT under the label
        ''' Compartment. The other substrates need to be annotated with CCO-IN. Additionally, a
        ''' location has to be stored in the Rxn-Locations slot. Please see the detailed comments of
        ''' that slot.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Property Left As List(Of String)
        ''' <summary>
        ''' These slots hold the compounds from the left and right sides, respectively, of the reaction
        ''' equation. Each value is either the key of a compound frame, or a string that names a
        ''' compound (when the compound is not yet described within the DB as a frame). The terms
        ''' reactant and product are not used because these terms may falsely imply the physiological
        ''' direction of the reaction.
        ''' The coefficient of each substrate, when that coefficient is not equal to 1, is stored as an
        ''' annotation on the substrate value. The annotation label is COEFFICIENT.
        ''' The substrates of transport reactions are also described using the Left and Right slots.
        ''' However, the values of these slots are annotated to indicate their compartments. For
        ''' example, a transporter that moves succinate from the periplasm to the cytosol, accompanied
        ''' by hydrolysis of ATP in the cytosol, would be described with succinate and ATP as
        ''' the values of the Left slot, and with succinate, ADP, and Pi as the values of the Right
        ''' slot. The succinate in the Left slot would be annotated with CCO-OUT under the label
        ''' Compartment. The other substrates need to be annotated with CCO-IN. Additionally, a
        ''' location has to be stored in the Rxn-Locations slot. Please see the detailed comments of
        ''' that slot.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Property Right As List(Of String)
        ''' <summary>
        ''' The value of this slot is computed automatically — its values may not be changed by the
        ''' user. The values of the slot are computed as the union of the values of the Left and Right
        ''' slots.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Left和Right字段的值的集合，不能够被用户指定的一个只读属性</remarks>
        Public ReadOnly Property Substrates As String()
            Get
                Dim ChunkBuffer As List(Of String) = Left.AsList
                Call ChunkBuffer.AddRange(Right)
                Return (From strValue As String
                        In ChunkBuffer
                        Select strValue Distinct).ToArray
            End Get
        End Property

        ''' <summary>
        ''' Proteins or protein-RNA complexes listed in this slot are those which would otherwise
        ''' have been inferred to take part in the pathway or reaction, but which in reality do not. In
        ''' other words, the protein may catalyze a general reaction with non-specific substrates, but
        ''' is known not to catalyze this specific form of the reaction.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property EnzymesNotused As String
        ''' <summary>
        ''' This slot contains the change in Gibbs free energy for the reaction in the direction the
        ''' reaction is written.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property DeltaG0 As String
        ''' <summary>
        ''' This slot is true in the case when this reaction occurs spontaneously, that is, it is not
        ''' catalyzed by any enzyme.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property Spontaneous As String
        ''' <summary>
        ''' This slot is used to indicate that a reaction is known to occur in an organism in the case
        ''' where the enzyme that catalyzes the reaction is unknown. In such cases, the value for this
        ''' slot in a given reaction would be the symbolic identifier of the species for the organism
        ''' for the current PGDB.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property Species As String
        ''' <summary>
        ''' This slot is used for storing information about the metabolite compartments of a reaction,
        ''' in the case where non-default compartments are involved. The default compartment is
        ''' defined as the frame CCO-CYTOSOL. There are two cases of reactions:
        ''' 
        '''    S: Reactions that have all of their metabolites in the same compartment.
        ''' 
        '''    T: Reactions that have metabolites in multiple compartments. This can only happen
        '''       at membranes, involving transport reactions or electron transfer reactions (ETRs).
        '''       These reactions may use only the generic directional compartments CCO-IN and
        '''       CCO-OUT for their metabolites, which need to be mapped to the actual compartments
        '''       in a given PGDB for certain operations.
        ''' 
        ''' The values of this slot differ between these cases.
        ''' 
        '''    S: If this reaction occurs in a non-default compartment, or in several compartments,
        '''       then this slot stores for every compartment the corresponding frame (a child of
        '''       CCO-SPACE). In cases where this slot contains a value, and the reaction also occurs
        '''       in the cytosol, then CCO-CYTOSOL must be included as a slot value.
        ''' 
        '''    T: This slot contains one or more frames that are children of CCO-MEMBRANE ,
        '''       or potentially symbols that have to be unique in this slot, for situations where the
        '''       metabolites are in spaces that are not directly adjacent to one membrane, or when 3
        '''       spaces are involved (such as if the transporter spans two membranes). If the reaction
        '''       was not assigned to any particular membrane, then no value is stored, which is the
        '''       default case.
        '''       Additionally, each slot value in this slot will have annotations with the labels 
        '''       CCOIN and CCO-OUT, and in the rare case of 3 compartments involved, also another
        '''       label called CCO-MIDDLE. The values of each of these annotations have to be one valid 
        '''       child of CCO-SPACE. These annotations define the mappings between the COMPARTMENT 
        '''       annotation values, which the metabolites have that are listed in the reaction’s Left 
        '''       and Right slots, and the final compartments in this PGDB.
        '''       Every metabolite in the reaction’s Left and Right slots needs to have a COMPARTMENT
        '''       annotation, the value of which needs to be one of CCO-IN, CCO-OUT, or possibly 
        '''       CCO-MIDDLE in complex situations.
        ''' 
        ''' If the reaction is catalyzed by more than one enzyme (i.e. it has more than one 
        ''' enzymaticreaction attached), then each value in the RXN-LOCATIONS slot has to have an 
        ''' annotation called ENZRXNS, which has as its values the frame IDs of the corresponding
        ''' enzymatic-reactions. This allows determining the precise compartment(s) in which the
        ''' catalyzed reaction is occurring.
        ''' Whenever a reaction is transferred between PGDBs (by import or schema upgrade operations),
        ''' all values in the RXN-LOCATIONS are filtered away (i.e. not copied). This
        ''' prevents inapplicable compartments from being introduced into other PGDBs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property RxnLocations As String

        <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Property EnzymaticReaction As List(Of String)

        <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Property RegulatedBy As List(Of String)

        <MetaCycField()> Public Property ReactionDirection As String

        Public Function IsTransportReaction() As Boolean
            Dim LQuery = (From strLine As String
                          In Me._innerHash.Values.IteratesALL
                          Where Regex.Match(strLine, "\[\^COMPARTMENT - .+?\]").Success
                          Select strLine).FirstOrDefault
            Return Not String.IsNullOrEmpty(LQuery)
        End Function

        ''' <summary>
        ''' 在左端返回-1，在右端返回1，不存在则返回0
        ''' </summary>
        ''' <param name="MetaboliteId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCoefficient(MetaboliteId As String) As Integer
            If Left.IndexOf(MetaboliteId) = -1 Then
                If Right.IndexOf(MetaboliteId) = -1 Then
                    Return 0
                Else
                    Return 1
                End If
            Else
                Return -1
            End If
        End Function

        Public ReadOnly Property Reversible As Boolean
            Get
                Return InStr(Me.ReactionDirection, "REVERSIBLE") > 0
            End Get
        End Property

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.reactions
            End Get
        End Property

        Public Function CreateSchemaItem() As MetaCyc.Schema.Metabolism.Reaction
            Return MetaCyc.Schema.Metabolism.Reaction.CreateObject(Me)
        End Function
    End Class
End Namespace
