Imports LANS.SystemsBiology.Assembly.SBML.FLuxBalanceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.SBML.Specifics.MetaCyc

Namespace Components

    Public Class Specie : Inherits IPartsBase
        Implements sIdEnumerable

        <Escaped> <XmlAttribute>
        Public Overrides Property id As String Implements sIdEnumerable.Identifier

        <Escaped>
        <XmlAttribute("compartment")>
        Public Overridable Property CompartmentID As String
        <XmlAttribute()>
        Public Property boundaryCondition As Boolean

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1} [{2}]", id, name, CompartmentID)
        End Function
    End Class
End Namespace