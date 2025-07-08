#Region "Microsoft.VisualBasic::a05e2c150c491efa9bbf8f5986ffb7dd, models\BioCyc\Models\reactions.vb"

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

    '   Total Lines: 100
    '    Code Lines: 90 (90.00%)
    ' Comment Lines: 1 (1.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (9.00%)
    '     File Size: 3.96 KB


    ' Class reactions
    ' 
    '     Properties: atomMappings, cannotBalance, ec_number, enzymaticReaction, equation
    '                 gibbs0, inPathway, left, orphan, physiologicallyRelevant
    '                 reactionBalanceStatus, reactionDirection, reactionList, reactionLocations, right
    '                 signal, species, spontaneous, systematicName
    ' 
    '     Function: (+2 Overloads) OpenFile, ParseText, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

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
<Xref("reactions.dat")>
Public Class reactions : Inherits Model

    ''' <summary>
    ''' This slot holds the EC (Enzyme Commission) number associated with the current reaction,
    ''' if such a number has been assigned by the IUBMB. This slot is single valued.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("EC-NUMBER")>
    Public Property ec_number As ECNumber
    <AttributeField("ENZYMATIC-REACTION")>
    Public Property enzymaticReaction As String()
    <AttributeField("GIBBS-0")>
    Public Property gibbs0 As Double
    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
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
    <AttributeField("LEFT")>
    Public Property left As CompoundSpecieReference()
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
    <AttributeField("RIGHT")>
    Public Property right As CompoundSpecieReference()
    <AttributeField("PHYSIOLOGICALLY-RELEVANT?")>
    Public Property physiologicallyRelevant As Boolean
    <AttributeField("REACTION-BALANCE-STATUS")>
    Public Property reactionBalanceStatus As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()

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
    <AttributeField("RXN-LOCATIONS")>
    Public Property reactionLocations As String()
    <AttributeField("SPONTANEOUS?")>
    Public Property spontaneous As Boolean
    <AttributeField("ATOM-MAPPINGS")>
    Public Property atomMappings As String()
    <AttributeField("ORPHAN?")>
    Public Property orphan As String
    <AttributeField("SYSTEMATIC-NAME")>
    Public Property systematicName As String
    <AttributeField("CANNOT-BALANCE?")>
    Public Property cannotBalance As Boolean
    <AttributeField("SIGNAL")>
    Public Property signal As String

    ''' <summary>
    ''' This slot is used to indicate that a reaction is known to occur in an organism in the case
    ''' where the enzyme that catalyzes the reaction is unknown. In such cases, the value for this
    ''' slot in a given reaction would be the symbolic identifier of the species for the organism
    ''' for the current PGDB.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("SPECIES")>
    Public Property species As String

    Public ReadOnly Property equation As Equation
        Get
            Dim uid As String = MyBase.ToString

            Select Case reactionDirection
                Case ReactionDirections.IrreversibleLeftToRight,
                     ReactionDirections.LeftToRight,
                     ReactionDirections.PhysiolLeftToRight

                    Return New Equation(uid) With {
                        .reversible = False,
                        .Reactants = left,
                        .Products = right
                    }
                Case ReactionDirections.Reversible
                    Return New Equation(uid) With {
                        .Reactants = left,
                        .Products = right,
                        .reversible = True
                    }
                Case Else
                    ' right to left
                    Return New Equation(uid) With {
                        .reversible = False,
                        .Reactants = right,
                        .Products = left
                    }
            End Select
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return equation.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of reactions)
        Return AttrDataCollection(Of reactions).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As String) As AttrDataCollection(Of reactions)
        Using s As Stream = file.OpenReadonly
            Return OpenFile(s)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of reactions)
        Return AttrDataCollection(Of reactions).LoadFile(New StringReader(data))
    End Function
End Class
