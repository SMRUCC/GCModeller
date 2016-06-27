Namespace MetaCyc.Schemas

    Public Class Reaction : Inherits SlotSchema(Of MetaCyc.File.DataFiles.Slots.Reaction)
        Public PhysiologicallyRelevant As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Compounds -----(Appears-in-left-side-of)----> Reaction
        ''' </remarks>
        Public Left As MetaCyc.Schemas.Compound()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Compounds -----(Appears-in-right-side-of)-----> Reaction
        ''' </remarks>
        Public Right As MetaCyc.Schemas.Compound()
    End Class
End Namespace
